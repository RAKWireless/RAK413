//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_start_autoconnect
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_start_autoconnect(void)
 * @brief        UART, 
 * @param[in]    none
 * @param[out]   none
 * @return       errCode
 *               -1 = Module busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_start_autoconnect(void)
{
	int16						retval;
	char sendCmd[20]="";
	strcpy(sendCmd,RAK_AUTO_CONNECT_CMD);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
