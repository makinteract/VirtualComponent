#include "Arduino.h"
#include "SwitchMatrix.h"

SwitchMatrix::SwitchMatrix (byte pclkPin, byte sclkPin, byte sinPin): pclk(pclkPin), sclk(sclkPin), sin(sinPin)
{
  pinMode(pclk, OUTPUT);
  pinMode(sclk, OUTPUT);
  pinMode(sin, OUTPUT);
  reset();
}


void SwitchMatrix::reset()
{
  memset(connectionSlots, NONE, sizeof(connectionSlots));
}


// x is 0-15, y is 1-30
void SwitchMatrix::connect (byte moduloPin, byte boardPin)
{
  if (moduloPin < 0 || moduloPin > 15) return;
  if (boardPin < 1 || boardPin > 30) return;

  boardPin--; // need to start from 0, not 1
  //clearSelection(boardPin); // one component can only be used once
  connectionSlots[moduloPin] = boardPin;
}

// x is 0-15, y is 1-30
void SwitchMatrix::disconnect (byte moduloPin, byte boardPin)
{
  if (moduloPin < 0 || moduloPin > 15) return;
  if (boardPin < 1 || boardPin > 30) return;

  boardPin--; // need to start from 0, not 1
  connectionSlots[moduloPin] = NONE;
}


bool SwitchMatrix::areConnected (byte moduloPin, byte boardPin)
{
  return connectionSlots[moduloPin] == boardPin;
}

void SwitchMatrix::update()
{
  updateHalf (15); // right side
  updateHalf (0); // left side

  // tail
  delay(1);
  digitalWrite(pclk, LOW);
  delay(1);
  digitalWrite(pclk, HIGH);
}



void SwitchMatrix::updateHalf(byte offsetBoardRow)
{
  for (int boardPin = 15; boardPin >= 0; boardPin--)
  {
    for (int moduloPin = 15; moduloPin >= 0; moduloPin--)
    {
      if (areConnected (moduloPin, offsetBoardRow + boardPin)) digitalWrite(sin, HIGH);
      else digitalWrite(sin, LOW);

      delay(1);
      digitalWrite(sclk, HIGH);
      delay(1);
      digitalWrite(sclk, LOW);
    }
  }
}

void SwitchMatrix::closeAllSwitches()
{
    for (int i=0; i<512; i++)
    {
      digitalWrite(sin, LOW);
      delay(1);
      digitalWrite(sclk, HIGH);
      delay(1);
      digitalWrite(sclk, LOW);
    }
}

void SwitchMatrix::clearSelection (byte boardPin)
{
  for (int i=0; i<16; i++)
  {
    if (connectionSlots[i]==boardPin) connectionSlots[i]=NONE;
  }
}


