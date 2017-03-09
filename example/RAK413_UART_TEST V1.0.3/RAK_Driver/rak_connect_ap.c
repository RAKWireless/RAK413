//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_connect_ap.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************


#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_connect_ap(void)
 * @brief        UART, Connet_Ap 
 * @param[in]    rak_uJoin 
 * @param[out]   none
 * @return       errCode
 *               -1 = Module s busy 
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_connect_ap(rak_uJoin *uJoinFrame)
{
	int16						retval;
	char sendCmd[48]="";
    strcpy(sendCmd,RAK_CONN_CMD);
	strcat(sendCmd,(char *)uJoinFrame->joinFrameSnd.ssid);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
