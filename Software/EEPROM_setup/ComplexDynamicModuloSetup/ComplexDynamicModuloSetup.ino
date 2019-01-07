
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
  Serial.println("Writing memory");
  
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_BOARD_ID, 102); // BOARD ID IS 102
  delay(100);
  Serial.println("Board id:");
  Serial.println(i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_BOARD_ID));
  
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 0, 0x2C); // Resistor  1 (X0 - X1)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 1, 0x2C); // Resistor  2 (X2 - X3)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 2, 0x48); // Capacitor 1 (X4 - X5)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 3, 0x49); // Capacitor 2 (X6 - X7)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 4, 0x4A); // Inductor  1 (X8 - X9)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 5, 0x4B); // Inductor  2 (X10 - X11)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 6, 0x4F); // ADC (X12 - X13)
  i2c_eeprom_write_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + 7, 0);    // OpenScope (X14-X15)

  // Print debug
  Serial.println("All");
  for (int i = 0; i < 8; i++) Serial.println(i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_BOARD_ID + i));
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
