// GLOBALS
SwitchMatrix matrix(PCLK_PIN, SCLK_PIN, SIN_PIN);
CommandInput parser (matrix);

void setup()
{
  // Serial setup
  Serial.begin(115200);
  Wire.begin();

#ifdef WIFI_INPUT
  initWifi();
#endif

  // Infinite loop if BoardID not valid
  if (getBoardID() < 100) while (true);

  // Starting up
  matrix.closeAllSwitches();
  pinMode(LED, OUTPUT);
  digitalWrite(LED, HIGH);
}



void loop()
{
#ifdef WIFI_INPUT

  WiFiEspClient client = server.available();

  if (client) {
    digitalWrite(LED, LOW);

    while (client.connected()) {
      if (client.available()) {
        char inChar = client.read();
        if (parser.updateInput (inChar)) {
          parser.executeCommand ("", &client);
          client.stop(); // it seems to improve reliability if we disconnect
        }
      }
    }
  } else {
    digitalWrite(LED, HIGH);
  }


#else

  while (Serial.available()) {
    char inChar = (char)Serial.read();
    if (parser.updateInput (inChar)) {
      parser.executeCommand("", &Serial);
    }
  }

#endif
}






