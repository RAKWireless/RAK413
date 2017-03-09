//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_get_scanaps
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.3
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_get_scanaps(uint8 num)
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
int16 rak_get_scanaps(uint8 num)
{
	int16						retval;
	char sendCmd[30]="";
	sprintf(sendCmd,"%s%d",RAK_GET_SCAN_CMD,num);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
