//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_http_post
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_http_post(char* http_req ,uint8* post_data ,uint16 post_datalen)
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
int16 rak_http_post(char* http_req ,uint8* post_data ,uint16 post_datalen)
{
	int16						retval;
	char sendCmd[120]="";

	sprintf(sendCmd,"%s%s%s",RAK_HTTP_POST_CMD,http_req,RAK_COMMA);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	retval=UART_send((char *)post_data,post_datalen);
	retval=UART_send(RAK_END,strlen(RAK_END));
	
	return retval;
}
