//;*****************************************************
//;Company :  RAK WIRELESS
//;File Name : BSP_DRIVER.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;示例代码更新地址：
//;http://www.rakwireless.com/zh/product/4
//;****************************************************

#include "BSP_Driver.h"


volatile uint8_t      read_flag = 0;			//串口接收WIFI数据标志位
volatile uint16_t UART_data_len = 0;            //串口接收长度                                                                          
uint16_t data_len=0;


/*---------------------------------------------------------------------------------------------------------*/
/* ISR to handle UART Channel 1 interrupt event                                                            */
/*---------------------------------------------------------------------------------------------------------*/
void UART1_IRQHandler(void)
{
   	uint8_t bInChar[1]={0};
		
    if(UART1->ISR & UART_ISR_RDA_INT_Msk)                              //检查是否接收中断 Check if receive interrupt
    {
		_TIMER_RESET(TIMER1); 				
		if(_UART_IS_RX_READY(UART1))                                //检查接收到的数据是否有效//en:Check if received data avaliable
        {
            while (UART1->FSR & UART_FSR_RX_EMPTY_Msk);                //Wait until an avaliable char present in RX FIFO
            bInChar[0] = UART1->RBR;                               //读取字符Read the char		
            if(UART_data_len < RXBUFSIZE)                        //测缓冲区满否 Check if buffer full
            {
                uCmdRspFrame.uCmdRspBuf[UART_data_len] = bInChar[0];        //Enqueue the character
                UART_data_len++;
            }	
        }
		_TIMER_START(TIMER1, TIMER_TCSR_CEN_Msk | TIMER_TCSR_IE_Msk | TIMER_TCSR_MODE_PERIODIC | TIMER_TCSR_TDR_EN_Msk ,1);				
    }


}

/********************  UART send data ************************/
uint8_t UART_send(char *tx_buf,uint16_t buflen)
{
    uint16_t i;
	if(read_flag == TRUE)
	   return 0xFF;
    for (i=0; i<buflen; i++) {       
    while((UART1->FSR & UART_FSR_TX_FULL_Msk) != 0);                   //发送FIFO满时等待
    UART1->THR = (uint8_t) tx_buf[i];                           //通过UART0发送一个字符
    }
    return 0;	
}


/************************  TMR1_IRQHandler  ****************************/

void TMR1_IRQHandler(void)
{
    /* Clear TIMER0 Timeout Interrupt Flag */
   _TIMER_CLEAR_CMP_INT_FLAG(TIMER1);
   _TIMER_RESET(TIMER1); 
   	read_flag = TRUE;				   //串口接收完一包
	data_len = UART_data_len;
	UART_data_len=0;

}

/* RESET CPU core and IP */
void CHIPIP_RST(void)
{
	/* RESET CPU core and IP */
    SYS->IPRSTC1 |= SYS_IPRSTC1_CHIP_RST_Msk;
}

/************************  RESET RAK413  ****************************/
void Delay_ms(int32_t ms)
{
    int32_t i;

    for(i=0;i<ms;i++)
        SYS_SysTickDelay(1000);
}

void Reset_Target(void)
{
    RESET_PORT_PIN=0;
    Delay_ms(20);
    RESET_PORT_PIN=1; 
}



