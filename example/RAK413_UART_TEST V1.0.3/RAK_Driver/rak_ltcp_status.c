//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_ltcp_status
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.2
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_ltcp_status(uint8 flag)
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
int16 rak_ltcp_status(uint8 flag)
{
	int16						retval;
	char sendCmd[30]="";

	sprintf(sendCmd,"%s%d",RAK_LTCP_STATUS_CMD,flag);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
