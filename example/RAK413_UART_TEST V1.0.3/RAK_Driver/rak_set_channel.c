//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_set_channel
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_channel(void)
 * @brief        UART 
 * @param[in]    rak_uApAdhoc
 * @param[out]   none
 * @return       errCode
 *               -1 = Module  busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_set_channel(rak_uApAdhoc *uApAdhocFrame)
{
	int16						retval;
	char sendCmd[18]="";
	char set_channel[3]="";
    strcpy(sendCmd,RAK_CHANNEL_CMD);
	if(uApAdhocFrame->apAdhocFrameSnd.ibssApChannel<10){set_channel[0]=uApAdhocFrame->apAdhocFrameSnd.ibssApChannel+0x30;}
	   else{set_channel[0]=(uApAdhocFrame->apAdhocFrameSnd.ibssApChannel)/10+0x30;set_channel[1]=(uApAdhocFrame->apAdhocFrameSnd.ibssApChannel)%10+0x30;}
	strcat(sendCmd,set_channel);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
