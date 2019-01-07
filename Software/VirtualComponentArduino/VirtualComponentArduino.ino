// Libs
#include <SwitchMatrix.h>
#include <ArduinoJson.h>
#include "Wire.h"
#include "AD524X.h"
#include "ADG715.h"

// Flags
#define DEBUG 1
#define WIFI_INPUT 1


// Const
#define JSON_BUFFER_SIZE 160


// WIFI GLOBALS
// Board ip 192.168.4.1
// Port: 5204
// Password: 12345678
char SSID[] = "VrComp1";
char PASSWORD[] = "12345678";
#define PORT_NUMBER 5204
#define WIFI_BAUD_RATE 57600


// EEPROM
#define ADDR_EEPROM 0x50
#define ADDR_BOARD_ID 0
#define ADDR_COMPONENTS_ADDRESSES 1


// Pins
#define LED 4
#define PCLK_PIN 11
#define SCLK_PIN 12
#define SIN_PIN 13


// FUNCTION FORWARD DECLARATIONS
byte i2c_eeprom_read_byte( int deviceaddress, unsigned int eeaddress );
byte getBoardID();
byte getComponentAddress (byte id);



#ifdef __arm__
// should use uinstd.h to define sbrk but Due causes a conflict
extern "C" char* sbrk(int incr);
#else  // __ARM__
extern char *__brkval;
#endif  // __arm__
 
int freeMemory() {
  char top;
#ifdef __arm__
  return &top - reinterpret_cast<char*>(sbrk(0));
#elif defined(CORE_TEENSY) || (ARDUINO > 103 && ARDUINO != 151)
  return &top - __brkval;
#else  // __arm__
  return __brkval ? &top - __brkval : &top - __malloc_heap_start;
#endif  // __arm__
}



