#ifndef _RAKGLOBAL_H_
#define _RAKGLOBAL_H_
/*
 * @ Type Definitions
 */
typedef unsigned char	uint8;
typedef unsigned short	uint16;
typedef unsigned long	uint32;
typedef signed char	    int8;
typedef signed short	int16;
typedef signed long	     int32;


/*********************  Interface Definition  ***********************************/
#define RXBUFSIZE 1500

#define    RESET_PORT             P4
#define    RESET_PIN              2
#define    RESET_PORT_PIN         P42	   //wifi RESET pin


/********************** The AT command definition **********************************/

/********************** 管理命令 **********************************/
#define    RAK_QRY_MAC_CMD                    "at+mac"
#define    RAK_QRY_VERSION_CMD                "at+version"
#define    RAK_PWRMODE_CMD                    "at+pwrmode="	     //@Param: 0：normal 1：sleep 2: deep sleep  3：shut down
#define    RAK_WAKE_UP_CMD                    "at+wake_up"
#define    RAK_RESET_CMD                      "at+reset"

/********************** 网络操作命令 **********************************/
#define    RAK_SCAN_CMD                   "at+scan="	 
#define    RAK_GET_SCAN_CMD               "at+get_scan="	      //@Param: number 
#define    RAK_PSK_CMD                    "at+psk=" 			  //@Param: psk
#define    RAK_CHANNEL_CMD                "at+channel="			  //@Param: 1-13
#define    RAK_AP_CMD                     "at+ap="				  //@Param:	ssid
#define    RAK_ADHOC_CMD                  "at+adhoc="			  //@Param:	ssid
#define    RAK_CONN_CMD                   "at+connect="			  //@Param:	ssid  
#define    RAK_IPSTATIC_CMD               "at+ipstatic="		  //@Param: <ip>,<mask>,<gateway>,<dns server1 >,< dns server2>
#define    RAK_IPDHCP_CMD             	  "at+ipdhcp="			  //@Param: 0 DHCP CLIENT  1 DHCP SERVER
#define    RAK_EASY_CONFIG_CMD            "at+easy_config"
#define    RAK_WPS_CMD                    "at+wps"
#define    RAK_QRY_CON_STATUS_CMD         "at+con_status"
#define    RAK_QRY_IPCONFIG_CMD           "at+ipconfig"
#define    RAK_RSSI_CMD                   "at+rssi"
#define    RAK_DNS_CMD                    "at+dns="				  //@Param: domain name
#define    RAK_PING_CMD                   "at+ping="			  //@Param:	<host>, <count>, <size>
#define    RAK_AP_CONFIG_CMD              "at+apconfig="		  //@Param:	<hidden>,<contry code>
#define    RAK_LISTEN_CMD 			      "at+listen="			  //@Param:	<listen interval>
#define    RAK_DIS_AP_CMD                 "at+disc" 	


/********************** Socket操作命令 **********************************/
#define    RAK_LTCP_CMD                   "at+ltcp="			 //@Param: <local_port>     
#define    RAK_TCP_CMD                    "at+tcp="				 //@Param: <dest_ip>,<dest_port>,<module_port>
#define    RAK_LUDP_CMD                   "at+ludp=" 			 //@Param: <local port>
#define    RAK_UDP_CMD                    "at+udp=" 			 //@Param: <dest_ip>,<dest_port>,<local_port>
#define    RAK_MULTICAST_CMD              "at+multicast="	 	 //@Param: <dest_ip>,<dest_port>,<local_port>
#define    RAK_CLS_SOCK_CMD               "at+cls="				 //@Param: <flag>
#define    RAK_SEND_DATA_CMD         	  "at+send_data="		 //@Param: <flag>,<dest_port>,<dest_ip>,<data_length> ,<data_stream>
#define    RAK_RECV_DATA_CMD         	  "at+recv_data=" 		 //@Param: <flag><dest_port><dest_ip><data_length><data_stream>
																 //@Param: <socket_status><flag><dest_port><dest_ip>
