//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_set_psk
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_psk(void)
 * @brief        UART 
 * @param[in]    rak_uJoin
 * @param[out]   none
 * @return       errCode
 *               -1 = Module busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
/*=============================================================================*/
int16 rak_set_psk(rak_uJoin *uJoinFrame)
{
	int16						retval;
	char sendCmd[44]="";
    strcpy(sendCmd,RAK_PSK_CMD);
	strcat(sendCmd,(char *)uJoinFrame->joinFrameSnd.psk);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
