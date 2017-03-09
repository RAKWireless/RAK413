#ifndef _RAKCONFIG_H_
#define _RAKCONFIG_H_

/*==============================================*/
/**
 * Global Defines
 */
#define RAK_POWER_LEVEL_0		  0
#define RAK_POWER_LEVEL_1		  1
#define RAK_POWER_LEVEL_2		  2
#define RAK_POWER_LEVEL_3		  3


#define RAK_AP_STATION_MODE        1				  //0 station  1 AP	 2 ADHOC

#define RAK_SCAN_SSID			    "TP-LINK_2.4GHz"				   //@ null string ("") scans all ssids
#define RAK_CONNECT_SSID		    "TP-LINK_2.4GHz"			   //@ SSID to join to in 2.4GHz 
#define RAK_SCAN_CHANNEL		    0				              //@ 0 scans all channels	1-11

#define RAK_GET_SCAN_NUM		     3			   //@ 0 get scan result number

#define RAK_PSK				         "lthonway303550"                //"1234567890"			   //@ If we are using WPA2, this is the key
#define RAK_CREAT_CHANNEL		       9
/* Set AP  CONFIG */         	
#define RAK_CREAT_AP_ADHOC_SSID       "RAK410_UART_TEST" 		  
#define RAK_AP_BRODCAST_ENABLE         0                           //  0: no hidden	 1:hidden

 /* Set IP  CONFIG */
#define RAK_IPDHCP_MODE_ENABLE			0	   			  // station 0:dhcp 1:static
#define RAK_IPSTATIC_IP_ADDRESS			"192.168.1.10"
#define RAK_IPSTATIC_GATEWAY			 "192.168.1.1"	
#define RAK_IPSTATIC_NETMASK			 "255.255.255.0"		
#define RAK_IPSTATIC_DNS1				  "0"
#define RAK_IPSTATIC_DNS2				  "0"

#define RAK_DOMAIN_NAME            "www.lthonway.com"                     //@ set the domain name for which dns is requested
    
#endif
