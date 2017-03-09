//;*****************************************************
//;Company :  RAK WIRELESS
//;File Name : USER_APP_TASK.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;示例代码更新地址：
//;http://www.rakwireless.com/zh/product/4
//;****************************************************

#include "BSP_Driver.h"
#include "BSP_Init.h"


uint8_t   TCP_server_socket_num;	     //TCPS 通信socket
uint8_t   retval;					   //函数执行返回值

//;***************************************************************
//;Description:  TCPUDPData_Handle(void)
//;input	  : void
//;Returns    : void
//;Notes      :	RAK  TCPSever 数据处理 
//;***************************************************************
void  TCPUDPData_Handle(void)
{
	uint16_t len = 0;
	int retval =0;

	//本地TCPS
    if(uCmdRspFrame.recvdataFrame.recvdataframe.socketDescriptor==TCP_server_socket_num )
	{
	  retval =RAK_Send_data(TCP_server_socket_num,0,0,data_len-24,uCmdRspFrame.recvdataFrame.recvdataframe.recvdataBuf);
	  if(retval!=0)
	  {			 			  
		 return ;					
	  }
	  while(read_flag == FALSE);	 //等待发送命令返回OK
	  read_flag = FALSE;
	}	

}

void recieveData_Handle(void)			  //收到数据的状态解析
{	  	
    uint8_t status=0;
    status =uCmdRspFrame.recvdataFrame.recvstatuframe.rspCode;
    switch(status)
	  {
			case 0x80://TCP客户端连接
			        if(data_len==23)		      //13+1+1+2+4+2
					 {
				 	   TCP_server_socket_num=uCmdRspFrame.recvdataFrame.recvstatuframe.socketDescriptor;
					 }
					 break;
			case 0x81://TCP客户端断开
			      							
					 break;
			case 0x82://网络自动连接成功
			      							
					 break;
		    case 0x83://网络发生断开
			      							
					 break;
	  	    default:
				   TCPUDPData_Handle();
		           break;					
	  } 
}

int main(void)
{
	SYS_Init();			//系统初始化
	
	BSP_Init();		    //初始化硬件 

	Reset_Target();	   //复位WIFI模块

	retval=WIFI_MODULE_INIT(); //RAK WIFI模块参数配置


    while(1)
	{
		 if(read_flag == TRUE)						 //接收到数据处理
		 {		   
		    read_flag = FALSE;
			recieveData_Handle();			 
		 }

	}
}


