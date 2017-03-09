//;*****************************************************
//;Company :  RAK WIRELESS
//;File Name : BSP_INIT.c
//;Author :    Junhua
//;Create Data : 2014-03-15
//;Last Modified : 
//;Description : RAK413 WIFI UART  DRIVER
//;Version : 1.0.1
//;示例代码更新地址：
//;http://www.rakwireless.com/zh/product/4
//;****************************************************



#include "BSP_Init.h"

void SYS_Init(void)
{
/*---------------------------------------------------------------------------------------------------------*/
/* Init System Clock                                                                                       */
/*---------------------------------------------------------------------------------------------------------*/
	// Unlock protected registers 
    SYS_UnlockReg();

    // Enable Internal RC clock 
    SYSCLK->PWRCON |= SYSCLK_PWRCON_XTL12M_EN_Msk;

    // Waiting for IRC22M clock ready 
    SYS_WaitingForClockReady(SYSCLK_CLKSTATUS_XTL12M_STB_Msk);

	/* Switch HCLK clock source to PLL */
    SYSCLK->CLKSEL0 =SYSCLK_CLKSEL0_STCLK_XTAL | SYSCLK_CLKSEL0_HCLK_XTAL;

    SYSCLK->APBCLK = SYSCLK_APBCLK_UART1_EN_Msk |SYSCLK_APBCLK_SPI1_EN_Msk|SYSCLK_APBCLK_TMR1_EN_Msk 
										 |SYSCLK_APBCLK_TMR0_EN_Msk | SYSCLK_APBCLK_ADC_EN_Msk	;
								
	SYSCLK->CLKSEL1 = SYSCLK_CLKSEL1_UART_XTAL| SYSCLK_CLKSEL1_TMR1_XTAL;

		 
//  SYS_LockReg();
}

void UART1_Init(void)
{
/*---------------------------------------------------------------------------------------------------------*/
/* Init UART                                                                                               */
/*---------------------------------------------------------------------------------------------------------*/
	SYS->IPRSTC2 |= SYS_IPRSTC2_UART1_RST_Msk; 
	SYS->IPRSTC2 &= ~SYS_IPRSTC2_UART1_RST_Msk;
   /* Set P1 multi-function pins for UART1 RXD and TXD  */
    SYS->P1_MFP |= SYS_MFP_P12_RXD1 | SYS_MFP_P13_TXD1;

	UART1->BAUD = UART_BAUD_MODE2 | UART_BAUD_DIV_MODE2(__XTAL,115200); // __XTAL
    _UART_SET_DATA_FORMAT(UART1, UART_WORD_LEN_8 | UART_PARITY_NONE | UART_STOP_BIT_1);

    /* Enable Interrupt and install the call back function */
    _UART_ENABLE_INT(UART1, (UART_IER_RDA_IEN_Msk | UART_IER_RTO_IEN_Msk));
    NVIC_EnableIRQ(UART1_IRQn);	
}

void GPIO_Init(void)
{
	
    /* Reset pin */
	_GPIO_SET_PIN_MODE(RESET_PORT, RESET_PIN, GPIO_PMD_OUTPUT);

}

void FMC_Init(void)
{
    /* Enable IP clock */        
    SYSCLK->APBCLK |=SYSCLK_AHBCLK_ISP_EN_Msk;
    _FMC_ENABLE_ISP();
    _FMC_ENABLE_CFG_UPDATE();//允许修改 Config	
    FMC_Erase(FMC_CONFIG_BASE);
	FMC_Erase(FMC_CONFIG_BASE+4);
	FMC_Write(FMC_CONFIG_BASE,0xfffffffe);//可读0xfffffffe   不可读0xfffffffc
	FMC_Write(FMC_CONFIG_BASE+4,0X0001F000);//mini52:0x1c00;  mini54:3c00;
	_FMC_DISABLE_CFG_UPDATE();
   //FMC_Erase(FMC_DATAFLASH_BASE);
}
	 	

void TIMER1_Init(void)
{
    _TIMER_RESET(TIMER1);
	 /* Enable TIMER0 NVIC */
    NVIC_EnableIRQ(TMR1_IRQn);
	/* To Configure TCMPR values based on Timer clock source and pre-scale value */
    TIMER1->TCMPR = __XTAL*100/115200;                  // For 1 tick per second when using external 12MHz (Prescale = 1)
	/* Configure TCMP values of TIMER0 */
   _TIMER_RESET(TIMER1);
}
void BSP_Init(void)
{

    UART1_Init();	   //UART接口初始化，用于控制RAK模块

	GPIO_Init();	   //GPIO初始化

	FMC_Init();		   //FMC操作初始化   

	TIMER1_Init();

}
