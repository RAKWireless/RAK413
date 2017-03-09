//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : RAK_WIFI_DRIVER.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;ʾ��������µ�ַ��
//;http://www.rakwireless.com/zh/product/4
//;****************************************************

#include "BSP_Driver.h"

rak_api rak_strApi;
rak_uCmdRsp	uCmdRspFrame;


/* Set SOCKET CONFIG */
uint8_t   RAK_DEST_IP_ADDRESS[4]={192,168,1,102}; 
uint16_t  RAK_DEST_PORT=0;
uint16_t  RAK_LOCAL_PORT=25000;

uint32_t  rak_Timer1=0;

//;***************************************************************
//;Description: WIFI_MODULE_INIT() 
//;input	  : void
//;Returns    : uint8_t
//;Notes      :	RAK ��WIFI�������� ִ�г������� ����Flash�е��û����� 
//;***************************************************************
uint8_t WIFI_MODULE_INIT()
{
   //Wait for "Welcome to RAK413"  Normal Boot
   do{
        Reset_Target();			  //RESET RAK413
	    while(read_flag == FALSE);
		read_flag = FALSE;       
    }while( strncmp((char *)uCmdRspFrame.uCmdRspBuf,"Welcome to RAK413\r\n",19) != 0); 
	

   //��ȡģ��İ汾��
	retval=rak_query_fwversion();					
	if(retval!=0)
	{			 			  
	  return retval;					
    }
	while(read_flag == FALSE);
	read_flag = FALSE;
	 
   
   //����ģ��Ĺ���ģʽ
	do{
		retval=rak_set_pwrmode(RAK_POWER_LEVEL_0);	
		if(retval!=0)
		{			 			  
			return retval;					
		}
		while(read_flag == FALSE);
		read_flag = FALSE;       
    }while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);
	 		
	//��ȡ�û�����
	rak_init_struct(&rak_strApi);

 if(RAK_AP_STATION_MODE==0)
  { 	    		
		//ɨ��ָ����ssid
		RAK_RESET_TIMER1;
		do {														
				retval=rak_scan_ap(&rak_strApi.uScanFrame);
				if(retval!=0)
				{			 			  
					return retval;					
				}	
				while(read_flag == FALSE);
				read_flag = FALSE;
				if(	RAK_INC_TIMER_1>10) break;			
		    }while( strncmp((char *)uCmdRspFrame.scanResponse.scanframe.rspCode,"OK",2) != 0);			
		 	if(uCmdRspFrame.errorResponse.errorframe.errorCode== 0xFE)
			 {

			 }

		 //�����ssid������
		do{
			retval=rak_set_psk(&rak_strApi.uJoinFrame);
				if(retval!=0)
				{			 			  
					return retval;					
					}				  
					while(read_flag == FALSE);
					read_flag = FALSE;      
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);	


	 //Connect to the specified network
	 RAK_RESET_TIMER1;
	 do {																 
			retval=rak_connect_ap(&rak_strApi.uJoinFrame);
			if(retval!=0)
				{			 			  
					return retval;					
				}				  
			while(read_flag == FALSE);
			read_flag = FALSE; 		
			if(	RAK_INC_TIMER_1>RAK_PRAR_ERROR_TIMEOUT)
					return uCmdRspFrame.errorResponse.errorframe.errorCode; 
		}while( strncmp((char *)uCmdRspFrame.mgmtResponse.mgmtframe.rspCode,"OK",2) != 0);
		if(uCmdRspFrame.errorResponse.errorframe.errorCode== 0xFD)
		{
		
		}
				
		//����IP��ȡ��ʽ
				  //0: ��̬����  1����̬����
		   if(RAK_IPDHCP_MODE_ENABLE==1)
		   {
		 	do {												
					retval=rak_set_ipstatic(&rak_strApi.uIpparamFrame);		  //IPstatic
					if(retval!=0)
					{			 			  
						return retval;					
					}	  
					while(read_flag == FALSE);
					read_flag = FALSE;
			}while( strncmp((char *)uCmdRspFrame.scanResponse.scanframe.rspCode,"OK",2) != 0);
		   }
		   else if(RAK_IPDHCP_MODE_ENABLE==0)
		  {
		 	rak_strApi.uIpparamFrame.ipparamFrameSnd.dhcpMode=0;
		 	do {												
				  retval=rak_set_ipdhcp(&rak_strApi.uIpparamFrame);		  //IPdhcp	client
				  if(retval!=0)
					{			 			  
						return retval;					
					}	  
					while(read_flag == FALSE);
					read_flag = FALSE;
			}while( strncmp((char *)uCmdRspFrame.scanResponse.scanframe.rspCode,"OK",2) != 0);		
		  }						
}			

