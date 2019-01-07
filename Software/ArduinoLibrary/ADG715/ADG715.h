#ifndef ADG715_h
#define ADG715_h

#if (ARDUINO >= 100)
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

#include "Wire.h"	

#define BASE_KEY 0x48 //p12 10010|A1A0

class ADG715
{
	public:
		ADG715(uint8_t address); //set the slave address of device
		void begin();
		void reset();		
		byte read(byte channel); //read all channels and return as a byte
		void all(boolean input);
		void writeChannel(uint8_t channel, byte state); //change status of a specified channel (0-7)
	private:
		int _address; //requestFrom only takes int
		byte _value; //shared value for all functions
		byte CH(int channel);
};

#endif