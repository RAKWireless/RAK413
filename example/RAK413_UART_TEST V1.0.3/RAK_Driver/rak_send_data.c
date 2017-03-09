//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_send_data
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************

#include "BSP_Driver.h"

extern uint8_t retval;
extern uint16_t  data_len;

/*=============================================================================*/
/**
 * @fn           int16 rak_send_data(void)
 * @brief        UART
 * @param[in]    socket_flag,Send_Data,Send_DataLen
 * @param[out]   none
 * @return       errCode
 *               -1 = Module busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/


uint8_t RAK_Send_data(uint8 socket_flag,uint16 dest_Port,uint8 *dest_Ip,uint16 Send_DataLen,uint8 *Send_Data)
{

	 char rak_sendHeader[100]="";
	 char ipframe[16]="";
	 uint16_t  headLen=0;
	 rak_bytes4ToAsciiDotAddr(dest_Ip,(uint8 *)ipframe);
	 headLen=sprintf(rak_sendHeader,"%s%d%s%d%s%s%s%d%s",RAK_SEND_DATA_CMD,socket_flag,RAK_COMMA,dest_Port,RAK_COMMA,ipframe,RAK_COMMA,Send_DataLen,RAK_COMMA);
	 retval=UART_send(rak_sendHeader,headLen);
     retval=UART_send((char *)Send_Data,Send_DataLen);
	 retval=UART_send(RAK_END,strlen(RAK_END));
	 if(retval!=0)
	 {			 			  
	    return retval;					
	 }				 
	 return retval;
}
