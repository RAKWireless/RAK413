//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_open_socket.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

int8 rak_bytes2ToAscii(uint16 hex,uint8 *strBuf)
{
  uint8			i,j=0;							// loop counter
  uint8		    hexAddr[5];

  hexAddr[0]=hex/10000+0x30;
  hexAddr[1]=hex/1000%10+0x30;
  hexAddr[2]=hex/100%10+0x30;
  hexAddr[3]=hex/10%10+0x30;
  hexAddr[4]=hex%10+0x30;						
 
    for (i = 0; i< 5; i++) {
  	  if(hexAddr[i]==0x30)
	  {
	    j++	;
	  }
	  else break;
   }
  for (i = 0; i< 5-j; i++) {							
 	strBuf[i]= hexAddr[i+j];
  }
  strBuf[5-j]=0;
  return 1;
}
/*=============================================================================*/
/**
 * @fn           int16 rak_open_socket(void)
 * @brief        UART, 
 * @param[in]    local_Port,dest_Port,rak_SocketCmd,dest_Ip
 * @param[out]   none
 * @return       errCode
 *               -1 = Module  busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
/*=============================================================================*/
int16 rak_open_socket(uint16 local_Port,uint16 dest_Port,uint8 rak_SocketCmd,uint8 *dest_Ip)  //0:TCP 1:LTCP 2:UDP 3:LUDP
{
	int16						retval;
	char sendCmd[42]="";
	char port[16]="";

	
	switch (rak_SocketCmd)
	{
	  
	  case 0x00: 
	      { 
		  strcpy(sendCmd,RAK_TCP_CMD);
		  rak_bytes4ToAsciiDotAddr(dest_Ip,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(dest_Port,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(local_Port,(uint8 *)port);
		  strcat(sendCmd,port); 
		  }

	      break;
	  
	  case 0x01:
	      { 
		  strcpy(sendCmd,RAK_LTCP_CMD );
		  rak_bytes2ToAscii(local_Port,(uint8 *)port);
		  strcat(sendCmd,port);
		 }
	  
	      break;

     case 0x02: 
	     { 
		  strcpy(sendCmd,RAK_UDP_CMD);
		  rak_bytes4ToAsciiDotAddr(dest_Ip,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(dest_Port,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(local_Port,(uint8 *)port);
		  strcat(sendCmd,port);		
		 }

	     break;

	  case 0x03: 
	      { 
		  strcpy(sendCmd,RAK_LUDP_CMD);
		  rak_bytes2ToAscii(local_Port,(uint8 *)port);
		  strcat(sendCmd,port);
		  }

	      break;
	  case 0x04: 
	      { 
		  strcpy(sendCmd,RAK_MULTICAST_CMD);
		  rak_bytes4ToAsciiDotAddr(dest_Ip,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(dest_Port,(uint8 *)port);
		  strcat(sendCmd,port);
		  strcat(sendCmd,RAK_COMMA);
		  rak_bytes2ToAscii(local_Port,(uint8 *)port);
		  strcat(sendCmd,port);	
		  }

	    break;

	
	}
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
