//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_query_con_status
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************

#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_query_con_status(void)
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
int16 rak_query_con_status(void)
{
	int16	retval;
	char sendCmd[20]="";
	strcpy(sendCmd,RAK_QRY_CON_STATUS_CMD);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
