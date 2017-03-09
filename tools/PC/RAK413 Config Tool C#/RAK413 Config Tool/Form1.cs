using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.IO;
using System.Threading;
using System.Globalization;
using CodeProject.Dialog;//引用显示类库


namespace RAK413_Config_Tool
{
    public partial class Form1 : Form
    {
        private SerialPort comm = new SerialPort();
        private int bandrate = 0;
        int n = 0;
        private int len = 0, length = 0;
        byte[] buf = new byte[5000];                    //声明一个临时数组存储当前来的串口数据
        string bufstring;
        bool mac = false;
        bool version = false;
        bool scan = false;
        bool get_scan = false;
        bool psk = false;
        bool staconnect = false;
        bool apconnect = false;
        bool adhocconnect = false;
        bool disc = false;
        bool dhcp = false;
        bool manual = false;
        bool ltcp = false;
        bool tcp = false;
        bool ludp = false;
        bool udp = false;
        bool send = false;
        bool cls = false;
        int scan_num = 0;
        string CMD_Return = "ACK";
        string[] destport = { "0", "0", "0", "0", "0", "0", "0", "0"};//记录socket ID对应的目标端口
        string[] destip = { "0", "0", "0", "0", "0", "0", "0", "0" };//记录socket ID对应的目标IP
        OpenFileDialog getconfigdata = new OpenFileDialog();
        SaveFileDialog saveFileDialogcfg = new SaveFileDialog();
        bool getconfig = false;
        bool setconfig = false;
        bool getweb = false;
        bool setweb = false;
        bool powermode = false;
        bool uartconfig = false;
        bool easyconfig = false;
        bool wps = false;
        bool autoconnect = false;
        bool webserver = false;
        bool dhcpsevse = false;
        bool channel = false;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化下拉串口名称列表框
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboPortName.Items.AddRange(ports);
            comboPortName.SelectedIndex = comboPortName.Items.Count > 0 ? 0 : -1;
            comboBaudrate.SelectedIndex = comboBaudrate.Items.IndexOf("115200");
        }
        //打开串口
        private void buttonOpenBaudrate_Click(object sender, EventArgs e)
        {
            //根据当前串口对象，来判断操作
            if (comm.IsOpen)
            {
                //打开时点击，则关闭串口
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Black;
                this.textBoxPlay.AppendText("Serial Port Closed.\r\n");
                buttonScan.Enabled = false;
                comm.Close();
            }
            else
            {
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Black;
                this.textBoxPlay.AppendText("Serial Port Opened.\r\n");
                
                buttonScan.Enabled = true;
                //关闭时点击，则设置好端口，波特率后打开
                comm.PortName = comboPortName.Text;                           //端口名
                comm.BaudRate = int.Parse(comboBaudrate.Text);                //波特率

                try
                {
                    comm.Open();
                    if (comm.IsOpen == true)//添加事件注册                       
                        comm.DataReceived += comm_DataReceived;
                    Com_Write("at+mac\r\n");//获取mac
                    mac = true;
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    comm = new SerialPort();
                    //初始化SerialPort对象
                    comm.NewLine = "\r\n";
                    //添加事件注册
                    comm.DataReceived += comm_DataReceived;
                    //显示异常信息给客户。                  
                    MessageBox.Show(ex.Message);
                }
            }
            buttonOpenBaudrate.Text = comm.IsOpen ? "Close" : "Open";
        }
        //接收数据转换成可见的字符显示
        string buftostring()
        {
            string bufstring="";
            for (int i = 0; i < n; i++)
            {
                if((buf[i]>=0)&&(buf[i]<10))
                    buf[i]=(byte)(buf[i]+0x30);
            }
            bufstring = Encoding.GetEncoding("gb2312").GetString(buf);
            return bufstring;
        }
        //错误返回，红色字体显示
        string errorbuftostring()
        {
            string bufstring = "";
            for (int i = 0; i < n; i++)
            {
                if ((buf[i] >= 240))
                    bufstring += "-" + Convert.ToString((256 - buf[i]));
                else
                    bufstring += ((char)buf[i]).ToString();
            }
            return bufstring;
        }