#define    RAK_LTCP_STATUS_CMD			  "at+ltcp_status="		 //@Param: <ltcp_flag>
#define    RAK_HTTP_GET_CMD			      "at+http_get=" 		 //@Param: <ip/domain>:<port>/<url>
#define    RAK_HTTP_POST_CMD			  "at+http_post=" 		 //@Param:

#define    RAK_UART_CONFIG_CMD			  "at+uartconfig="		 //@Param: <baud rate>,<data bits>,<stop bits>,<parity>,<flow ctrol>
	
/********************** 参数保存命令 **********************************/
#define    RAK_STORE_CONFIG_CMD           "at+storeconfig"  //"at+storeconfig="	//@Param: param_struct  带参数保存和不带参数保存
#define    RAK_GET_STORE_CONFIG_CMD       "at+get_storeconfig" 
#define    RAK_WEB_CONFIG_CMD             "at+web_config="		 //@Param:	 param_struct
#define    RAK_GET_WEB_CONFIG_CMD         "at+get_webconfig"
#define    RAK_AUTO_CONNECT_CMD           "at+auto_connect"
#define    RAK_START_WEB_CMD              "at+start_web"

/**************************符号*******************************************/
#define    RAK_EQUAL					  "="
#define    RAK_COMMA					  ","
#define    RAK_END						  "\r\n"


#define RAK_TCP_SOCKET                     0
#define RAK_LTCP_SOCKET                    1 			 //0:TCP 1:LTCP 2:UDP 3:LUDP  4:multicast
#define RAK_UDP_SOCKET                     2
#define RAK_LUDP_SOCKET                    3
#define RAK_MULTI_SOCKET                   4

/** **********超时设置***********/		

#define RAK_TICKS_PER_SECOND   100 
#define RAK_INC_TIMER_1        rak_Timer1++
#define RAK_RESET_TIMER1       rak_Timer1=0

#define RAK_PRAR_ERROR_TIMEOUT     10 * RAK_TICKS_PER_SECOND
#define RAK_POST_DATA_TIMEOUT       2 * RAK_TICKS_PER_SECOND

/**
 * Device Parameters
 */
#define RAK_MAXSOCKETS			        8		// maximum number of open sockets
#define RAK_FRAME_CMD_RSP_LEN           22
#define RAK_MAX_PAYLOAD_SIZE			1024     // maximum recieve data payload size
#define RAK_MAX_PAYLOAD_SEND_SIZE		800       // maximum recieve data payload size
#define RAK_AP_SCANNED_MAX		        10	       // maximum number of scanned acces points
#define RAK_LTCP_CLIENT_MAX		        7	  	  // maximum number of ltcp connected clients


#define RAK_MAX_DOMAIN_NAME_LEN 		42
#define RAK_SSID_LEN			        33	     // maximum length of SSID
#define RAK_BSSID_LEN			        6	     // maximum length of SSID
#define RAK_IP_ADD_LEN                  4
#define RAK_MAC_ADD_LEN                 6

#define RAK_PSK_LEN						65 

/*==============================================*/
/*
 * Device Send  Struct
 */
/*==============================================*/
typedef  union{	
	struct{
	uint8                   sndDataCmd[sizeof(RAK_SEND_DATA_CMD)-1]; 
	uint8				    socketFlag[1];
	uint8			   	    destPort[2];			                // Port number of what we are send to
	uint8				    destIpaddr[4];				  
    uint8				    sndDataLen[2];
	}usndDataFramesend;
    uint8	usndDataBuf[sizeof(RAK_SEND_DATA_CMD)-1+9];			
} rak_usnddata;




/*==============================================*/
/*
 * Main struct which defines all the paramaters of the API library
 * This structure is defined as a variable, the  pointer to, which is passed
 *  toall the functions in the API
 * The struct is initialized by a set of #defines for default values
 *  or for simple use cases
 */
/*==============================================*/
// The scan command argument union/variables

typedef union {
	struct {
		uint8				channel;			    // RF channel to scan, 0=All, 1-13 for 2.5GHz 
		uint8				ssid[RAK_SSID_LEN];		// uint8[32], ssid to scan for
	} scanFrameSnd;
	uint8 				uScanBuf[RAK_SSID_LEN + 1];		// byte format to send to the spi interface, 36 bytes
} rak_uScan;


