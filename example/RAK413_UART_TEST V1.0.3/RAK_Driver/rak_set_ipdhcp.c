//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_set_ipdhcp
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_ipdhcp(void)
 * @brief        UART 
 * @param[in]    rak_uIpparam
 * @param[out]   none
 * @return       errCode
 *               -1 = Module busy / Timeout
 *               0  = SUCCESS
 * @section description  
 * 
 */
 /*=============================================================================*/
int16 rak_set_ipdhcp(rak_uIpparam *uIpparamFrame)
{
	int16						retval;
	char sendCmd[14]="";
	char dhcpmode[2]="";
	strcpy(sendCmd,RAK_IPDHCP_CMD);
	dhcpmode[0]=uIpparamFrame->ipparamFrameSnd.dhcpMode+0x30;
	strcat(sendCmd,dhcpmode);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
