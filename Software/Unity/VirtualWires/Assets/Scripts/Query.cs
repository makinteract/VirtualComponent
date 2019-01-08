public static class Query
{
    public const string getBoardID = "{\"query\": \"Info\"}\n";
    public const string getVoltage = "{\"query\": \"V\", \"id\": 6}\n";
    public const string resetAllConnections = "{\"set\": \"Rst\"}\n";
	public const string awgRun = "{ \"awg\":{ \"1\":[ { \"command\":\"run\" } ] } }\r\n";
	public const string awgStop = "{ \"awg\":{ \"1\":[ { \"command\":\"stop\" } ] } }\r\n";
	public const string awgCommand = "{ \"awg\":{ \"1\":[ { \"command\":\"setRegularWaveform\", \"signalType\":\"sine\", \"signalFreq\":100000, \"vpp\":3000, \"vOffset\":0, \"dutyCycle\": 50 } ] } }\r\n";
}

/*
// COMMANDS

// 1) Get Board ID 
{"query": "Info"}
// Feedback example: {"BoardID":"102"}
// 255 means no board is connected


// 2) Get Voltage
{"query": "V", "id": 6}
// Feedback example: {"V":0.00}


// 3) Reset all connections
{"set": "Rst"}
// Feedback example: {"Status":"Reset"}


// 4) Set connection [on is 0 or 1]
{"set":"X", "on": 1, "val":[{"M":0,"B":1},{"M":1,"B":2}]}
// Feedback example: {"Status":"Ok"}


// 5) Set resistor value [0-255]
{"set":"R", "id": 0, "val": 0}
// Feedback example: {"Status":"Ok"}


// 6) Set capacitor value [0-255]
{"set":"C", "id": 2, "val": 0}
// Feedback example: {"Status":"Ok"}


// 7) Set inductor value [0-255]
{"set":"L", "id": 5, "val": 0}
// Feedback example: {"Status":"Ok"}




// TESTS


// TEST RES 1
{"set":"X", "on": 1, "val":[{"M":0,"B":1},{"M":1,"B":2}]}
{"set":"R", "id": 0, "val": 0}

// TEST RES 2
{"set":"X", "on": 1, "val":[{"M":2,"B":1},{"M":3,"B":2}]}
{"set":"R", "id": 1, "val": 0}

// TEST CAP 1
{"set":"X", "on": 1, "val":[{"M":4,"B":1},{"M":5,"B":2}]}
{"set":"C", "id": 2, "val": 0}

// TEST CAP 2
{"set":"X", "on": 1, "val":[{"M":6,"B":1},{"M":7,"B":2}]}
{"set":"C", "id": 3, "val": 0}

// TEST INDUCTOR 1
{"set":"X", "on": 1, "val":[{"M":8,"B":1},{"M":9,"B":2}]}
{"set":"L", "id": 4, "val": 0}

// TEST INDUCTOR 2
{"set":"X", "on": 1, "val":[{"M":10,"B":1},{"M":11,"B":2}]}
{"set":"L", "id": 5, "val": 0}

// TEST VOLTAGE
{"set":"X", "on": 1, "val":[{"M":12,"B":1},{"M":13,"B":2}]}
{"query": "V", "id": 6}

// FUNCTION GENERATOR
{"set":"X", "on": 1, "val":[{"M":14,"B":10},{"M":15,"B":11}]}
*/