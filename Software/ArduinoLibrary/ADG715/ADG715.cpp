#include "ADG715.h"
//LOOK INTO ARDUINO bitWrite, bitRead, bitClear, bitSet

//set address of slave device and join I2C bus as master
ADG715::ADG715(byte address)
{
	_address = BASE_KEY | address;
};

void ADG715::begin() 
{
	Wire.begin();
	reset(); //clear out register
}

//idiot proofing channel selection
byte ADG715::CH(int channel)
{
	channel -= 1; //assume input is 1-8 -> 0-7
	if(channel < 0)
		channel = 0;
	else if(channel > 7)
		channel = 7;
	return channel;
};

//return status as a byte of all channel (1-8)
byte ADG715::read(byte channel) //if channel exceeds 9, read all
{
	byte value = 255; //error possibly?
	Wire.requestFrom(_address, 0x01); //request one byte from address
	while(Wire.available())
		value = Wire.read(); //grab one byte
	if(channel < 9) //1-8 
	{
		channel = CH(channel); //resize to 0-7	
		value = bitRead(value, channel);
	}
	return value; //return all	
};

void ADG715::all(boolean input) //255 for on, 0 for off
{
	byte output = 0x00;
	if(input)
		output = 0xFF;
	Wire.beginTransmission(_address);
	Wire.write(output);
	Wire.endTransmission();		
};

//change status of a specified channel (1-8)
void ADG715::writeChannel(byte channel, byte state)
{
	byte value;
	value = read(9); //read all channels
	bitWrite(value, CH(channel), state);
	Wire.beginTransmission(_address);
	Wire.write(value);
	Wire.endTransmission();	
};

void ADG715::reset()
{
    Wire.beginTransmission(_address);
    Wire.write(0x00); //clear out register
    Wire.endTransmission();
};