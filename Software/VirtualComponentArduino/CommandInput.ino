class CommandInput
{
  public:
    CommandInput (const SwitchMatrix& mt);
    bool updateInput (char inChar);
    void executeCommand (const String& str, Stream* stream);

  private:
    void handleBoardInfo(const JsonObject& in, Stream* stream);
    void handleConnection(const JsonObject& in, Stream* stream);
    void handleDoubleConnection(const JsonObject& in, Stream* stream);
    void handleResistor(const JsonObject& in, Stream* stream);
    void handleCapacitor(const JsonObject& in, Stream* stream);
    void handleInductor(const JsonObject& in, Stream* stream);
    void handleReset (const JsonObject& in, Stream* stream);
    void readVoltage(const JsonObject& in, Stream* stream);

    SwitchMatrix m;
    String input;
};


CommandInput::CommandInput(const SwitchMatrix& mt): m(mt), input("") {}


bool CommandInput::updateInput (char inChar)
{
  if (inChar == '\n') return true;
  input += inChar;
  if (input.length() > JSON_BUFFER_SIZE) return true;
  return false;
}

// check this code here
void CommandInput::executeCommand (const String& str, Stream* stream)
{
  StaticJsonBuffer<JSON_BUFFER_SIZE> inBuffer;
  JsonObject& root = inBuffer.parseObject(input);

  if (!root.success()) stream->println(F("{\"Status\":\"Parse ERR\"}"));
  else if (root["query"] == "Info") handleBoardInfo (root, stream); // info
  else if (root["query"] == "V") readVoltage (root, stream); // voltage
  else if (root["set"] == "Rst") handleReset (root, stream); // rset
  else if (root["set"] == "X") handleConnection (root, stream); // connection
  else if (root["set"] == "X2") handleDoubleConnection (root, stream); // connection
  else if (root["set"] == "R") handleResistor (root, stream); // res
  else if (root["set"] == "C") handleCapacitor (root, stream); // cap
  else if (root["set"] == "L") handleInductor (root, stream); // inductor

  // clean input
  input = "";
}

void CommandInput::handleBoardInfo(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Info"));
#endif
  stream->print(F("{\"BoardID\":\""));
  stream->print(getBoardID());
  stream->println(F("\"}"));
}

void CommandInput::handleReset(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Reset"));
#endif
  m.closeAllSwitches();
  stream->println(F("{\"Status\":\"Reset\"}"));
}

void CommandInput::handleConnection(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Connection"));
#endif
  boolean on = in["on"] == 1;
  int moduloPin = in["M"];
  int boardPin = in["B"];

  if (on)
  {
    m.connect(moduloPin, boardPin);
  } else {
    m.disconnect(moduloPin, boardPin);
  }
  m.update();

  stream->println(F("{\"Status\":\"Ok\"}"));
}


void CommandInput::handleDoubleConnection(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Connection"));
#endif

  JsonArray& connections = in["value"];
  int boardPin0 = connections[0]["B"];
  int moduloPin0 = connections[0]["M"];
  int boardPin1 = connections[1]["B"];
  int moduloPin1 = connections[1]["M"];

  boolean on = in["on"] == 1;

  if (on)
  {
    m.connect(moduloPin0, boardPin0);
    m.connect(moduloPin1, boardPin1);
  } else {
    m.disconnect(moduloPin0, boardPin0);
    m.disconnect(moduloPin1, boardPin1);
  }
  m.update();

  stream->println(F("{\"Status\":\"Ok\"}"));
}

void CommandInput::handleResistor(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Resistor"));
#endif
  byte id = in["id"];
  int value = in["val"];

  if (id < 0 || id > 7 || value < 0 || value > 255)
  {
    stream->println(F("{\"Status\":\"ERR\"}"));
    return;
  }

  byte addr = getComponentAddress(id);
  AD524X AD01(addr);
  AD01.write(id % 2, value); // 0 or 1 for each resistor

  stream->println(F("{\"Status\":\"Ok\"}"));
}

void CommandInput::handleCapacitor(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Capacitor"));
#endif
  byte id = in["id"];
  byte value = in["val"];
  byte addr = getComponentAddress(id);

  if (id < 0 || id > 7)
  {
    stream->println(F("{\"Status\":\"ERR\"}"));
    return;
  }

  ADG715 adg(addr);
  adg.begin(); delay(1);

  for (int i = 0; i < 8; i++)
  {
    if (value >> i & 1) adg.writeChannel(i + 1, 1); // close switch (1-8)
    else adg.writeChannel(i + 1, 0); // open switch (1-8)
  }
  stream->println(F("{\"Status\":\"Ok\"}"));
}

void CommandInput::handleInductor(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Inductor"));
#endif
  byte id = in["id"];
  byte value = in["val"];
  byte addr = getComponentAddress(id);

  if (id < 0 || id > 7)
  {
    stream->println(F("{\"Status\":\"ERR\"}"));
    return;
  }

  ADG715 adg(addr);
  adg.begin(); delay(1);
  adg.all(true); delay(1);

  for (int i = 0; i < 8; i++)
  {
    if (value >> i & 1) adg.writeChannel(i + 1, 0); // open switch to use inductor (1-8)
    else adg.writeChannel(i + 1, 1); // close switch to avoid inductor (1-8)
  }
  stream->println(F("{\"Status\":\"Ok\"}"));
}


void CommandInput::readVoltage(const JsonObject& in, Stream* stream)
{
#ifdef DEBUG
  Serial.println(F("Voltage"));
#endif
  byte id = in["id"];
  byte addr = getComponentAddress(id);

  if (id < 0 || id > 7)
  {
    stream->println(F("{\"Status\":\"ERR\"}"));
    return;
  }

  long data = 0;
  Wire.requestFrom(int(addr), 3);
  while (Wire.available()) // ensure all the data comes in
  {
    data = Wire.read();
    data = data << 8;
    data +=  Wire.read();
    Wire.read();
  }

  float voltage = data * 2048 / 32768; // in millivolt
  if (voltage > 2048) voltage = voltage - 2048 * 2;

  stream->print(F("{\"V\":"));
  stream->print(voltage);
  stream->println(F("}"));

  //Serial.println(freeMemory());
}
