using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WifiConnection : MonoBehaviour {
    public QueryResult queryResult;
    bool socketReady = false;                // global variables are setup here
    TcpClient unityClient;
    public NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    public string Host = "192.168.4.1";
    public int Port = 5204;

    bool start = false;

    private JObject sendJsonData;

    public UnityAction<string> sendDataAction;
    public SendDataEvent sendDataEvent;

    void Update() {
        if(start) {
            getData();
        }
    }

    void Start() {
        //closeSocket();
        //setupSocket();
        sendDataAction = new UnityAction<string>(wifiThread);
        sendDataEvent = new SendDataEvent();
        sendDataEvent.AddListener(sendDataAction);
    }

    void Close() {
        closeSocket();
    }

    public void wifiThread(string jsonString)
    {
        Thread clientReceiveThread = new Thread(new ParameterizedThreadStart(sendData));
        clientReceiveThread.IsBackground = true; 			
        clientReceiveThread.Start(jsonString); 
    }

    private void sendData(object jsonString)
    {
        sendData((string) jsonString);
    }

    private void saveSentData(string data) {
        sendJsonData = JObject.Parse(data);
    }

    public void sendData(string jsonString)
    {
        saveSentData(jsonString);

        JObject temp = JObject.Parse((string)jsonString);
        temp.Remove("state");
        jsonString = temp.ToString(Formatting.None)+"\n";

        Debug.Log("json to send from final spot = " + jsonString);

        closeSocket();
        setupSocket();
        while(true) {
            if(theStream.CanRead) break;
        }
        //string dataString = jsonString + "\n";
        writeSocket(jsonString);
        start = true;
        // Thread clientReceiveThread = new Thread(new ThreadStart(getData));
        // clientReceiveThread.IsBackground = true; 			
        // clientReceiveThread.Start();  
    }

    public void getData()
    {
        string name = "";
        string receivedData = readSocket();
        if( (receivedData != "NoData") && (receivedData != "NotReady") ) {
            Debug.Log("getData() received = " + receivedData);

            if((string)sendJsonData.GetValue("set") == "R") {
                name = "resistor" + (int)sendJsonData.GetValue("id");
                queryResult.setQueryResult(receivedData, sendJsonData, name);
            } else if((string)sendJsonData.GetValue("set") == "C") {
                name = "capacitor" + (int)sendJsonData.GetValue("id");
                queryResult.setQueryResult(receivedData, sendJsonData, name);
            } else {
                queryResult.setQueryResult(receivedData);
            }
            
            closeSocket();
            start = false;
        }
    }

    public void setupSocket() {
        Debug.Log("setupSocket");
        unityClient = new TcpClient(Host, Port);
        theStream = unityClient.GetStream();
        theWriter = new StreamWriter(theStream);
        theReader = new StreamReader(theStream);
        socketReady = true;
    }
    
    public void writeSocket(string data) {
        if (!socketReady) {
            Debug.Log("writeSocket not ready");
            return;
        }
        theWriter.Write(data);
        theWriter.Flush();
    }
    
    public string readSocket() {
        if (!socketReady) {
            Debug.Log("readSocket not ready");
            return "NotReady";
        }
        if (theStream.DataAvailable) {
            string result = theReader.ReadLine();
            theStream.Flush();
            return result;
        }
        return "NoData";
    }
    
    public void closeSocket() {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        unityClient.Close();
        socketReady = false;
    }
    
    public void maintainConnection() {
        if(!theStream.CanRead) {
            setupSocket();
        }
    }
}