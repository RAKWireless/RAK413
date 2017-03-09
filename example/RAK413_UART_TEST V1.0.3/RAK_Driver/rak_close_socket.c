//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_close_socket
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_close_socket(uint8 flag)
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
int16 rak_close_socket(uint8 flag)
{
	int16						retval;
	char sendCmd[30]="";

	sprintf(sendCmd,"%s%d",RAK_CLS_SOCK_CMD,flag);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
