using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine.Events;

public class WifiConnection : MonoBehaviour {
    bool socketReady = false;                // global variables are setup here
    TcpClient unityClient;
    public NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    public string Host = "192.168.4.1";
    public int Port = 5204;

    bool start = false;

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
        clientReceiveThread.Start(); 
    }

    private void sendData(object jsonString)
    {
        sendData((string) jsonString);
    }

    public void sendData(string jsonString)
    {
        Debug.Log("sendData");
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
        string recievedData = readSocket();
        if( (recievedData != "NoData") && (recievedData != "NotReady") ) {
            Debug.Log("getData() recieved = " + recievedData);
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

    // Use this for initialization
    /*
    string url = "http://192.168.4.1:5204";

    public void sendPost(string jsonString)
    {
        StartCoroutine(SendPostCoroutine(jsonString));
    }

    IEnumerator SendPostCoroutine(string vwData)
    {
        Debug.Log("json result: " + vwData);
        
        using (UnityWebRequest www = UnityWebRequest.Post(url, vwData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError) {
                Debug.Log("network error");
                Debug.Log(www.error);
            } else {
                Debug.Log("POST successful!");
                StringBuilder sb = new StringBuilder();
                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
            }
        }        
    }
    */

/*
    

        IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
 
        yield return request.SendWebRequest();
 
        Debug.Log("Status Code: " + request.responseCode);
    }

	private string result;
	// Use this for initialization
	void Start () {
		result = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
    public void sendData(string jsonString)
    {
        StartCoroutine(Upload(jsonString));
    }

    public void recvData()
    {
        StartCoroutine(Download());
    }

    IEnumerator Upload(string vwData)
    {
        Debug.Log("json result: " + vwData);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(vwData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://192.168.4.1:5204", data))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("feedback");
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }

	IEnumerator Download() {
		string address = "http://192.168.4.1:5204";
		WWW www = new WWW(address);
		yield return www;
		result = www.text;
		Debug.Log("Download() result = " + result);
	}

	public string getJsonDataFromVC() {
		return result;
	}

    // void GetData()
    // {
    //     //sending the request to url
    //     WWW www = new WWW(Url);
    //     StartCoroutine("GetdataEnumerator", Url);
    // }
    IEnumerator GetdataEnumerator(WWW www)
    {
        //Wait for request to complete
        yield return www;
        if (www.error != null)
        {
            string feedback = www.text;
            //Data is in json format, we need to parse the Json.
            Debug.Log(feedback);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
*/
	/*
    IEnumerator Download()
    {
        byte[] vwData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.4.1:80");
        //yield return www.SendWebRequest();

        www.SetRequestHeader("accept", "application/json; charset=UTF-8");
        www.SetRequestHeader("content-type", "application/json; charset=UTF-8");

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Download complete!");
        }
    }

    IEnumerator ReqestTrasData(string method)
    {
        UnityWebRequest request;

        switch (method)
        {
            case "POST":
            case "PATCH":
            // Defaults are fine for PUT
            case "PUT":
                byte[] bytes = Encoding.UTF8.GetBytes(requestBodyJsonString);
                request = UnityWebRequest.Put(url, bytes);
                request.SetRequestHeader("X-HTTP-Method-Override", method);
                break;

            case "GET":
                // Defaults are fine for GET
                request = UnityWebRequest.Get(url);
                break;

            case "DELETE":
                // Defaults are fine for DELETE
                request = UnityWebRequest.Delete(url);
                break;

            default:
                throw new Exception("Invalid HTTP Method");
        }

        request.SetRequestHeader("accept", "application/json; charset=UTF-8");
        request.SetRequestHeader("content-type", "application/json; charset=UTF-8");
    } */
}