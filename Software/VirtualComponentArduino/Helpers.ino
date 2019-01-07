//#define ADDR_EEPROM 0x50
//#define ADDR_BOARD_ID 0
//#define ADDR_COMPONENTS_ADDRESSES 1


byte getBoardID()
{
  return i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_BOARD_ID);
}


byte getComponentAddress (byte id)
{
  return i2c_eeprom_read_byte(ADDR_EEPROM, ADDR_COMPONENTS_ADDRESSES + id);
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

/*
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
*/


