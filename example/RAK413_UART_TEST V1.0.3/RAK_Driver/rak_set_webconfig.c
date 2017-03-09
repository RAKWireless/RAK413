//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_set_webconfig
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_webconfig(void)
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
int16 rak_set_webconfig(webconfig_t* param)
{
	int16		retval;

	retval=UART_send(RAK_WEB_CONFIG_CMD,strlen(RAK_WEB_CONFIG_CMD));
	retval=UART_send((char *)param,sizeof(webconfig_t));
	retval=UART_send(RAK_END,strlen(RAK_END));	
	return retval;
}
