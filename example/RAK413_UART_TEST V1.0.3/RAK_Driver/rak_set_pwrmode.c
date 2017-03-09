//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_set_pwrmode
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_pwrmode(void)
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
int16 rak_set_pwrmode(uint8 powerMode)
{
	int16						retval;
	char sendCmd[16]="";
	char pwrmode[2]="";
    strcpy(sendCmd,RAK_PWRMODE_CMD);
	pwrmode[0]=powerMode+0x30;
	strcat(sendCmd,pwrmode);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
