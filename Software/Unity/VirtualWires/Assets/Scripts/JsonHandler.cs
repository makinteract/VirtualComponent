using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;

public class JsonHandler : MonoBehaviour {
	private BoardDataHandler boardDataHandler;
	//private string result;
	public WifiConnection wifi;

    public UnityAction<ComponentBase> updateJsonAction;
    public UpdateDataEvent updateJsonEvent;

	public void Start()
    {
		getBoardDataHandlerObject();
        updateJsonAction = new UnityAction<ComponentBase>(updateJsonData);
        updateJsonEvent = new UpdateDataEvent();
        updateJsonEvent.AddListener(updateJsonAction);
    }

    private void getBoardDataHandlerObject()
    {
        boardDataHandler = GameObject.Find("BoardDataHandler").GetComponent<BoardDataHandler>();
    }

    public void updateJsonData(ComponentBase _data)
    {
		JObject resultJObject = buildJsonObject(_data.type, _data.componentName, _data.id, _data.value, _data.connection);
		string jsonResultString = resultJObject.ToString(Formatting.None) + "\n";
		//Debug.Log("[JsonHandler] json to send : " + jsonResultString);
		
		//wifi.sendPost(jsonResultString);
		//wifi.sendData(test());
		//wifi.sendData(jsonResultString);
		wifi.sendDataEvent.Invoke(jsonResultString);

		Debug.Log("[JsonHandler] json to send : " + test());
        boardDataHandler.updateBoardDataEvent.Invoke(resultJObject);
    }

	public JObject buildJsonObject(string _fixed, string _name, int _id, int _value, JArray _connection)
	{
		JObject vcJson = new JObject();

		if(_fixed != "fixed") {
			vcJson.Add("value", _value);
		}
		vcJson.Add("id", _id);
		vcJson.Add("componentType", _name);
		vcJson.Add("connections", _connection);

		return vcJson;
	}

	public string test()
	{
		JObject vcJson = new JObject();

		vcJson.Add("query", "info");

		return vcJson.ToString(Formatting.None);
	}
}