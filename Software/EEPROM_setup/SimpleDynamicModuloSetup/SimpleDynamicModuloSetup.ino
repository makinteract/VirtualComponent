
#include "Wire.h"

#define ADDR_EEPROM 0x50
#define ADDR_BOARD_ID 0
#define ADDR_COMPONENTS_ADDRESSES 1


// FORWARD DECLARATION
void i2c_eeprom_write_byte( int deviceaddress, unsigned int eeaddress, byte data );
byte i2c_eeprom_read_byte( int deviceaddress, unsigned int eeaddress );

void setup() 
{
  Serial.begin(115200);
  Wire.begin();

  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_BOARD_ID, 101);
  
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 0, 0x2C);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 1, 0x2C);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 2, 0x2E);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 3, 0x2E);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 4, 0x2D);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 5, 0x2D);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 6, 0x2F);
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 7, 0x2F);

  // Print debug
  for (int i = 0; i < 17; i++) Serial.println(i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_BOARD_ID + i));
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
