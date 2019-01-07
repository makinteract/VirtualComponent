#ifdef WIFI_INPUT

#include "WiFiEsp.h"
#include "SoftwareSerial.h"


SoftwareSerial softwareSerial (3, 2); // RX, TX
WiFiEspServer server (PORT_NUMBER);
int status = WL_IDLE_STATUS;


void initWifi()
{
  softwareSerial.begin(WIFI_BAUD_RATE);
  WiFi.init(&softwareSerial);

  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println(F("WiFi not present"));
    while(true){}
    return;
  }

  // set IP address
  IPAddress localIp(192, 168, 4, 1);
  WiFi.configAP(localIp);

  // start access point
  status = WiFi.beginAP(SSID, 10, PASSWORD, ENC_TYPE_WPA2_PSK);
  server.begin();
}

#endif