        string ipconfig(byte[] buf,int num)
        {
            string ipconfig = "";
            string mac = "";
            string ip = "";
            string mask = "";
            string gateway = "";
            string dns1 = "";
            string dns2 = "";

            mac = "mac="+buf[2].ToString("X") + ":" + buf[3].ToString("X") + ":"
                  + buf[4].ToString("X") + ":" + buf[5].ToString("X") + ":"
                  + buf[6].ToString("X") + ":" + buf[7].ToString("X");

            dns2 = "dns2=" + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString();
            dns1 = "dns1=" + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString();
            gateway = "gw=" + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString();
            mask = "mask=" + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString();
            ip = "addr=" + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString() + "." + buf[num--].ToString();

            ipconfig = "OK\r\n" + mac + "\r\n" + ip + "\r\n" + mask + "\r\n" +
                        gateway + "\r\n" + dns1 + "\r\n" + dns2 + "\r\n";
            
            return ipconfig;
        }
        //串口发送，蓝色字体显示
        void Com_Write(string data)
        {
            if (comm.IsOpen)
            {
                comm.Write(data);
                this.textBoxPlay.Select(textBoxPlay.TextLength,0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Blue;               
                this.textBoxPlay.AppendText("CMD => " + data);
            }
            else
            {
                MsgBox.Show("Please open serial port !!!");
            }
        }
        //串口接收数据信息
        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(10);//延时，以确保数据完全接收
            if (get_scan || getconfig || getweb)
                System.Threading.Thread.Sleep(200);//延时，以确保数据完全接收
            if(dhcp)
                System.Threading.Thread.Sleep(1000);//延时，以确保数据完全接收
            n = comm.BytesToRead;             //先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            comm.Read(buf, length, n);            //读取缓冲数据
            //因为要访问ui资源，所以需要使用invoke方式同步ui。
            this.Invoke((EventHandler)(delegate
            {                
                bufstring = Encoding.GetEncoding("gb2312").GetString(buf);
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Green;

                if ((n == 19) && (bufstring.Contains("Welcome to RAK")))//复位模块
                {
                    this.dataGridView1.Rows.Clear();
                    this.dataGridView1.Enabled = false;

                    radioButtonStation.Checked = true;
                    radioButtonAP.Checked = false;
                    radioButtonAdhoc.Checked = false;
                    textBoxSSID.Text = "";
                    textBoxSSID.Enabled = false;
                    textBoxKey.Text = "";
                    textBoxKey.Enabled = false;
                    comboBoxchannel.Text = "";
                    comboBoxchannel.Enabled = false;
                    buttonConnect.Text = "Connect";
                    buttonConnect.Enabled = false;
                    radioButtonManual.Checked = false;
                    radioButtonManual.Enabled = false;
                    radioButtonDHCP.Checked = true;
                    buttonOK.Enabled = false;
                    panel8.Enabled = false;
                    panel9.Enabled = false;
                    panel10.Enabled = false;
                    panel11.Enabled = false;
                    panel12.Enabled = false;
                    textBoxsrcip1.Text = "";
                    textBoxsrcip2.Text = "";
                    textBoxsrcip3.Text = "";
                    textBoxsrcip4.Text = "";
                    textBoxmaskip1.Text = "";
                    textBoxmaskip2.Text = "";
                    textBoxmaskip3.Text = "";
                    textBoxmaskip4.Text = "";
                    textBoxgwip1.Text = "";
                    textBoxgwip2.Text = "";
                    textBoxgwip3.Text = "";
                    textBoxgwip4.Text = "";
                    textBoxdns1.Text = "";
                    textBoxdns2.Text = "";
                    textBoxdns3.Text = "";
                    textBoxdns4.Text = "";
                    textBoxdns5.Text = "";
                    textBoxdns6.Text = "";
                    textBoxdns7.Text = "";
                    textBoxdns8.Text = "";
                    comboBoxSocket.Text = "";
                    textBoxdestIP.Text = "";
                    textBoxDestPort.Text = "";
                    textBoxLocalPort.Text = "";
                    buttonSetUp.Enabled = false;
                    textBoxSenddata.Text = "";
                    comboBoxSendID.Text = "0";
                    comboBoxClsID.Text = "0";
                    buttonSend.Enabled = false;
                    buttonCloseSocket.Enabled = false;
                    textBoxConfigData.Text = "";
                    this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                    this.textBoxPlay.SelectionColor = Color.Green;
                    this.textBoxPlay.AppendText(CMD_Return + " => " + bufstring);
                }
                if(send)
                {
                    send = false;
                    if (buf[0] != 0)
                    {
                        if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                        {
                            this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                            this.textBoxPlay.SelectionColor = Color.Green;
                            this.textBoxPlay.AppendText(CMD_Return + " => OK\r\n");
                        }
                        else
                        {
                            this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                            this.textBoxPlay.SelectionColor = Color.Red;
                            this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                        }
                    }
                }
                if (bufstring.Contains("recv_data"))
                {
                    this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                    this.textBoxPlay.SelectionColor = Color.Tomato;
                    if (buf[13]==0x80)//TCP Client连接上模块
                    {
                        string ip = buf[20].ToString() + "." + buf[19].ToString() + "."
                                  + buf[18].ToString() + "." + buf[17].ToString();
                        string port = Convert.ToString(buf[16] * 256 + buf[15]);
                        string connect_tcp = "at+recv_data=open,"+ Convert.ToString(buf[14])+","
                                           + port + ","+ ip + "\r\n";
                        this.textBoxPlay.AppendText("Recv Data => " + connect_tcp);
                        destport[buf[14]] = port+"";//记录Socket ID为buf[14]对应的目标端口
                        destip[buf[14]] = ip+"";//记录Socket ID为buf[14]对应的目标IP
                    }
                    else if (buf[13] == 0x81)//TCP Client断开了模块
                    {
                        string disconnect_tcp = "at+recv_data=close," + Convert.ToString(buf[14]) + ","
                                           + Convert.ToString(buf[16] * 256 + buf[15]) + ","
                                           + buf[17].ToString() + "." + buf[18].ToString() + "."
                                           + buf[19].ToString() + "." + buf[20].ToString() + "\r\n";
                        this.textBoxPlay.AppendText("Recv Data => " + disconnect_tcp);
                        destport[buf[14]] = "0";//记录Socket ID为buf[14]对应的目标端口
                        destip[buf[14]] = "0";//记录Socket ID为buf[14]对应的目标IP
                    }
                    else
                    {
                        if ((buf[13] < 8)&&(n>22)&&(buf[0]!=0))//端口标识符小于8
                        {
                            string port = Convert.ToString(buf[15] * 256 + buf[14]);
                            string ip = buf[19].ToString() + "." + buf[18].ToString() + "."
                                               + buf[17].ToString() + "." + buf[16].ToString();
                            string recv_data = "at+recv_data=" + Convert.ToString(buf[13]) + ","
                                               + port + "," + ip + ","
                                               + Convert.ToString(buf[21] * 256 + buf[20]) + ",";
                            destport[buf[13]] = port + "";//记录Socket ID为buf[0x0d]对应的目标端口
                            destip[buf[13]] = ip + "";//记录Socket ID为buf[0x0d]对应的目标IP

                            recv_data +=  Encoding.GetEncoding("gb2312").GetString(buf, 22, n - 22);
                            this.textBoxPlay.AppendText("Recv Data => " + recv_data);
                        }
                    }
                }
                //等待查询版本号返回
                if (version)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        textBoxVersion.Text = Encoding.GetEncoding("gb2312").GetString(buf,2,n-2);
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + bufstring);
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    
                    version = false;
                    
                }
                //等待查询MAC返回
                if (mac)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))//获取mac成功
                    {
                        string macstring = buf[2].ToString("X") + ":"+ buf[3].ToString("X") + ":"
                                         + buf[4].ToString("X") + ":"+ buf[5].ToString("X") + ":"
                                         + buf[6].ToString("X") + ":"+ buf[7].ToString("X");
                        textBoxMAC.Text = macstring;
                        mac = false;
                        version = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => OK" + macstring +"\r\n");
                        Com_Write("at+version\r\n");//获取固件版本号                       
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                }
                //等待获取扫描所有网络返回
                if (get_scan)
                {                    
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        ScanData_Received(); //Analytical data 
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => OK\r\n");                                             
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    get_scan = false;
                }
                //等待扫描所有网络返回
                if (scan)
                {
                    dataGridView1.Enabled = true;
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + buftostring());
                        scan_num = buf[2];//记录扫描到网络个数
                        string sendata = "at+get_scan=";
                        sendata += ((char)buf[2]).ToString();
                        sendata += "\r\n";
                        Com_Write(sendata);
                        get_scan = true;                       
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    
                    scan = false;
                }
                //等待发送信道返回
                if (channel)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + bufstring);
                        string sendata;
                        if (radioButtonAP.Checked == true)//创建AP
                        {
                            apconnect = true;
                            sendata = "at+ap=";
                            sendata = sendata + textBoxSSID.Text + "\r\n";
                            Com_Write(sendata);
                        }
                        if (radioButtonAdhoc.Checked == true)//创建Adhoc
                        {
                            adhocconnect = true;
                            sendata = "at+adhoc=";
                            sendata = sendata + textBoxSSID.Text + "\r\n";
                            Com_Write(sendata);
                        }
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    channel = false;
                }
                //等待发送密码返回
                if (psk)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))//打开ascii成功
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));

                        string sendata;

                        if (radioButtonStation.Checked == true)//创建STA
                        {
                            staconnect = true;
                            sendata = "at+connect=";
                            sendata = sendata + textBoxSSID.Text + "\r\n";
                            Com_Write(sendata);
                        }
                        if ((radioButtonAP.Checked == true)||(radioButtonAdhoc.Checked == true))//创建Adhoc //创建AP
                        {
                            channel = true;
                            sendata = "at+channel=";
                            switch (comboBoxchannel.Text)
                            {
                                case "channel 1":
                                    sendata += "1";
                                    break;
                                case "channel 2":
                                    sendata += "2";
                                    break;
                                case "channel 3":
                                    sendata += "3";
                                    break;
                                case "channel 4":
                                    sendata += "4";
                                    break;
                                case "channel 5":
                                    sendata += "5";
                                    break;
                                case "channel 6":
                                    sendata += "6";
                                    break;
                                case "channel 7":
                                    sendata += "7";
                                    break;
                                case "channel 8":
                                    sendata += "8";
                                    break;
                                case "channel 9":
                                    sendata += "9";
                                    break;
                                case "channel 10":
                                    sendata += "10";
                                    break;
                                case "channel 11":
                                    sendata += "11";
                                    break;
                                default:
                                    sendata += "0";
                                    break;
                            }
                            sendata = sendata + "\r\n";                          
                            Com_Write(sendata);
                        }
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                   
                    psk = false;
                }
                //等待连接网络返回
                if (staconnect)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        buttonConnect.Text = "Disconnect";
                        connect = false;
                        radioButtonDHCP.Enabled = true;
                        radioButtonManual.Enabled = true;
                        buttonOK.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    staconnect = false;
                }
                if (dhcpsevse)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        //this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                        this.textBoxPlay.AppendText(CMD_Return + " => OK\r\n");
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    dhcpsevse = false;
                }
                //等待创建AP返回
                if (apconnect)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        radioButtonManual.Enabled = true;
                        radioButtonDHCP.Enabled = false;
                        buttonOK.Enabled = true;
                        string sendata;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));                       
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    apconnect = false;
                }
                //等待创建Adhoc返回
                if (adhocconnect)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        radioButtonManual.Enabled = true;
                        radioButtonDHCP.Enabled = false;
                        buttonOK.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    } 
                    adhocconnect = false;
                }
                //等待断开网络返回
                if(disc)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        buttonConnect.Text = "Connect";
                        connect = true;
                        radioButtonManual.Enabled = false;
                        buttonOK.Enabled = false;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    disc = false;
                }
                //等待DHCP返回
                if (dhcp)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        if (n >= 30)
                        {
                            int dhcpnum = 0;
                            if (n == 34)
                                dhcpnum = 31;
                            if (n == 30)
                                dhcpnum = 27;

                            textBoxdns5.Text = buf[dhcpnum--].ToString();
                            textBoxdns6.Text = buf[dhcpnum--].ToString();
                            textBoxdns7.Text = buf[dhcpnum--].ToString();
                            textBoxdns8.Text = buf[dhcpnum--].ToString();

                            textBoxdns1.Text = buf[dhcpnum--].ToString();
                            textBoxdns2.Text = buf[dhcpnum--].ToString();
                            textBoxdns3.Text = buf[dhcpnum--].ToString();
                            textBoxdns4.Text = buf[dhcpnum--].ToString();

                            textBoxgwip1.Text = buf[dhcpnum--].ToString();
                            textBoxgwip2.Text = buf[dhcpnum--].ToString();
                            textBoxgwip3.Text = buf[dhcpnum--].ToString();
                            textBoxgwip4.Text = buf[dhcpnum--].ToString();

                            textBoxmaskip1.Text = buf[dhcpnum--].ToString();
                            textBoxmaskip2.Text = buf[dhcpnum--].ToString();
                            textBoxmaskip3.Text = buf[dhcpnum--].ToString();
                            textBoxmaskip4.Text = buf[dhcpnum--].ToString();

                            textBoxsrcip1.Text = buf[dhcpnum--].ToString();
                            textBoxsrcip2.Text = buf[dhcpnum--].ToString();
                            textBoxsrcip3.Text = buf[dhcpnum--].ToString();
                            textBoxsrcip4.Text = buf[dhcpnum--].ToString();

                            this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                            this.textBoxPlay.SelectionColor = Color.Green;
                            this.textBoxPlay.AppendText(CMD_Return + " => " + "OK\r\n");
                        }
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    buttonSetUp.Enabled = true;
                    dhcp = false;
                }
                //等待静态设置IP返回
                if (manual)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        buttonSetUp.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                        if (radioButtonAP.Checked == true)//模块作为AP的时候，才有at+ipdhcp=1
                        {
                            string sendata = "at+ipdhcp=1\r\n";//开启DHCP sever
                            Com_Write(sendata);
                            dhcpsevse = true;
                        }
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    manual = false;
                }
                //等待建立ltcp返回
                if (ltcp)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        buttonSend.Enabled = true;
                        buttonCloseSocket.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        if(buf[2]==10)
                            this.textBoxPlay.AppendText(CMD_Return + " => OK10\r\n");
                        else if (buf[2] == 11)
                            this.textBoxPlay.AppendText(CMD_Return + " => OK11\r\n");
                        else
                            this.textBoxPlay.AppendText(CMD_Return + " => " + buftostring());
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    ltcp = false;
                }
                //等待建立tcp返回
                if (tcp)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        destport[buf[2]] = "0";//记录Socket ID为buf[2]对应的目标端口
                        destip[buf[2]] = "0";//记录Socket ID为buf[2]对应的目标IP
                        buttonSend.Enabled = true;
                        buttonCloseSocket.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + buftostring());
                        
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    tcp = false;
                }
                //等待建立ludp返回
                if (ludp)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        buttonSend.Enabled = true;
                        buttonCloseSocket.Enabled = true;
                        //destport[buf[2]] = textBoxDestPort.Text;//记录Socket ID为buf[2]对应的目标端口
                        //destip[buf[2]] = textBoxdestIP.Text;//记录Socket ID为buf[2]对应的目标IP
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + buftostring());                        
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    ludp = false;
                }
                //等待建立udp返回
                if (udp)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        destport[buf[2]] = textBoxDestPort.Text;//记录Socket ID为buf[2]对应的目标端口
                        destip[buf[2]] = textBoxdestIP.Text;//记录Socket ID为buf[2]对应的目标IP
                        buttonSend.Enabled = true;
                        buttonCloseSocket.Enabled = true;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + buftostring());                       
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    udp = false;
                }
                //等待关闭socket 端口返回
                if (cls)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        if (Convert.ToInt16(comboBoxClsID.Text) < 8)
                        {
                            destport[Convert.ToInt16(comboBoxClsID.Text)] = "0";//清除comboBoxClsID选中的目标端口
                            destip[Convert.ToInt16(comboBoxClsID.Text)] = "0";//清除comboBoxClsID选中的目标IP
                        }
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    cls = false;
                }
                //等待功耗设置返回
                if (powermode)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    powermode = false;
                }
                //等待uart设置返回
                if (uartconfig)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    uartconfig = false;
                }
                //等待easyconfig设置返回
                if (easyconfig)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + ipconfig(buf,27));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    easyconfig = false;
                }
                //等待wps设置返回
                if (wps)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + ipconfig(buf, 27));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    wps = false;
                }
                //等待自动联网设置返回
                if (autoconnect)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.Select(textBoxPlay.TextLength,0);//将光标始终指向最末尾this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + ipconfig(buf, 27));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    autoconnect = false;
                }
                //等待功耗设置返回
                if (webserver)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    webserver = false;
                }
                //等待获取网络参数返回
                if (getconfig)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        string net_param="";
                        net_param = Decode_netweb_config(buf);
                        textBoxConfigData.Text = net_param;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => OK\r\n");
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    getconfig = false;
                }
                //等待设置网络参数返回
                if (setconfig)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    setconfig = false;
                }
                //等待获取web参数返回
                if (getweb)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        string net_param = "";
                        net_param = Decode_netweb_config(buf);
                        textBoxConfigData.Text = net_param;
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => OK\r\n");
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    getweb = false;
                }
                //等待设置网络参数返回
                if (setweb)
                {
                    if ((buf[0] == 0x4f) && (buf[1] == 0x4b))
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Green;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + Encoding.GetEncoding("gb2312").GetString(buf));
                    }
                    else
                    {
                        this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                        this.textBoxPlay.SelectionColor = Color.Red;
                        this.textBoxPlay.AppendText(CMD_Return + " => " + errorbuftostring());
                    }
                    setweb = false;
                }

                //将所有变量改为false
                mac = false;scan = false;psk = false;disc = false;dhcp = false;manual = false;ltcp = false;
                tcp = false;ludp = false;udp = false;send = false;cls = false;getconfig = false;setconfig = false;
                getweb = false;setweb = false;powermode = false;uartconfig = false;easyconfig = false;wps = false;
                autoconnect = false;webserver = false;

                Array.Clear(buf, 0, 5000);//清空buf              
            }));
        }
        //扫描周围网络
        private void buttonScan_Click(object sender, EventArgs e)
        {
            if (comm.IsOpen)
            {
                scan = true;
                string sendata = "at+scan=0\r\n";
                Com_Write(sendata);
            }
        }

        //解析网络加密类型
        string Security_Decode(int sec)
        {
            string WPA2 = "WPA2";
            bool wpa2 = false;
            string WPA = "WPA";
            bool wpa = false;
            string WEP = "WEP";
            bool wep = false;
            string X = "802.1X";
            bool x = false;
            string PSK = "PSK";
            bool psk = false;
            string bWEP = "WEP";
            bool bwep = false;
            string TKIP = "TKIP";
            bool tkip = false;
            string CCMP = "CCMP";
            bool ccmp = false;
            string SECURITY_BACK = "";
            if (sec== 0)
            {
                SECURITY_BACK = "NONE";
            }
            else
            {
                if ((sec & 0x80) != 0)
                {
                    wpa2 = true;
                }
                if ((sec & 0x40) != 0)
                {
                    wpa = true;
                }
                if ((sec & 0x20) != 0)
                {
                    wep = true;
                }
                if ((sec & 0x10) != 0)
                {
                    x = true;
                }
                if ((sec & 0x08) != 0)
                {
                    psk = true;
                }
                if ((sec & 0x04) != 0)
                {
                    bwep = true;
                }
                if ((sec & 0x02) != 0)
                {
                    tkip = true;
                }
                if ((sec & 0x01) != 0)
                {
                    ccmp = true;
                }
                if (wpa2)
                {
                    SECURITY_BACK = WPA2;
                    if (psk)
                    {
                        SECURITY_BACK += "-" + PSK;
                        if (bwep)
                            SECURITY_BACK += "-" + bWEP;
                        if (tkip)
                            SECURITY_BACK += "-" + TKIP;
                        if (ccmp)
                            SECURITY_BACK += "-" + CCMP;
                    }
                    if (x)
                    {
                        SECURITY_BACK += "-" + X;
                        if (bwep)
                            SECURITY_BACK += "-" + bWEP;
                        if (tkip)
                            SECURITY_BACK += "-" + TKIP;
                        if (ccmp)
                            SECURITY_BACK += "-" + CCMP;
                    }
                }
                if (wpa)
                {
                    if (wpa2)
                        SECURITY_BACK += "/" + SECURITY_BACK.Remove(3, 1);
                    else
                    {
                        SECURITY_BACK = WPA;
                        if (psk)
                        {
                            SECURITY_BACK += "-" + PSK;
                            if (bwep)
                                SECURITY_BACK += "-" + bWEP;
                            if (tkip)
                                SECURITY_BACK += "-" + TKIP;
                            if (ccmp)
                                SECURITY_BACK += "-" + CCMP;
                        }
                        if (x)
                        {
                            SECURITY_BACK += "-" + X;
                            if (bwep)
                                SECURITY_BACK += "-" + bWEP;
                            if (tkip)
                                SECURITY_BACK += "-" + TKIP;
                            if (ccmp)
                                SECURITY_BACK += "-" + CCMP;
                        }
                    }
                }
                if (wep)
                {
                    SECURITY_BACK = WEP;
                    if (psk)
                    {
                        SECURITY_BACK += "-" + PSK;
                        if (bwep)
                            SECURITY_BACK += "-" + bWEP;
                        if (tkip)
                            SECURITY_BACK += "-" + TKIP;
                        if (ccmp)
                            SECURITY_BACK += "-" + CCMP;
                    }
                    if (x)
                    {
                        SECURITY_BACK += "-" + X;
                        if (bwep)
                            SECURITY_BACK += "-" + bWEP;
                        if (tkip)
                            SECURITY_BACK += "-" + TKIP;
                        if (ccmp)
                            SECURITY_BACK += "-" + CCMP;
                    }
                }
            }
           
            return SECURITY_BACK;
        }

        //构成网络参数和web参数，保存到模块
        byte[] Encode_netweb_config(string bufstring)
        {
            byte[] data = new byte[200];
            string flag1 = "=";
            string flag2 = "\r\n";
            int len=bufstring.Length;
            bufstring.Trim();
            //bufstring.Replace(" ","");
            int index = bufstring.IndexOf(flag1);
            int index2 = bufstring.IndexOf(flag2);
            string key_name = "";
            string key_data = "";
            bool error = false;//记录输入是否有错

            if (index < index2)//"="在前，换行在后
            {
                bufstring = "\r\n" + bufstring;//添加一个换行符
            }
            index = 0; index2 = 0;
            int index3 = 0;//索引关键数据
            while(true)
            {
                index2 = bufstring.IndexOf(flag2, index);//"\r\n"
                if (index2 == -1)
                    break;
                index2 += 2;
                index = bufstring.IndexOf(flag1, index2);// "="
                if (index == -1)
                    break;
                key_name = bufstring.Substring(index2, index - index2);//获取到关键字
                index += 1;                
                index3 = bufstring.IndexOf(flag2, index);//"\r\n"
                if (index3 == -1)
                    break;
                key_data = bufstring.Substring(index, index3 - index);//获取到关键数据

                switch (key_name)
                {
                    case "wifi_mode":
                        {
                            if (key_data == "STA")
                                data[4] = 0;
                            else if (key_data == "AP")
                                data[4] = 1;
                            else if (key_data == "ADHOC")
                                data[4] = 2;
                            else
                            {
                                error = true;
                                MsgBox.Show("Input ERROR,only STA、AP、ADHOC are allowed  !!!");
                            }
                            break;
                        }
                    case "rak_channel":
                        {
                            data[5] = Convert.ToByte(key_data);
                            if (((data[5] <= 0)) && (data[5] > 12))
                            {
                                error = true;
                                MsgBox.Show("The channel should be 0 to 11 !!!");                               
                            }
                            break;
                        }
                    case "rak_sec_en":
                        {
                            data[6] = Convert.ToByte(key_data);
                            if ((data[6] != 0) && (data[6] != 1))
                            {
                                error = true;
                                MsgBox.Show("The rak_sec_en should be 0 or 1 !!!"); 
                            }
                            break;
                        }
                    case "rak_dhcp_en":
                        {
                            data[7] = Convert.ToByte(key_data);
                            if ((data[7] != 0) && (data[7] != 1))
                            {
                                error = true;
                                MsgBox.Show("The rak_dhcp_en should be 0 or 1 !!!");
                            }
                            break;
                        }
                    case "rak_ssid":
                        {
                            byte[] ssid = new byte[33];
                            if (key_data=="")
                            {
                                error = true;
                                MsgBox.Show("The ssid can't be null !!!");
                            }
                            else if (key_data.Length <= 33)
                                ssid = Encoding.GetEncoding("gb2312").GetBytes(key_data);
                            else
                            {
                                error = true;
                                MsgBox.Show("The ssid length can't more than 33 bytes !!!");
                            }
                            Array.Copy(ssid, 0, data, 8, ssid.Length);   
                            break;
                        }
                    case "rak_psk":
                        {
                            byte[] psk = new byte[65];
                            if (key_data == "")
                            {
                            }
                            else if ((key_data.Length <8)||(key_data.Length >65))
                            {
                                error = true;
                                MsgBox.Show("The ssid length can't lee than 8 or more than 65 bytes !!!");
                            }                               
                            else
                            {
                                psk = Encoding.GetEncoding("gb2312").GetBytes(key_data);
                            }
                            Array.Copy(psk, 0, data, 41, psk.Length); 
                            break;
                        }
                    case "rak_ipaddr":
                        {
                            key_data += "\r\n";
                            int k = key_data.IndexOf(".");
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input IP ERROR !!!");
                                break;
                            }
                            string x=key_data.Substring(0, k);
                            data[111] = Convert.ToByte(x);
                            k += 1;

                            int k1 = key_data.IndexOf(".", k);
                            if (k1 == -1)
                            {
                                error = true;
                                MsgBox.Show("Input IP ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k, k1-k);
                            data[110] = Convert.ToByte(x);
                            k1 += 1;

                            k = key_data.IndexOf(".", k1);
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input IP ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k1, k-k1);
                            data[109] = Convert.ToByte(x);
                            k += 1;

                            k1 = key_data.IndexOf("\r\n", k);
                            x = key_data.Substring(k, k1 - k);
                            data[108] = Convert.ToByte(x);
                            break;
                        }
                    case "rak_netmask":
                        {
                            key_data += "\r\n";
                            int k = key_data.IndexOf(".");
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Mask ERROR !!!");
                                break;
                            }
                            string x = key_data.Substring(0, k);
                            data[115] = Convert.ToByte(x);
                            k += 1;

                            int k1 = key_data.IndexOf(".", k);
                            if (k1 == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Mask ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k, k1 - k);
                            data[114] = Convert.ToByte(x);
                            k1 += 1;

                            k = key_data.IndexOf(".", k1);
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Mask ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k1, k - k1);
                            data[113] = Convert.ToByte(x);
                            k += 1;

                            k1 = key_data.IndexOf("\r\n", k);
                            x = key_data.Substring(k, k1 - k);
                            data[112] = Convert.ToByte(x);
                            break;
                        }
                    case "rak_gw":
                        {
                            key_data += "\r\n";
                            int k = key_data.IndexOf(".");
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Gateway ERROR !!!");
                                break;
                            }
                            string x = key_data.Substring(0, k);
                            data[119] = Convert.ToByte(x);
                            k += 1;

                            int k1 = key_data.IndexOf(".", k);
                            if (k1 == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Gateway ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k, k1 - k);
                            data[118] = Convert.ToByte(x);
                            k1 += 1;

                            k = key_data.IndexOf(".", k1);
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input Gateway ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k1, k - k1);
                            data[117] = Convert.ToByte(x);
                            k += 1;

                            k1 = key_data.IndexOf("\r\n", k);
                            x = key_data.Substring(k, k1 - k);
                            data[116] = Convert.ToByte(x);
                            break;
                        }
                    case "rak_dns1":
                        {
                            key_data += "\r\n";
                            int k = key_data.IndexOf(".");
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS1 ERROR !!!");
                                break;
                            }
                            string x = key_data.Substring(0, k);
                            data[123] = Convert.ToByte(x);
                            k += 1;

                            int k1 = key_data.IndexOf(".", k);
                            if (k1 == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS1 ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k, k1 - k);
                            data[122] = Convert.ToByte(x);
                            k1 += 1;

                            k = key_data.IndexOf(".", k1);
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS1 ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k1, k - k1);
                            data[121] = Convert.ToByte(x);
                            k += 1;

                            k1 = key_data.IndexOf("\r\n", k);
                            x = key_data.Substring(k, k1 - k);
                            data[120] = Convert.ToByte(x);
                            break;
                        }
                    case "rak_dns2":
                        {
                            key_data += "\r\n";
                            int k = key_data.IndexOf(".");
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS2 ERROR !!!");
                                break;
                            }
                            string x = key_data.Substring(0, k);
                            data[127] = Convert.ToByte(x);
                            k += 1;

                            int k1 = key_data.IndexOf(".", k);
                            if (k1 == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS2 ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k, k1 - k);
                            data[126] = Convert.ToByte(x);
                            k1 += 1;

                            k = key_data.IndexOf(".", k1);
                            if (k == -1)
                            {
                                error = true;
                                MsgBox.Show("Input DNS2 ERROR !!!");
                                break;
                            }
                            x = key_data.Substring(k1, k - k1);
                            data[125] = Convert.ToByte(x);
                            k += 1;

                            k1 = key_data.IndexOf("\r\n", k);
                            x = key_data.Substring(k, k1 - k);
                            data[124] = Convert.ToByte(x);
                            break;
                        }
                    case "rak_ap_hidden":
                        {
                            data[128] = Convert.ToByte(key_data);
                            if ((data[128] != 0) && (data[128] != 1))
                            {
                                error = true;
                                MsgBox.Show("The rak_ap_hidden should be 0 or 1 !!!");
                            }
                            break;
                        }
