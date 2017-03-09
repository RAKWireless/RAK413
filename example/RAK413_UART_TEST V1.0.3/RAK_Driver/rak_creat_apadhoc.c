//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_creat_apadhoc.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************

#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_creat_apadhoc(void)
 * @brief        UART
 * @param[in]    rak_uApAdhoc
 * @param[out]   none
 * @return       errCode
 *               -1 = Module  busy 
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_creat_apadhoc(rak_uApAdhoc *uApAdhocFrame)
{
	int16	 retval = 0;
	char sendCmd[48]="";

     strcpy(sendCmd,RAK_AP_CMD);
	 strcat(sendCmd,(char *)uApAdhocFrame->apAdhocFrameSnd.ssid);
	 strcat(sendCmd,RAK_COMMA);
	 strcat(sendCmd,(char *)(uApAdhocFrame->apAdhocFrameSnd.apMode+0x30));
	 strcat(sendCmd,RAK_END);
	 retval=UART_send((char *)sendCmd,strlen(sendCmd));

	return retval;
}