else if(RAK_AP_STATION_MODE==1)

	{
			
		//����AP��ADHCO����	���ŵ�
		do{
				retval=rak_set_channel(&rak_strApi.uApAdhocFrame);
				if(retval!=0)
				{			 			  
					return retval;					
				}				  
				while(read_flag == FALSE);
				read_flag = FALSE;      
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);
				
		//����AP��ADHCO����	������
		do{
				retval=rak_set_psk(&rak_strApi.uJoinFrame);
				if(retval!=0)
				{			 			  
					return retval;					
					}				  
					while(read_flag == FALSE);
					read_flag = FALSE;      
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);		
		
		//����AP��ADHCO����
		RAK_RESET_TIMER1;
		do{
				retval=rak_creat_apadhoc(&rak_strApi.uApAdhocFrame);
				if(retval!=0)
				{			 			  
					return retval;					
				}
				while(read_flag == FALSE);
				read_flag = FALSE;  
				if(	RAK_INC_TIMER_1>RAK_PRAR_ERROR_TIMEOUT)return uCmdRspFrame.errorResponse.errorframe.errorCode;
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);	
		
		//���ñ��ؾ�̬IP							
		do{
				retval=rak_set_ipstatic(&rak_strApi.uIpparamFrame);		  //IPstatic 
				if(retval!=0)
				{			 			  
					return retval;					
				}	  
				while(read_flag == FALSE);
				read_flag = FALSE;   
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);
				
		////APģʽ  ����DHCP SERVER����
	
			do{
					rak_strApi.uIpparamFrame.ipparamFrameSnd.dhcpMode=1;
					retval=rak_set_ipdhcp(&rak_strApi.uIpparamFrame);		  //IPdhcp  sever
					if(retval!=0)
					{			 			  
						return retval;					
					}	  
					while(read_flag == FALSE);
					read_flag = FALSE;  
				}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);
	


		//��ѯ����״̬
		do{
				retval=rak_query_con_status();  //
				if(retval!=0)
				{			 			  
					return retval;					
				}									
				while(read_flag == FALSE);
				read_flag = FALSE;   
			}while(strncmp((char *)uCmdRspFrame.uCmdRspBuf,"OK",2) != 0);
			
			
	 
  }

	//��������socket
	do {
		SYS_SysTickDelay(5000);
		retval=rak_open_socket(RAK_LOCAL_PORT,RAK_DEST_PORT,1,RAK_DEST_IP_ADDRESS);  //0:TCP 1:LTCP 2:UDP 3:LUDP  
		while(read_flag == FALSE);
		read_flag = FALSE;
	   }while( strncmp((char *)uCmdRspFrame.socketFrameRcv.socketframe.rspCode,"OK",2) != 0);	
//	  TCP_server_socket_num=uCmdRspFrame.socketFrameRcv.socketOkframe.socketDescriptordata;

	//��ѯģ���IP������Ϣ
	retval=rak_query_ipconfig();
    if(retval!=0)
	{			 			  
	  return retval;					
    }	  
    while(read_flag == FALSE);
    read_flag = FALSE;

	if( strncmp((char *)uCmdRspFrame.ipconfigResponse.ipconfigframe.rspCode,"OK",2) == 0)
	{
	  memcpy((char *)rak_strApi.macAddress,uCmdRspFrame.ipconfigResponse.ipconfigframe.macAddr,6);		//����MAC��ַ
	  memcpy((char *)rak_strApi.uIpparamFrame.ipparamFrameSnd.ipaddr,uCmdRspFrame.ipconfigResponse.ipconfigframe.ipAddr,4);
	}		
	return retval; 
}

