//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_scan_ap
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_scan_ap(void)
 * @brief        UART
 * @param[in]    rak_uScan
 * @param[out]   none
 * @return       errCode
 *               -1 = Module s busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_scan_ap(rak_uScan *uScanFrame)
{
	int16						retval;
	char sendCmd[36]="";
	char sendchannel[3]="";

	strcpy(sendCmd,RAK_SCAN_CMD);
	if(uScanFrame->scanFrameSnd.channel<10){sendchannel[0]=uScanFrame->scanFrameSnd.channel+0x30;}
	   else{sendchannel[0]=(uScanFrame->scanFrameSnd.channel)/10+0x30;sendchannel[1]=(uScanFrame->scanFrameSnd.channel)%10+0x30;}
	strcat(sendCmd,sendchannel);													 
	strcat(sendCmd,RAK_COMMA);
	strcat(sendCmd,(char *)uScanFrame->scanFrameSnd.ssid);
	strcat(sendCmd,RAK_END);
	retval=UART_send(sendCmd,strlen(sendCmd));
	return retval;
}
