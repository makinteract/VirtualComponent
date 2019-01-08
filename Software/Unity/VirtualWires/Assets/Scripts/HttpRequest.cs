using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HttpRequest : MonoBehaviour {
    public QueryResult queryResult;
    public void postJson(string url, string json) {
        //StartCoroutine(PostRequest("http://192.168.4.3", json));
		StartCoroutine(PostRequest(url, json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        Debug.Log("json = " + json);
        var www = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text. UTF8Encoding().GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.chunkedTransfer = false;

        //Send the request then wait here until it returns
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Error While Sending: " + www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            Debug.Log("Received: " + result);
            queryResult.setQueryResult(result);
        }
    }
}