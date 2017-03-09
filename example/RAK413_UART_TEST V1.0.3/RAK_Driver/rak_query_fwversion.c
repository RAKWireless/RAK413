//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rsi_query_fwversion
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rsi_query_fwversion(void)
 * @brief        UART, QUERY FIRMWARE VERSION 
 * @param[in]    none
 * @param[out]   none
 * @return       errCode
 *               -1 = Module  busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * This API is used to query the firmware version of the Wi-Fi module.
 */
 /*=============================================================================*/
int16 rak_query_fwversion(void)
{
	int16						retval;
	char sendCmd[15]="";
    strcpy(sendCmd,RAK_QRY_VERSION_CMD);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
