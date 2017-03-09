#ifndef __BSP_INIT_H__
#define __BSP_INIT_H__

/*******************************************************************************
 *****************************   INCLUDE FILES   *******************************
 ******************************************************************************/
#include <stdio.h>
#include <string.h>
#include "M051Series.h"
#include "rak_config.h"
#include "rak_global.h"

/*******************************************************************************
 **************************   FUNCTION PROTOTYPES   ****************************
 ******************************************************************************/

void SYS_Init(void);
void WDT_init(void);
void UART1_Init(void);
void GPIO_Init(void);
void FMC_Init(void);
void SPI1_LCD_Init(void);
void ADC_Init(void);
void TIMER0_Init(void);
void BSP_Init(void);
#endif