typedef union {
	struct {
	    uint8			    securityType ;			// 0:不加密  1：加密
		uint8				psk[RAK_PSK_LEN];		// pre-shared key, 32-byte string
		uint8				ssid[RAK_SSID_LEN];	    // ssid of access point to join to, 33-byte string
	} joinFrameSnd;
	uint8					uJoinBuf[RAK_SSID_LEN + RAK_PSK_LEN + 1];			
} rak_uJoin;

typedef union {
	struct {
	    uint8				nwType;					// 0 sta 1 AP 2 ADHOC
		uint8			    securityType ;			// 0:不加密  1：加密
		uint8				ssid[RAK_SSID_LEN];	    // ssid of access point to join to, 33-byte string
		uint8				apMode;					// ap mode	 是否隐藏
		uint8				ibssApChannel;			// rf channel number for Ad-Hoc (IBSS) mode or  AP mode
	} apAdhocFrameSnd;
	uint8					uApAdhocBuf[RAK_SSID_LEN + 4];			
} rak_uApAdhoc;

typedef union {
	struct {
		uint8				dhcpMode;		        // 0=dhcp client, 1=ip static
		uint8				ipaddr[4];			// IP address of this module if in manual mode
		uint8				netmask[4];		        // Netmask used if in manual mode
		uint8				gateway[4];			// IP address of default gateway if in manual mode
		uint8               dnssvr1[4];
		uint8               dnssvr2[4];
		uint8				padding;
	} ipparamFrameSnd;
	uint8					uIpparamBuf[22];		
} rak_uIpparam;


typedef union {
	struct {
		uint8 				DomainName[RAK_MAX_DOMAIN_NAME_LEN];   // 42 bytes, Domain name like www.google.com 
	} dnsFrameSnd;
	uint8					uDnsBuf[RAK_MAX_DOMAIN_NAME_LEN];			       
} rak_uDns;


typedef union {
	struct {
	    uint8				socketType;					   //0:TCP 1:LTCP 2:UDP 3:LUDP 
		uint8				moduleSocket[2];		       // Our local module port number
		uint8				destSocket[2];			       // Port number of what we are connecting to
		uint8				destIpaddr[4];			       // IP address of socket we are connecting to
	} socketFrameSnd;
	uint8					uSocketBuf[10];			       
} rak_uSocket;

typedef struct {

	uint8					powerMode;				
	uint8					macAddress[6];				// 6 bytes, mac address
	rak_uScan				uScanFrame;
	rak_uJoin				uJoinFrame;
	rak_uApAdhoc            uApAdhocFrame;    
	rak_uIpparam		    uIpparamFrame;
	rak_uDns          		uDnsFrame;
	rak_uSocket				uSocketFrame;
	rak_usnddata      		usnddataFrame;	
	uint8             		listeninterval[2];		   //new
} rak_api;


typedef union{
	struct {
		uint8                  rspCode[2];  			   			
		uint8	 			   end[2]; 
	} mgmtframe;
	uint8				       mgmtFrameRcv[6];
} rak_mgmtResponse;

typedef union{
	struct {
		uint8                  rspCode[2];
		uint8                  errorCode;  			   			
		uint8	 			   end[2]; 
	} errorframe;
	uint8				       errorFrameRcv[6];
} rak_errorResponse;


typedef struct {
	uint8					ssid[RAK_SSID_LEN];			// 33-byte ssid of scanned access point
	uint8					bssid[RAK_BSSID_LEN];			// 32-byte bssid of scanned access point
	uint8					rfChannel;				// rf channel to us, 0=scan for all
	uint8					rssiVal;				// absolute value of RSSI
	uint8					securityMode;				// security mode
} rak_getscanInfo;

