
#include "Wire.h"

#define ADDR_EEPROM 0x50
#define ADDR_BOARD_ID 0
#define ADDR_COMPONENTS_ADDRESSES 1


// FORWARD DECLARATION
void i2c_eeprom_write_byte( int deviceaddress, unsigned int eeaddress, byte data );
byte i2c_eeprom_read_byte( int deviceaddress, unsigned int eeaddress );


#define BOARD_STATIC_RESISTORS 201
#define BOARD_STATIC_CAPACITORS 202
#define BOARD_STATIC_MIX 203

// chnage this
int BOARD= BOARD_STATIC_RESISTORS;



void setup()
{
  Serial.begin(115200);
  Wire.begin();
  Serial.println("Writing memory");

  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_BOARD_ID, BOARD);
  Serial.println(i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_BOARD_ID ));
}



void loop() {

}



byte i2c_eeprom_read_byte( int deviceaddress, unsigned int eeaddress ) {
  delay(10); // very important a small pause before reading again
  byte rdata = 0xFF;
  Wire.beginTransmission(deviceaddress);
  Wire.write((int)(eeaddress >> 8)); // MSB
  Wire.write((int)(eeaddress & 0xFF)); // LSB
  Wire.endTransmission();
  Wire.requestFrom(deviceaddress, 1);
  if (Wire.available()) rdata = Wire.read();
  return rdata;
}



void i2c_eeprom_write_byte( int deviceaddress, unsigned int eeaddress, byte data )
{
  int rdata = data;
  Wire.beginTransmission(deviceaddress);
  Wire.write((int)(eeaddress >> 8)); // MSB
  Wire.write((int)(eeaddress & 0xFF)); // LSB
  Wire.write(rdata);
  Wire.endTransmission();
  delay(10); // very important a small pause before writing again
}
