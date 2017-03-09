#ifndef __BSP_DRIVER_H__
#define __BSP_DRIVER_H__

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "M051Series.h"
#include "rak_config.h"
#include "rak_global.h"

extern  uint8_t retval;
extern  volatile uint8_t  read_flag;
extern  uint32_t  valid_dataLen;
extern  uint16_t data_len;
extern  uint8_t UART_Coun_flg,UART_Coun;

int8    rak_bytes2ToAscii(uint16 hex,uint8 *strBuf);
uint8_t UART_send(char *tx_buf,uint16_t buflen);


void    CHIPIP_RST(void);
void    Reset_Target(void);
void    Delay_ms(int32_t ms);

#endif
