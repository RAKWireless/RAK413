//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_wake_up
//;Author :    Junhua
//;Create Data : 2014-10-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.2
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_wake_up(void)
 * @brief        UART 
 * @param[in]    powerMode
 * @param[out]   none
 * @return       errCode
 *               -1 = Module  busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
/*=============================================================================*/
int16 rak_wake_up(void)
{
	int16						retval;
	char sendCmd[20]="";

    strcpy(sendCmd, RAK_WAKE_UP_CMD);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
