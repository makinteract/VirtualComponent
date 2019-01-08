using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;


public class QueryResult : MonoBehaviour
{
    public ConstraintsHandler constraintHandle;
    public RefreshButton refresh;
    public StatusButton status;
    public AwgHandler awgHandler;
    public Communication comm;
    public Sprite refreshedVoltmeter;
    JObject queryResult;

    public void setQueryResult(string result) {
        Debug.Log("setQueryResult()");
        Debug.Log("result = " + result);
        queryResult = JObject.Parse(result);
        IDictionary<string, JToken> qResult = queryResult;
        if(qResult.ContainsKey("BoardID")) {
            //notify to refresh button
            //it will reload board and reset board name on UI
            //Debug.Log("qResult");
            refresh.dataReceivedEvent.Invoke(queryResult);
            status.dataReceivedEvent.Invoke(queryResult);
        } else if(qResult.ContainsKey("V")) {
            //notify voltage reader object
            //Debug.Log("v");
            GameObject temp = GameObject.Find("ADC6");
            if(temp != null) {
                Util.getChildObject(temp, "ValueText").GetComponent<Text>().text = Util.changeUnit((float)queryResult.GetValue("V"), "ADC");
            }
            if(GameObject.Find("VoltValue")) {
                GameObject.Find("VoltValue").GetComponent<Text>().text = Util.changeUnit((float)queryResult.GetValue("V"), "ADC");
            }
            if(GameObject.Find("ReadVoltageButton")) {
                GameObject.Find("ReadVoltageButton").GetComponent<Button>().image.sprite = refreshedVoltmeter;
            }
            else Debug.Log("object missing");
        } else if(qResult.ContainsKey("Status")) {
            // status button color 바꾸기
            //Debug.Log("status");
            status.dataReceivedEvent.Invoke(queryResult);
            // JObject constraintsInfo = new JObject();
            // constraintsInfo.Add("value", 1);
            // constraintsInfo.Add("name", "com");
            // constraintHandle.handleConstraintsEvent.Invoke(constraintsInfo);
            //constraint invoke //constraintHandle
        } else if(qResult.ContainsKey("awg")) {
            //Debug.Log("****awg");
             //Received: {"awg":{"1":[{"command":"setRegularWaveform","statusCode":2684354573,"wait":0}]}}
            if((string)queryResult["awg"]["1"][0]["command"] == "setRegularWaveform") {
                switch((uint)queryResult["awg"]["1"][0]["statusCode"]) {
                    case 0:
                    //Debug.Log("****setRegularWaveform");
                    awgHandler.awgStartEvent.Invoke(comm.getAwgIP(),result);
                    break;
                }
            }
        }
    }

    public void setQueryResult(string _result, JObject _sentData, string _sourceComponentName) {
        Debug.Log("setQueryResult( " + _result + ", " + _sentData.ToString(Formatting.None) + ", " + _sourceComponentName + " )");
        queryResult = JObject.Parse(_result);
        IDictionary<string, JToken> qResult = queryResult;
        
        if(qResult.ContainsKey("Status")) {
            // status button color 바꾸기
            Debug.Log("status");
            status.dataReceivedEvent.Invoke(queryResult);
            
            if( ((string)_sentData.GetValue("state") == "userRequest") ) {
                JObject constraintsInfo = new JObject();
                constraintsInfo.Add("value", (int)_sentData.GetValue("val"));
                constraintsInfo.Add("name", _sourceComponentName);
                constraintHandle.handleConstraintsEvent.Invoke(constraintsInfo);
            }
        }
    }

    public JObject getQueryResult() {
        return queryResult;
    }
}