typedef  union {	
	struct {
		uint8                  rspCode[2];  			   //0= success	   !0= Failure
		uint8				   scanCount;				// uint8, number of access points found
		uint8	 			   end[2]; 
	} scanframe;
	uint8				      scanFrameRcv[6]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_scanResponse;

typedef  union {	
	struct {
		uint8                   rspCode[2]; 
		rak_getscanInfo		    strScanInfo[RAK_AP_SCANNED_MAX];	// 32 maximum responses from scan command
		uint8	 				end[2]; 
	} getscanframe;
	uint8				      getscanFrameRcv[44]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_getscanResponse;


typedef union {
	struct {
		uint8                       rspCode[2];  			   
		uint8				    	status;				      
		uint8	 					end[2]; 
	}qryconFrame;
	uint8				      qryconFrameRcv[6]  ;
} rak_qryconResponse; 


typedef  union {
	
	struct {
		uint8                   rspCode[2];  	 //0=connected  -2=no connect
		uint8                   rssi;
		uint8	 			    end[2]; 
	} qryrssiframe;
	uint8				      qryrssiFrameRcv[6]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_qryrssiResponse; 


typedef  union {	
	struct {
		uint8                       rspCode[2];  			
		uint8				        macAddr[6];				// MAC address of this module
		uint8				        ipAddr[4];				// Configured IP address
		uint8				        netMask[4];				// Configured netmask
		uint8				        gateWay[4];				// Configured default gateway
		uint8				        dns1[4];				// dns1
		uint8				        dns2[4];				// dns2
		uint8	 					end[2];  
	} ipconfigframe;
	uint8				          ipconfigFrameRcv[30];			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_ipconfigResponse; 


typedef  union{	
	struct {
	   uint8                   rspCode[2];  			   //0= success	   !0= Failure
	   uint8				   domainAddr[4];				//  
	   uint8	 			   end[2]; 
	} qrydnsframe;
	uint8				       qrydnsFrameRcv[8];			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_qrydnsResponse;


typedef union{
	struct {
		uint8                  rspCode[2];  			   
		uint8				   status;			
		uint8	 			   end[2]; 
	} pingframe;
	uint8				       pingFrameRcv[6];
} rak_pingResponse;


typedef union {	
	struct {
		uint8                   rspCode[2];                    		// command code 
		uint8                   socketDescriptordata; 
		uint8	 				end[2];   
	} socketframe;
	uint8				       socketFrameRcv[6]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_socketFrameRcv;



typedef  union {
	
	struct {
		uint8                  CMDCode[13]; 		   
		uint8				   socketDescriptor;
		uint8				   destPort[2];
		uint8				   destIp[4];
		uint8				   recDataLen[2];
		uint8                  recvdataBuf[RAK_MAX_PAYLOAD_SIZE];
		uint8	 			   end[2]; 
	} recvdataframe;
	struct {
		uint8                   CMDCode[13]; 		   // 0x01 TCP/UCP 0xFF recieve error 0x80 TCP con	0x81 TCP dis 0x82 0x83
		uint8				    rspCode;//socketstatus;		
		uint8				    socketDescriptor;
		uint8					destPort[2];
		uint8					destIp[4];
		uint8	 			    end[2];
	} recvstatuframe;
	uint8				      socketFrameRcv[RAK_MAX_PAYLOAD_SIZE+24]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_recvdataFrame;

typedef union {	
	struct {
		uint8                 rspCode[2]; 
		uint8				  mac[6];				
		uint8	 			  end[2];	 
	} qryMacframe;
	uint8				      qryMacFrameRcv[18];		
} rak_qryMacFrameRcv;

typedef union {	
	struct {
		uint8                 rspCode[2]; 
		uint8				  hostFwversion[8];				// uint8[10], firmware version text string, x.y.z as 1.3.0
		uint8				  wlanFwversion[6];
		uint8	 			  end[2];	 
	} qryFwversionframe;
	uint8				      qryFwversionFrameRcv[18]  ;			// uint8, socket descriptor, like a file handle, usually 0x00
} rak_qryFwversionFrameRcv;


typedef struct {
    uint32   addr;
    uint32   mask;
    uint32   gw;
    uint32   dnsrv1;
    uint32   dnsrv2;     
}ip_param_t;

typedef struct {
    uint8   hidden;      //AP隐藏开关
    char    country[3];  //国家代码
}ap_config_t;

typedef struct {
    uint32        feature_bitmap;
    uint8         net_type;    
    uint8         channel;
    uint8         sec_mode;
    uint8         dhcp_mode;
    char          ssid[RAK_SSID_LEN];
    char          psk[RAK_PSK_LEN];
	uint8		  dummy[2];
    ip_param_t    ip_param;
    ap_config_t   ap_config;
}config_t;

typedef struct {
    config_t       web_params;    
    char           user_name[17];
    char           user_psk[17];
	uint8          dummy[2];
}webconfig_t; 


typedef union {	
	struct {
		uint8                 rspCode[2]; 
		config_t			  config_params; 
		uint8	 			  end[2];	 
	} getStoreCfgFrame;
	uint8				      getStoreCfgFrameRcv[sizeof(config_t)+4]  ;		
}rak_getStoreCfgFrame;


typedef union {	
	struct {
		uint8                 rspCode[2]; 
		webconfig_t 		  web_config;
		uint8	 			  end[2];	 
	} getWebCfgFrame;
	uint8				      getWebCfgFrameRcv[sizeof(webconfig_t)+4]  ;			
}rak_getWebCfgFrame;

typedef struct {
	uint8					tcpclnt_flag;			
	uint8					dest_port[2];		
	uint8					dest_ip[4];						
}rak_tcpClntInfo;

typedef union {	
	struct {
		uint8                 rspCode[2]; 
		uint8                 tcp_num;
	    rak_tcpClntInfo		  tcpClntInfo[RAK_LTCP_CLIENT_MAX];
		uint8	 			  end[2];	 
	}ltcpStatusFrame;
	uint8				      ltcpStatusRcv[sizeof(webconfig_t)+4]  ;			
}rak_ltcpStatusFrame;

typedef union {	
	struct {
		uint8                 rspCode[2]; 
		uint8			      statusCode[2];
		uint8			      pageLen[2];
		uint8			      pageData[RAK_MAX_PAYLOAD_SIZE]; 
		uint8	 			  end[2];	 
	} httpGetPostFrame;
	uint8				      httpGetPostFrameRcv[RAK_MAX_PAYLOAD_SIZE +8];			// uint8, socket descriptor, like a file handle, usually 0x00
}rak_httpGetPostFrame;


typedef union {
	uint8                     		rspCode;                    		 // command code response
	uint8 							bootInfo[19];						 //rak开机信息
	rak_mgmtResponse				mgmtResponse;						 //返回中不带参数的判断区
    rak_errorResponse				errorResponse;
	rak_scanResponse          		scanResponse;
	rak_getscanResponse          	getscanResponse;
	rak_qryconResponse              qryconResponse;
    rak_qryrssiResponse             qryrssiResponse;
	rak_ipconfigResponse            ipconfigResponse;			//同DHCP client命令 和 auto_connect 命令返回
	rak_qrydnsResponse              qrydnsResponse;
	rak_pingResponse                pingResponse;
	rak_qryMacFrameRcv  		    qryMacFrameRcv;
	rak_qryFwversionFrameRcv  		qryFwversionFrameRcv;
	rak_socketFrameRcv        		socketFrameRcv;
	rak_recvdataFrame          		recvdataFrame;
	rak_getStoreCfgFrame            StoreCfgFrame;
	rak_getWebCfgFrame			    WebCfgFrame;
	rak_ltcpStatusFrame			    LtcpStatusFrame;
	rak_httpGetPostFrame			HttpGetPostFrame;
	uint8					        uCmdRspBuf[RAK_FRAME_CMD_RSP_LEN + RAK_MAX_PAYLOAD_SIZE];
} rak_uCmdRsp;



extern volatile uint8 read_flag ;
extern rak_uCmdRsp	uCmdRspFrame;

extern rak_api rak_strApi;
int8 * rak_bytes4ToAsciiDotAddr(uint8 *hexAddr,uint8 *strBuf);
void rak_asciiDotAddressTo4Bytes(uint8 *hexAddr, int8 *asciiDotAddress,  uint8 length);
uint16 rak_init_struct(rak_api *ptrStrApi);
#include "rak_uart_api.h"
#include "rak_config.h"
#endif

