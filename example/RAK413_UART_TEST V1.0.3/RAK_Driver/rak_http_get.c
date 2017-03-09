//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_http_get
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_http_get(char* http_req)
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
int16 rak_http_get(char* http_req)
{
	int16						retval;
	char sendCmd[120]="";
	sprintf(sendCmd,"%s%s",RAK_HTTP_GET_CMD,http_req);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
