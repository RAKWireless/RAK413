/**************************************************************************//**
 * @file     retarget.c
 * @version  V1.00
 * $Revision: 1 $
 * $Date: 12/04/05 7:07p $ 
 * @brief    M051 Series Debug Port and Semihost Setting Source File
 *
 * @note
 * Copyright (C) 2011 Nuvoton Technology Corp. All rights reserved.
 *
 ******************************************************************************/


#include <stdio.h>
#include "M051Series.h"

#if defined ( __CC_ARM   )
#if (__ARMCC_VERSION < 400000)
#else
/* Insist on keeping widthprec, to avoid X propagation by benign code in C-lib */
#pragma import _printf_widthprec
#endif
#endif

/*---------------------------------------------------------------------------------------------------------*/
/* Global variables                                                                                        */
/*---------------------------------------------------------------------------------------------------------*/
#if !(defined(__ICCARM__) && (__VER__ >= 6010000))
struct __FILE { int handle; /* Add whatever you need here */ };
#endif
FILE __stdout;
FILE __stdin;

/*---------------------------------------------------------------------------------------------------------*/
/* Routine to write a char                                                                                 */
/*---------------------------------------------------------------------------------------------------------*/
#define DEBUG_ENABLE_SEMIHOST
#if defined(DEBUG_ENABLE_SEMIHOST)
/* The static buffer is used to speed up the semihost */
static char g_buf[16];
static char g_buf_len = 0;

/* The function to process semihosted command */
extern int SH_DoCommand(int n32In_R0, int n32In_R1, int *pn32Out_R0);
#endif


/**
 * @brief    Routine to send a char
 *
 * @param    None
 *
 * @returns  Send value from UART debug port
 *
 * @details  Send a target char to UART debug port .
 */

void SendChar_ToUART(int ch)
{

    while(DEBUG_PORT->FSR & UART_FSR_TX_FULL_Msk);
    DEBUG_PORT->DATA = ch;
    if(ch == '\n'){
        while(DEBUG_PORT->FSR & UART_FSR_TX_FULL_Msk);
        DEBUG_PORT->DATA = '\r';
    }
}

/**
 * @brief    Routine to send a char
 *
 * @param    None
 *
 * @returns  Send value from UART debug port or semihost
 *
 * @details  Send a target char to UART debug port or semihost.
 */
void SendChar(int ch)
{
#if defined(DEBUG_ENABLE_SEMIHOST)
    g_buf[g_buf_len++] = ch;
    g_buf[g_buf_len] = '\0';
    if(g_buf_len + 1 >= sizeof(g_buf) || ch == '\n' || ch == '\0')
    {
        /* Send the char */
        if(SH_DoCommand(0x04, (int)g_buf, NULL) != 0)
        {
            g_buf_len = 0;
            return;
        }
        else
        {
            int i;

            for(i=0;i<g_buf_len;i++)
                SendChar_ToUART(g_buf[i]);
            g_buf_len = 0;
        }
    }
#else
    SendChar_ToUART(ch);
#endif
}

/**
 * @brief    Routine to get a char
 *
 * @param    None
 *
 * @returns  Get value from UART debug port or semihost
 *
 * @details  Wait UART debug port or semihost to input a char.
 */
char GetChar(void)
{
#ifdef DEBUG_ENABLE_SEMIHOST
    int nRet;
    while(SH_DoCommand(0x7, 0, &nRet) != 0)  
    {
                
        if(nRet != 0)
        {
            return (char)nRet;
        }
    }
		return 0;//??????********??????
#else

    while (1){
        if((DEBUG_PORT->FSR & UART_FSR_RX_EMPTY_Msk) == 0 )
        {
            return (DEBUG_PORT->DATA);
        }
    }

#endif
}

/**
 * @brief    Check any char input from UART 
 *
 * @param    None
 *
 * @retval   1: No any char input
 * @retval   0: Have some char input
 *
 * @details  Check UART RSR RX EMPTY or not to determine if any char input from UART
 */

int kbhit(void)
{
    return !((DEBUG_PORT->FSR&UART_FSR_RX_EMPTY_Msk) == 0);
}
/**
 * @brief    Check if debug message finished 
 *
 * @param    None
 *
 * @retval   1: Message is finished
 * @retval   0: Message is transmiting.
 *
 * @details  Check if message finished (FIFO empty of debug port)
 */

int IsDebugFifoEmpty(void)
{
    return ((DEBUG_PORT->FSR&UART_FSR_TE_FLAG_Msk) != 0);
}

/**
 * @brief    C library retargetting 
 *
 * @param    None
 *
 * @returns  None
 *
 * @details  Check if message finished (FIFO empty of debug port)
 */

void _ttywrch(int ch)
{
  SendChar(ch);
  return;
}


/**
 * @brief      Write character to stream
 *
 * @param[in]  ch       Character to be written. The character is passed as its int promotion. 
 * @param[in]  stream   Pointer to a FILE object that identifies the stream where the character is to be written. 
 *
 * @returns    If there are no errors, the same character that has been written is returned.
 *             If an error occurs, EOF is returned and the error indicator is set (see ferror).
 *
 * @details    Writes a character to the stream and advances the position indicator.\n
 *             The character is written at the current position of the stream as indicated \n
 *             by the internal position indicator, which is then advanced one character.
 *
 * @note       The above descriptions are copid from http://www.cplusplus.com/reference/clibrary/cstdio/fputc/.
 *
 *
 */

int fputc(int ch, FILE *stream)
{
  SendChar(ch);
  return ch;
}


/**
 * @brief      Get character from UART debug port or semihosting input
 *
 * @param[in]  stream   Pointer to a FILE object that identifies the stream on which the operation is to be performed. 
 *
 * @returns    The character read from UART debug port or semihosting
 *
 * @details    For get message from debug port or semihosting.
 *
 */

int fgetc(FILE *stream) {
   return (GetChar());
}

/**
 * @brief      Check error indicator
 *
 * @param[in]  stream   Pointer to a FILE object that identifies the stream.  
 *
 * @returns    If the error indicator associated with the stream was set, the function returns a nonzero value.
 *             Otherwise, it returns a zero value.
 *
 * @details    Checks if the error indicator associated with stream is set, returning a value different 
 *             from zero if it is. This indicator is generaly set by a previous operation on the stream that failed.
 *
 * @note       The above descriptions are copid from http://www.cplusplus.com/reference/clibrary/cstdio/ferror/.
 *
 */

int ferror(FILE *stream) {
  return EOF;
}

#ifdef DEBUG_ENABLE_SEMIHOST 
# ifdef __ICCARM__
void __exit(int return_code) {

    /* Check if link with ICE */
    if(SH_DoCommand(0x18, 0x20026, NULL) == 0)
    {
        /* Make sure all message is print out */
        while(IsDebugFifoEmpty() == 0);
    }
label:  goto label;  /* endless loop */
}
# else
void _sys_exit(int return_code) {

    /* Check if link with ICE */
    if(SH_DoCommand(0x18, 0x20026, NULL) == 0)
    {
        /* Make sure all message is print out */
        while(IsDebugFifoEmpty() == 0);
    }
label:  goto label;  /* endless loop */
}
# endif
#endif