/*                    case "rak_ap_country_en":
                        {
                            data[127] = Convert.ToByte(key_data);
                            if ((data[127] != 0) && (data[127] != 1))
                            {
                                error = true;
                                MsgBox.Show("The rak_ap_country_en should be 0 or 1 !!!");
                            }
                            break;
                        }
*/                    case "rak_ap_country":
                        {
                            byte[] country = new byte[3];
                            if (key_data == "")
                            {
                                error = true;
                                MsgBox.Show("The ap_country can't be null !!!");
                            }
                            else if (key_data.Length <= 3)
                                country = Encoding.GetEncoding("gb2312").GetBytes(key_data);
                            else
                            {
                                error = true;
                                MsgBox.Show("The ap_country length can't more than 3 bytes !!!");
                            }
                            Array.Copy(country, 0, data, 129, country.Length); 
                            break;
                        }
                      case "rak_user_name":
                        {
                            byte[] user_name = new byte[17];
                            if (key_data == "")
                            {
                                error = true;
                                MsgBox.Show("The user_name can't be null !!!");
                            }
                            else if (key_data.Length <= 17)
                                user_name = Encoding.GetEncoding("gb2312").GetBytes(key_data);
                            else
                            {
                                error = true;
                                MsgBox.Show("The user_name length can't more than 17 bytes !!!");
                            }
                            Array.Copy(user_name, 0, data, 132, user_name.Length);
                            break;
                        }
                      case "rak_user_psk":
                        {
                            byte[] user_psk = new byte[17];
                            if (key_data == "")
                            {
                                error = true;
                                MsgBox.Show("The user_psk can't be null !!!");
                            }
                            else if (key_data.Length <= 17)
                                user_psk = Encoding.GetEncoding("gb2312").GetBytes(key_data);
                            else
                            {
                                error = true;
                                MsgBox.Show("The user_psk length can't more than 17 bytes !!!");
                            }
                            Array.Copy(user_psk, 0, data, 149, user_psk.Length);
                            break;
                        }
                }
            }
            
            return data;
        }

        //解析获取到的网络参数和web参数
        string Decode_netweb_config(byte[] config_buf)
        {
            string netparam = "";
            if (config_buf[6] == 0)
                netparam += "wifi_mode=STA\r\n";
            else if (config_buf[6] == 1)
                netparam += "wifi_mode=AP\r\n";
            else if (config_buf[6] == 2)
                netparam += "wifi_mode=ADHOC\r\n";

            netparam += "rak_channel=" + Convert.ToString(config_buf[7]) + "\r\n";
            netparam += "rak_sec_en=" + Convert.ToString(config_buf[8]) + "\r\n";
            netparam += "rak_dhcp_en=" + Convert.ToString(config_buf[9]) + "\r\n";
            string ssid = "";
            ssid = Encoding.GetEncoding("gb2312").GetString(config_buf, 10, 33);
            int ssidnum = ssid.IndexOf("\0");
            netparam += "rak_ssid=" + Encoding.GetEncoding("gb2312").GetString(config_buf, 10, ssidnum) + "\r\n";

            string psk = "";
            psk = Encoding.GetEncoding("gb2312").GetString(config_buf, 43, 65);
            int psknum = psk.IndexOf("\0");
            netparam += "rak_psk=" + Encoding.GetEncoding("gb2312").GetString(config_buf, 43, psknum) + "\r\n";
            int netnum = 113;
            netparam += "rak_ipaddr=" + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "."
                        + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "\r\n";
            netnum = 117;
            netparam += "rak_netmask=" + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "."
                        + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "\r\n";
            netnum = 121;
            netparam += "rak_gw=" + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "."
                        + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "\r\n";
            netnum = 125;
            netparam += "rak_dns1=" + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "."
                        + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "\r\n";
            netnum = 129;
            netparam += "rak_dns2=" + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "."
                        + config_buf[netnum--].ToString() + "." + config_buf[netnum--].ToString() + "\r\n";

            netparam += "rak_ap_hidden=" + Convert.ToString(config_buf[130]) + "\r\n";
            //netparam += "rak_ap_country_en=" + Convert.ToString(config_buf[netnum++]) + "\r\n";
            netparam += "rak_ap_country="+Encoding.GetEncoding("gb2312").GetString(config_buf, 131, 3).Replace("\0", "") + "\r\n";

            if (n > 170)
            {
                string user_name = "";
                user_name = Encoding.GetEncoding("gb2312").GetString(config_buf, 134, 17);
                int user_namenum = user_name.IndexOf("\0");
                netparam += "rak_user_name=" + Encoding.GetEncoding("gb2312").GetString(config_buf, 134, user_namenum) + "\r\n";

                string user_psk = "";
                user_psk = Encoding.GetEncoding("gb2312").GetString(config_buf, 151, 17);
                int user_psknum = user_psk.IndexOf("\0");
                netparam += "rak_user_psk=" + Encoding.GetEncoding("gb2312").GetString(config_buf, 151, user_psknum) + "\r\n";
            }
            return netparam;
        }

        //串口接收扫描数据信息
        void ScanData_Received()
        {
            string GroupName; // 组名称
            string CH;        // 通道
            string RSSI;      // 信号强度
            string SECURITY = "";  // 加密方式
            string Mac;       // mac地址
            string scanstring = Encoding.GetEncoding("gb2312").GetString(buf);
            
            int pos = 0;
            int index = scanstring.IndexOf("\r\n");//第一个换行符
            this.dataGridView1.Rows.Clear();

            if (scan_num > 8)
                scan_num = 8;// 限定网络数量8个
            for (int i = 1; i < scan_num; i++)
                dataGridView1.Rows.Add();
            pos = 2;//指向第一个网络
            byte[] ssid = new byte[33];
            byte[] bssid = new byte[6];
            int channel;
            int rssi;
            int security;

            for (int i = 0; i < scan_num; i++)
            {
                Array.Copy(buf, pos, ssid,0 , 33);
                pos += 33;
                Array.Copy(buf, pos, bssid, 0,6);
                pos += 6;
                channel = buf[pos];
                pos += 1;
                rssi = buf[pos];
                pos += 1;
                security = buf[pos];
                pos += 1;

                GroupName = Encoding.GetEncoding("gb2312").GetString(ssid);
                GroupName = GroupName.Replace("\0","");
                Mac = Encoding.GetEncoding("gb2312").GetString(bssid);
                CH = Convert.ToString(channel);
                RSSI = "-" + Convert.ToString((256 - rssi));
                SECURITY = Security_Decode(security);
               
                if (CH!="0")
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = GroupName;
                    this.dataGridView1.Rows[i].Cells[1].Value = CH;
                    this.dataGridView1.Rows[i].Cells[2].Value = RSSI;
                    this.dataGridView1.Rows[i].Cells[3].Value = SECURITY;
                }
            } 
        }
        //点击列表，选择要加入的网络
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxSSID.Text = Convert.ToString(this.dataGridView1.CurrentRow.Cells[0].Value);
            string value = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            switch (value)
            { 
                case "1":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 1");
                    break;
                case "2":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 2");
                    break;
                case "3":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 3");
                    break;
                case "4":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 4");
                    break;
                case "5":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 5");
                    break;
                case "6":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 6");
                    break;
                case "7":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 7");
                    break;
                case "8":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 8");
                    break;
                case "9":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 9");
                    break;
                case "10":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 10");
                    break;
                case "11":
                    comboBoxchannel.SelectedIndex = comboBoxchannel.Items.IndexOf("channel 11");
                    break;                   
            }
            textBoxSSID.Enabled=true;
            textBoxKey.Enabled=true;
            buttonConnect.Enabled = true;
        }
        //连接路由器
        bool connect = true;
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string sendata;

            if (radioButtonStation.Checked==true)//创建STA
            {                
                if (connect)
                {               
                    //Com_Write("at+scan=0," + textBoxSSID.Text + "\r\n");//手动输入
                    if (textBoxKey.Text != "")
                    {
                        if (textBoxKey.Text.ToString().Length < 8)
                        {
                            MsgBox.Show("The password length can't <8 !!!");
                        }
                        else
                        {
                            sendata = "at+psk=";
                            sendata = sendata + textBoxKey.Text + "\r\n";
                            Com_Write(sendata);
                            psk = true;
                        }
                    }
                    else
                    {
                        sendata = "at+connect=";
                        sendata = sendata + textBoxSSID.Text + "\r\n";
                        Com_Write(sendata);
                        staconnect = true;
                    }
                }
                else
                {
                    sendata = "at+disc\r\n";
                    Com_Write(sendata);
                    disc = true;
                }

            }
            else if (radioButtonAP.Checked == true)//创建AP
            {
                if (textBoxKey.Text != "")
                {
                    if (textBoxKey.Text.ToString().Length < 8)
                    {
                        MsgBox.Show("The password length can't <8 !!!");
                    }
                    else
                    {
                        sendata = "at+psk=";
                        sendata = sendata + textBoxKey.Text + "\r\n";
                        Com_Write(sendata);
                        psk = true;
                    }
                }
                else
                {
                    sendata = "at+psk\r\n";
                    Com_Write(sendata);
                    psk = true;
                }
            }
            else if (radioButtonAdhoc.Checked == true)//创建Adhoc
            {
                if (textBoxKey.Text != "")
                {
                    if (textBoxKey.Text != "")
                    {
                        if (textBoxKey.Text.ToString().Length < 8)
                        {
                            MsgBox.Show("The password length can't <8 !!!");
                        }
                        else
                        {
                            sendata = "at+psk=";
                            sendata = sendata + textBoxKey.Text + "\r\n";
                            Com_Write(sendata);
                            psk = true;
                        }
                    }
                    else
                    {
                        sendata = "at+psk=";
                        sendata = sendata + textBoxKey.Text + "\r\n";
                        Com_Write(sendata);
                        psk = true;
                    }
                }
                else
                {
                    sendata = "at+psk\r\n";
                    Com_Write(sendata);
                    psk = true;
                }
            }
        }
        //确定IP设置
        private void buttonOK_Click(object sender, EventArgs e)
        {
            string sendata;

            if (radioButtonDHCP.Checked == true)
            {
                dhcp = true;
                manual = false;

            }
            if (radioButtonManual.Checked == true)
            {
                dhcp = false;
                manual = true;

            }

            if (manual)
            {
                sendata = "at+ipstatic=";
                sendata += textBoxsrcip1.Text.Trim() + "." + textBoxsrcip2.Text.Trim() + "." + textBoxsrcip3.Text.Trim() + "." + textBoxsrcip4.Text.Trim() + ","
                         + textBoxmaskip1.Text.Trim() + "." + textBoxmaskip2.Text.Trim() + "." + textBoxmaskip3.Text.Trim() + "." + textBoxmaskip4.Text.Trim() + ","
                         + textBoxgwip1.Text.Trim() + "." + textBoxgwip2.Text.Trim() + "." + textBoxgwip3.Text.Trim() + "." + textBoxgwip4.Text.Trim() + ","
                         + "0,0\r\n";
                Com_Write(sendata);
            }

            if (dhcp)
            {
                sendata = "at+ipdhcp=0\r\n";
                Com_Write(sendata);
            }
        }
        //设置DHCP
        private void radioButtonDHCP_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Enabled = false;
            panel9.Enabled = false;
            panel10.Enabled = false;
            panel11.Enabled = false;
            panel12.Enabled = false;
        }
        //静态IP设置
        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Enabled = true;
            panel9.Enabled = true;
            panel10.Enabled = true;
            panel11.Enabled = true;
            panel12.Enabled = true;
        }
        //建立TCP/UDP连接
        private void buttonSetUp_Click(object sender, EventArgs e)
        {
            string sendata;
            if (comboBoxSocket.Text == "Tcp Sever")
            {
                if (textBoxLocalPort.Text == "")
                {
                    MsgBox.Show("Local Port can't be null !!!");
                }
                else
                { 
                    sendata = "at+ltcp=";
                    sendata = sendata + textBoxLocalPort.Text + "\r\n";
                    Com_Write(sendata);
                    ltcp = true;
                }                
            }
            if (comboBoxSocket.Text == "Tcp Client")
            {
                bool error = false;
                if (textBoxdestIP.Text == "")
                {
                    error = true;
                    MsgBox.Show("Dest IP can't be null !!!");
                }
                if (textBoxDestPort.Text == "")
                {
                    error = true;
                    MsgBox.Show("Dest Port can't be null !!!");
                }
                if (textBoxLocalPort.Text == "")
                {
                    error = true;
                    MsgBox.Show("Local Port can't be null !!!");
                }
                if (error == false)
                {
                    sendata = "at+tcp=";
                    sendata = sendata + textBoxdestIP.Text + "," + textBoxDestPort.Text + "," + textBoxLocalPort.Text + "\r\n";
                    Com_Write(sendata);
                    tcp = true;
                }
            }
            if (comboBoxSocket.Text == "Udp Sever")
            {
                if (textBoxLocalPort.Text == "")
                {
                    MsgBox.Show("Local Port can't be null !!!");
                }
                else
                {
                    sendata = "at+ludp=";
                    sendata = sendata + textBoxLocalPort.Text + "\r\n";
                    Com_Write(sendata);
                    ludp = true;
                }
            }
            if (comboBoxSocket.Text == "Udp Client")
            {
                bool error = false;
                if (textBoxdestIP.Text == "")
                {
                    error = true;
                    MsgBox.Show("Dest IP can't be null !!!");
                }
                if (textBoxDestPort.Text == "")
                {
                    error = true;
                    MsgBox.Show("Dest Port can't be null !!!");
                }
                if (textBoxLocalPort.Text == "")
                {
                    error = true;
                    MsgBox.Show("Local Port can't be null !!!");
                }
                if (error == false)
                {
                    sendata = "at+udp=";
                    sendata = sendata + textBoxdestIP.Text + "," + textBoxDestPort.Text + "," + textBoxLocalPort.Text + "\r\n";
                    Com_Write(sendata);
                    udp = true;
                }
            }
            //Send data shoule be after the socket have been set up
            textBoxSenddata.Enabled = true;
            buttonSend.Enabled = true;
        }
        //选择Socket类型，Choose Socket Style
        private void comboBoxSocket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBoxSocket.Text == "Tcp Sever") || (comboBoxSocket.Text == "Udp Sever"))
            {
                textBoxdestIP.Enabled = false;
                textBoxDestPort.Enabled = false;
                textBoxLocalPort.Enabled = true;
            }

            if ((comboBoxSocket.Text == "Tcp Client") || (comboBoxSocket.Text == "Udp Client"))
            {
                textBoxdestIP.Enabled = true;
                textBoxDestPort.Enabled = true;
                textBoxLocalPort.Enabled = true;
            }
        }
        //发送数据
        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (comboBoxSendID.Text == "")
            {
                MsgBox.Show("Socket ID can't be null !!!");
            }
            else
            {
                send = true;
                int socket_ID = Convert.ToInt16(comboBoxSendID.Text);
                string sendata;
                sendata = "at+send_data=";
                sendata += comboBoxSendID.Text.ToString() + ",";
                sendata += destport[socket_ID] + "," + destip[socket_ID] + ",";
                sendata = sendata + textBoxSenddata.Text.Length.ToString() + "," + textBoxSenddata.Text + "\r\n";
                Com_Write(sendata);
            }
        }
        //清空显示框
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBoxPlay.Text = "";
        }
        //清空数据框
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBoxSenddata.Text = "";
        }
        //设置模块为STA模式
        private void radioButtonStation_CheckedChanged(object sender, EventArgs e)
        {
            buttonConnect.Text = "Connect";
        } 
        //设置模块为AP模式
        private void radioButtonAP_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSSID.Enabled = true;
            textBoxKey.Enabled = true;
            comboBoxchannel.Enabled = true;
            buttonConnect.Enabled = true;
            buttonConnect.Text = "Create";
        }
        //设置模块为Adhoc模式
        private void radioButtonAdhoc_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSSID.Enabled = true;
            textBoxKey.Enabled = true;
            comboBoxchannel.Enabled = true;
            buttonConnect.Enabled = true;
            buttonConnect.Text = "Create";
        }
        //关闭对应端口的socket
        private void buttonCloseSocket_Click(object sender, EventArgs e)
        {
            if (comboBoxClsID.Text == "")
            {
                MsgBox.Show("Socket ID can't be null !!!");
            }
            else
            {
                cls = true;
                string sendata;
                sendata = "at+cls=";
                sendata = sendata + comboBoxClsID.Text.ToString() + "\r\n";
                Com_Write(sendata);
            }
        }
        //导入配置数据
        private void button_import_cfg_Click(object sender, EventArgs e)
        {
            getconfigdata.Filter = "Config Flie(*.cfg)|*.cfg";
            if (getconfigdata.ShowDialog() == DialogResult.OK)
            {
                string folder = getconfigdata.FileName;
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Brown;
                this.textBoxPlay.AppendText("File In" + " => " + getconfigdata.FileName + "\r\n");//导入的文件路径

                try
                {
                    System.IO.StreamReader objreaddata = new System.IO.StreamReader(folder, UnicodeEncoding.GetEncoding("GB2312"));

                    string cfg_data = objreaddata.ReadToEnd();
                    textBoxConfigData.Text = cfg_data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        //导出配置数据
        private void button_export_cfg_Click(object sender, EventArgs e)
        {
            saveFileDialogcfg.Filter = "Config Flie(*.cfg)|*.cfg";
            saveFileDialogcfg.FileName = "RAK413 Config Data";
            if (saveFileDialogcfg.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialogcfg.FileName;
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Brown;
                this.textBoxPlay.AppendText("File Out" + " => " + fName + "\r\n");//导入的文件路径
                File.WriteAllText(fName, textBoxConfigData.Text, Encoding.Default);
            }
        }
        //获取模块的网络参数
        private void buttonGetConfig_Click(object sender, EventArgs e)
        {
            getconfig = true;
            string sendata;
            sendata = "at+get_storeconfig\r\n";
            Com_Write(sendata);
        }
        //设置模块的网络参数
        private void buttonStoreConfig_Click(object sender, EventArgs e)
        {
            setconfig = true;
            string sendata="";
            if (textBoxConfigData.Text.Trim() == "")
            {
                sendata = "at+storeconfig\r\n";
                Com_Write(sendata);
            }
            else
            {
                byte[] sendbyte = new byte[149];
                Array.Copy(Encoding.GetEncoding("gb2312").GetBytes("at+storeconfig="), 0, sendbyte, 0, 15);
                Array.Copy(Encode_netweb_config(textBoxConfigData.Text.ToString()), 0, sendbyte, 15, 132);
                sendbyte[147] = 0x0d;
                sendbyte[148] = 0x0a;
                comm.Write(sendbyte, 0, 149);
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Blue;
                this.textBoxPlay.AppendText("CMD => " + textBoxConfigData.Text);
            }
            
        }
        //获取模块的Web参数
        private void button1_Click(object sender, EventArgs e)
        {
            getweb = true;
            string sendata;
            sendata = "at+get_webconfig\r\n";
            Com_Write(sendata);
        }
        //设置模块的Web参数
        private void buttonWebConfig_Click(object sender, EventArgs e)
        {
            setweb = true;
            string sendata="";
            if (textBoxConfigData.Text.Trim() == "")
            {
                MsgBox.Show("The Web Config Data can't be null !!!");
            }
            else
            {
                byte[] sendbyte = new byte[184];
                Array.Copy(Encoding.GetEncoding("gb2312").GetBytes("at+web_config="), 0, sendbyte, 0, 14);
                Array.Copy(Encode_netweb_config(textBoxConfigData.Text.ToString()), 0, sendbyte, 14, 166);
                sendbyte[180] = 0x00;
                sendbyte[181] = 0x00;
                sendbyte[182] = 0x0d;
                sendbyte[183] = 0x0a;
                comm.Write(sendbyte, 0, 184);
                this.textBoxPlay.Select(textBoxPlay.TextLength, 0);//将光标始终指向最末尾
                this.textBoxPlay.SelectionColor = Color.Blue;
                this.textBoxPlay.AppendText("CMD => " + textBoxConfigData.Text);
            }
        }
        //清空配置框中的参数
        private void linkLabelConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBoxConfigData.Text = "";
        }
        //功耗设置
        private void buttonSet_Click(object sender, EventArgs e)
        {
            
            if (comboBoxPowermode.Text == "")
            {
                MsgBox.Show("Please choose a power mode !!!");
            }
            else 
            { 
                powermode = true;
                string sendata;
                sendata = "at+pwrmode=" + comboBoxPowermode.Text.ToString().Remove(0,10) + "\r\n";
                Com_Write(sendata);
            }
            
        }
        //串口参数设置
        private void buttonuartSet_Click(object sender, EventArgs e)
        {
            bool error = false;
            if (comboBoxboardrate.Text == "")
            {
                error = true;
                MsgBox.Show("Baudrate can't be null !!!");
            }
            if (comboBoxdata.Text == "")
            {
                error = true;
                MsgBox.Show("Data Bits can't be null !!!");
            }
            if (comboBoxstop.Text == "")
            {
                error = true;
                MsgBox.Show("Stop Bits can't be null !!!");
            }
            if (comboBoxcheck.Text == "")
            {
                error = true;
                MsgBox.Show("Check Sum can't be null !!!");
            }
            if (comboBoxctsrts.Text == "")
            {
                error = true;
                MsgBox.Show("RTS/CTS can't be null !!!");
            }
            if (error == false)
            {
                uartconfig = true;
                string check = "";
                if (comboBoxcheck.Text == "None")
                {
                    check = "0";
                }
                else if (comboBoxcheck.Text == "Odd")
                {
                    check = "1";
                }
                else if (comboBoxcheck.Text == "Even")
                {
                    check = "2";
                }

                string sendata;
                sendata = "at+uartconfig=" + comboBoxboardrate.Text + "," + comboBoxdata.Text + ","
                         + comboBoxstop.Text + "," + check + "," + comboBoxctsrts.Text + "\r\n";
                Com_Write(sendata);
            }
        }
        //Easy_Config
        private void buttoneasyconfig_Click(object sender, EventArgs e)
        {
            easyconfig = true;
            string sendata;
            sendata = "at+easy_config\r\n";
            Com_Write(sendata);
        }
        //WPS
        private void buttonwps_Click(object sender, EventArgs e)
        {
            wps = true;
            string sendata;
            sendata = "at+wps\r\n";
            Com_Write(sendata);

        }
        //自动联网
        private void buttonAutoConnect_Click(object sender, EventArgs e)
        {
            autoconnect = true;
            string sendata;
            sendata = "at+auto_connect\r\n";
            Com_Write(sendata);
        }
        //进入WEB SERVER
        private void buttonStartWeb_Click(object sender, EventArgs e)
        {
            webserver = true;
            string sendata;
            sendata = "at+start_web\r\n";
            Com_Write(sendata);
        }
        //为输入方便，处理焦点切换
        private void textBoxsrcip1_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxsrcip1.Text.Length > 0) && (textBoxsrcip1.Text != " ") && (textBoxsrcip1.Text != ".")) 
            if ((textBoxsrcip1.Text.Length == 3) || (textBoxsrcip1.Text.Contains(" ") == true) || (textBoxsrcip1.Text.Contains(".") == true))
                textBoxsrcip2.Focus();
            string x = textBoxsrcip1.Text.Trim().Replace(".", "");//去掉空格
            textBoxsrcip1.Text = x;
        }

        private void textBoxsrcip2_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxsrcip2.Text.Length > 0) && (textBoxsrcip2.Text != " ") && (textBoxsrcip2.Text != "."))
            if ((textBoxsrcip2.Text.Length == 3) || (textBoxsrcip2.Text.Contains(" ") == true) || (textBoxsrcip2.Text.Contains(".") == true))
                textBoxsrcip3.Focus();
            string x = textBoxsrcip2.Text.Trim().Replace(".", "");//去掉空格
            textBoxsrcip2.Text = x;
        }

        private void textBoxsrcip3_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxsrcip3.Text.Length > 0) && (textBoxsrcip3.Text != " ") && (textBoxsrcip3.Text != "."))
            if ((textBoxsrcip3.Text.Length == 3) || (textBoxsrcip3.Text.Contains(" ") == true) || (textBoxsrcip3.Text.Contains(".") == true))
                textBoxsrcip4.Focus();
            string x = textBoxsrcip3.Text.Trim().Replace(".", "");//去掉空格
            textBoxsrcip3.Text = x;
        }

        private void textBoxsrcip4_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxsrcip4.Text.Length > 0) && (textBoxsrcip4.Text != " ") && (textBoxsrcip4.Text != "."))
            if ((textBoxsrcip4.Text.Length == 3) || (textBoxsrcip4.Text.Contains(" ") == true) || (textBoxsrcip4.Text.Contains(".") == true))
                textBoxmaskip1.Focus();
            string x = textBoxsrcip4.Text.Trim().Replace(".", "");//去掉空格
            textBoxsrcip4.Text = x;            
        }

        private void textBoxmaskip1_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxmaskip1.Text.Length > 0) && (textBoxmaskip1.Text != " ") && (textBoxmaskip1.Text != "."))
            if ((textBoxmaskip1.Text.Length == 3) || (textBoxmaskip1.Text.Contains(" ") == true) || (textBoxmaskip1.Text.Contains(".") == true))
                textBoxmaskip2.Focus();
            string x = textBoxmaskip1.Text.Trim().Replace(".", "");//去掉空格
            textBoxmaskip1.Text = x;
        }

        private void textBoxmaskip2_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxmaskip2.Text.Length > 0) && (textBoxmaskip2.Text != " ") && (textBoxmaskip2.Text != "."))
            if ((textBoxmaskip2.Text.Length == 3) || (textBoxmaskip2.Text.Contains(" ") == true) || (textBoxmaskip2.Text.Contains(".") == true))
                textBoxmaskip3.Focus();
            string x = textBoxmaskip2.Text.Trim().Replace(".", "");//去掉空格
            textBoxmaskip2.Text = x;
        }

        private void textBoxmaskip3_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxmaskip3.Text.Length > 0) && (textBoxmaskip3.Text != " ") && (textBoxmaskip3.Text != "."))
            if ((textBoxmaskip3.Text.Length == 3) || (textBoxmaskip3.Text.Contains(" ") == true) || (textBoxmaskip3.Text.Contains(".") == true))
                textBoxmaskip4.Focus();
            string x = textBoxmaskip3.Text.Trim().Replace(".", "");//去掉空格
            textBoxmaskip3.Text = x;
        }
        private void textBoxmaskip4_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxmaskip4.Text.Length > 0) && (textBoxmaskip4.Text != " ") && (textBoxmaskip4.Text != "."))
            if ((textBoxmaskip4.Text.Length == 3) || (textBoxmaskip4.Text.Contains(" ") == true) || (textBoxmaskip4.Text.Contains(".") == true))
                textBoxgwip1.Focus();
            string x = textBoxmaskip4.Text.Trim().Replace(".", "");//去掉空格
            textBoxmaskip4.Text = x;           
        }

        private void textBoxgwip1_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxgwip1.Text.Length > 0) && (textBoxgwip1.Text != " ") && (textBoxgwip1.Text != "."))
            if ((textBoxgwip1.Text.Length == 3) || (textBoxgwip1.Text.Contains(" ") == true) || (textBoxgwip1.Text.Contains(".") == true))
                textBoxgwip2.Focus();
            string x = textBoxgwip1.Text.Trim().Replace(".", "");//去掉空格
            textBoxgwip1.Text = x;
        }

        private void textBoxgwip2_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxgwip2.Text.Length > 0) && (textBoxgwip2.Text != " ") && (textBoxgwip2.Text != "."))
            if ((textBoxgwip2.Text.Length == 3) || (textBoxgwip2.Text.Contains(" ") == true) || (textBoxgwip2.Text.Contains(".") == true))
                textBoxgwip3.Focus();
            string x = textBoxgwip2.Text.Trim().Replace(".", "");//去掉空格
            textBoxgwip2.Text = x;
        }

        private void textBoxgwip3_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxgwip3.Text.Length > 0) && (textBoxgwip3.Text != " ") && (textBoxgwip3.Text != "."))
            if ((textBoxgwip3.Text.Length == 3) || (textBoxgwip3.Text.Contains(" ") == true) || (textBoxgwip3.Text.Contains(".") == true))
                textBoxgwip4.Focus();
            string x = textBoxgwip3.Text.Trim().Replace(".", "");//去掉空格
            textBoxgwip3.Text = x;
        }
        private void textBoxgwip4_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxgwip4.Text.Length > 0) && (textBoxgwip4.Text != " ") && (textBoxgwip4.Text != "."))
            if ((textBoxgwip4.Text.Length == 3) || (textBoxgwip4.Text.Contains(" ") == true) || (textBoxgwip4.Text.Contains(".") == true))
                textBoxdns1.Focus();
            string x = textBoxgwip4.Text.Trim().Replace(".", "");//去掉空格
            textBoxgwip4.Text = x;            
        }
        private void textBoxdns1_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns1.Text.Length > 0) && (textBoxdns1.Text != " ") && (textBoxdns1.Text != "."))
            if ((textBoxdns1.Text.Length == 3) || (textBoxdns1.Text.Contains(" ") == true) || (textBoxdns1.Text.Contains(".") == true))
                textBoxdns2.Focus();
            string x = textBoxdns1.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns1.Text = x;
        }

        private void textBoxdns2_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns2.Text.Length > 0) && (textBoxdns2.Text != " ") && (textBoxdns2.Text != "."))
            if ((textBoxdns2.Text.Length == 3) || (textBoxdns2.Text.Contains(" ") == true) || (textBoxdns2.Text.Contains(".") == true))
                textBoxdns3.Focus();
            string x = textBoxdns2.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns2.Text = x;
        }

        private void textBoxdns3_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns3.Text.Length > 0) && (textBoxdns3.Text != " ") && (textBoxdns3.Text != "."))
            if ((textBoxdns3.Text.Length == 3) || (textBoxdns3.Text.Contains(" ") == true) || (textBoxdns3.Text.Contains(".") == true))
                textBoxdns4.Focus();
            string x = textBoxdns3.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns3.Text = x;
        }
        private void textBoxdns4_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxdns4.Text.Length > 0) && (textBoxdns4.Text != " ") && (textBoxdns4.Text != "."))
            if ((textBoxdns4.Text.Length == 3) || (textBoxdns4.Text.Contains(" ") == true) || (textBoxdns4.Text.Contains(".") == true))
                textBoxdns5.Focus();
            string x = textBoxdns4.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns4.Text = x;
            
        }
        private void textBoxdns5_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns5.Text.Length > 0) && (textBoxdns5.Text != " ") && (textBoxdns5.Text != "."))
            if ((textBoxdns5.Text.Length == 3) || (textBoxdns5.Text.Contains(" ") == true) || (textBoxdns5.Text.Contains(".") == true))
                textBoxdns6.Focus();
            string x = textBoxdns5.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns5.Text = x;
        }

        private void textBoxdns6_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns6.Text.Length > 0) && (textBoxdns6.Text != " ") && (textBoxdns6.Text != "."))
            if ((textBoxdns6.Text.Length == 3) || (textBoxdns6.Text.Contains(" ") == true) || (textBoxdns6.Text.Contains(".") == true))
                textBoxdns7.Focus();
            string x = textBoxdns6.Text.Trim().Replace(".","");//去掉空格
            textBoxdns6.Text = x;
        }

        private void textBoxdns7_TextChanged(object sender, EventArgs e)
        {
            //输入空格或字符数等于3时，焦点切换到下一个
            if ((textBoxdns7.Text.Length > 0) && (textBoxdns7.Text != " ") && (textBoxdns7.Text != "."))
            if ((textBoxdns7.Text.Length == 3) || (textBoxdns7.Text.Contains(" ") == true) || (textBoxdns7.Text.Contains(".") == true))
                textBoxdns8.Focus();
            string x = textBoxdns7.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns7.Text = x;
        }

        private void textBoxdns8_TextChanged(object sender, EventArgs e)
        {
            string x = textBoxdns8.Text.Trim().Replace(".", "");//去掉空格
            textBoxdns8.Text = x;
        }
        //设置文本框输入类型
        void input_type(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar != 8) && (e.KeyChar != 0x20) && (e.KeyChar != 0x2E))//允许后退、空格、小数点 
            {
                if ((e.KeyChar < 48 || e.KeyChar > 57))//只允许输入数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBoxsrcip1_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字      
        }

        private void textBoxsrcip2_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxsrcip3_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxsrcip4_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxmaskip1_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxmaskip2_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxmaskip3_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxmaskip4_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxgwip1_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxgwip2_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxgwip3_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxgwip4_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns1_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns2_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns3_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns4_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns5_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns6_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns7_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        private void textBoxdns8_KeyPress(object sender, KeyPressEventArgs e)
        {
            input_type(sender, e);  //允许后退、空格、小数点、数字  
        }

        



    }
}
