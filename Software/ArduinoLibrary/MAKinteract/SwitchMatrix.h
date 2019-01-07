/*
  SwitchMatrix.h - Library for controlling AD75019 Crosspoint switch pairs
  Created by Andrea Bianchi, April 16 2018.
*/

#ifndef SwitchMatrix_h
#define SwitchMatrix_h

#include "Arduino.h"


class SwitchMatrix
{
  private:
    byte pclk, sclk, sin;
    byte connectionSlots [16];  
    static const byte NONE= 0xFF;  

    bool areConnected (byte moduloPin, byte boardPin);
    void updateHalf (byte offsetBoardRow);
    void clearSelection (byte boardPin);

  public:
    SwitchMatrix (byte pclkPin, byte sclkPin, byte sinPin);
    void reset();
    void connect (byte moduloPin, byte boardPin);
    void disconnect (byte moduloPin, byte boardPin);
    void update ();
    void closeAllSwitches();
};

#endif