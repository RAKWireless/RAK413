#ifndef _RAK_UART_API_H_
#define _RAK_UART_API_H_

/*******************  correlation function ******************/

uint8 WIFI_MODULE_INIT(void);
uint8 RAK_Send_data(uint8 socket_flag,uint16 dest_Port,uint8 *dest_Ip,uint16 Send_DataLen,uint8 *Send_Data);
int16 rak_query_fwversion(void);
int16 rak_set_pwrmode(uint8 powerMode);
int16 rak_scan_ap(rak_uScan *uScanFrame);
int16 rak_get_scanaps(uint8 num);
int16 rak_set_psk(rak_uJoin *uJoinFrame);
int16 rak_set_channel(rak_uApAdhoc *uApAdhocFrame);
int16 rak_connect_ap(rak_uJoin *uJoinFrame);
int16 rak_set_ApAdhoc_psk(rak_uApAdhoc *uApAdhocFrame);
int16 rak_creat_apadhoc(rak_uApAdhoc *uApAdhocFrame);
int16 rak_set_ipstatic(rak_uIpparam *uIpparamFrame);
int16 rak_set_ipdhcp(rak_uIpparam *uIpparamFrame);
int16 rak_query_dns(rak_uDns *uDnsFrame);
int16 rak_query_rssi(void);
int16 rak_wps(void);
int16 rak_wake_up(void);
int16 rak_easyconfig(void);
int16 rak_storeconfig(int8 param_en,config_t* param);
int16 rak_get_storeconfig(void);
int16 rak_set_webconfig(webconfig_t* param);
int16 rak_start_webconfig(void);
int16 rak_query_ipconfig(void);
int16 rak_open_socket(uint16 local_Port,uint16 dest_Port,uint8 rak_SocketCmd,uint8 *dest_Ip);
int16 rak_close_socket(uint8 flag);
int16 rak_query_con_status(void);
int16 rak_ltcp_status(uint8 flag);
int16 rak_http_get(char* http_req);
int16 rak_http_post(char* http_req ,uint8* post_data ,uint16 post_datalen);

#endif

