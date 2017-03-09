//;****************************************************
//;Company :  RAK WIRELESS
//;File Name : rak_storeconfig
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;****************************************************
#include "BSP_Driver.h"

/*=============================================================================*/
/**
 * @fn           int16 rak_storeconfig(void)
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
int16 rak_storeconfig(int8 param_en,config_t* param)
{
	int16		retval;
	uint8	    CmdheadLen=0;
	char sendCmd[20]="";

	if(param_en)
	{
	  CmdheadLen=sprintf(sendCmd,"%s%s",RAK_STORE_CONFIG_CMD,RAK_EQUAL);
	  retval=UART_send((char *)sendCmd,CmdheadLen);
	  retval=UART_send((char *)param,sizeof(config_t));
	  retval=UART_send(RAK_END,strlen(RAK_END));
	}else
	{
	  strcpy(sendCmd,RAK_STORE_CONFIG_CMD);
	  strcat(sendCmd,RAK_END);
	  retval=UART_send((char *)sendCmd,strlen(sendCmd));
	}	
	return retval;
}
