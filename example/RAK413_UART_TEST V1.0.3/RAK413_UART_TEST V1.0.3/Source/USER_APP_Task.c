//;*****************************************************
//;Company :  RAK WIRELESS
//;File Name : USER_APP_TASK.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;ʾ��������µ�ַ��
//;http://www.rakwireless.com/zh/product/4
//;****************************************************

#include "BSP_Driver.h"
#include "BSP_Init.h"


uint8_t   TCP_server_socket_num;	     //TCPS ͨ��socket
uint8_t   retval;					   //����ִ�з���ֵ

//;***************************************************************
//;Description:  TCPUDPData_Handle(void)
//;input	  : void
//;Returns    : void
//;Notes      :	RAK  TCPSever ���ݴ��� 
//;***************************************************************
void  TCPUDPData_Handle(void)
{
	uint16_t len = 0;
	int retval =0;

	//����TCPS
    if(uCmdRspFrame.recvdataFrame.recvdataframe.socketDescriptor==TCP_server_socket_num )
	{
	  retval =RAK_Send_data(TCP_server_socket_num,0,0,data_len-24,uCmdRspFrame.recvdataFrame.recvdataframe.recvdataBuf);
	  if(retval!=0)
	  {			 			  
		 return ;					
	  }
	  while(read_flag == FALSE);	 //�ȴ����������OK
	  read_flag = FALSE;
	}	

}

void recieveData_Handle(void)			  //�յ����ݵ�״̬����
{	  	
    uint8_t status=0;
    status =uCmdRspFrame.recvdataFrame.recvstatuframe.rspCode;
    switch(status)
	  {
			case 0x80://TCP�ͻ�������
			        if(data_len==23)		      //13+1+1+2+4+2
					 {
				 	   TCP_server_socket_num=uCmdRspFrame.recvdataFrame.recvstatuframe.socketDescriptor;
					 }
					 break;
			case 0x81://TCP�ͻ��˶Ͽ�
			      							
					 break;
			case 0x82://�����Զ����ӳɹ�
			      							
					 break;
		    case 0x83://���緢���Ͽ�
			      							
					 break;
	  	    default:
				   TCPUDPData_Handle();
		           break;					
	  } 
}

int main(void)
{
	SYS_Init();			//ϵͳ��ʼ��
	
	BSP_Init();		    //��ʼ��Ӳ�� 

	Reset_Target();	   //��λWIFIģ��

	retval=WIFI_MODULE_INIT(); //RAK WIFIģ���������


    while(1)
	{
		 if(read_flag == TRUE)						 //���յ����ݴ���
		 {		   
		    read_flag = FALSE;
			recieveData_Handle();			 
		 }

	}
}


