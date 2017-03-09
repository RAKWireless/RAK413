//;****************************************************
//;Company :  RAK WIRELESS
//;File Name :  rak_set_ipstatic
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_set_ipstatic(void)
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
int16 rak_set_ipstatic(rak_uIpparam *uIpparamFrame)
{
	int16						retval;
	char sendCmd[80]="";
	uint8 ipparmddr[14]="";
	strcpy(sendCmd,RAK_IPSTATIC_CMD);
	rak_bytes4ToAsciiDotAddr(uIpparamFrame->ipparamFrameSnd.ipaddr,ipparmddr);
	strcat(sendCmd,(char *)ipparmddr);
	strcat(sendCmd,RAK_COMMA);
	rak_bytes4ToAsciiDotAddr(uIpparamFrame->ipparamFrameSnd.netmask,ipparmddr);
	strcat(sendCmd,(char *)ipparmddr);
	strcat(sendCmd,RAK_COMMA);
	rak_bytes4ToAsciiDotAddr(uIpparamFrame->ipparamFrameSnd.gateway,ipparmddr);
	strcat(sendCmd,(char *)ipparmddr);
	strcat(sendCmd,RAK_COMMA);
	rak_bytes4ToAsciiDotAddr(uIpparamFrame->ipparamFrameSnd.dnssvr1,ipparmddr);
	strcat(sendCmd,(char *)ipparmddr);
	strcat(sendCmd,RAK_COMMA);
	rak_bytes4ToAsciiDotAddr(uIpparamFrame->ipparamFrameSnd.dnssvr2,ipparmddr);
	strcat(sendCmd,(char *)ipparmddr);
	strcat(sendCmd,RAK_END);
	retval=UART_send((char *)sendCmd,strlen(sendCmd));
	return retval;
}
