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

	// public UnityAction<JObject> resetValueAction;
	// public UnityAction<JObject> disconnectLeftAction;
	// public UnityAction<JObject> disconnectRightAction;
	// public UpdateJsonEvent resetValueEvent;
	// public UpdateJsonEvent disconnectLeftEvent;
	// public UpdateJsonEvent disconnectRightEvent;

	public StatusButton statusButton;

	public void Start()
    {
		getBoardDataHandlerObject();
        updateJsonAction = new UnityAction<ComponentBase>(updateJsonData);
        updateJsonEvent = new UpdateDataEvent();
        updateJsonEvent.AddListener(updateJsonAction);

		// resetValueAction = new UnityAction<JObject>(resetValue);
        // resetValueEvent = new UpdateJsonEvent();
        // resetValueEvent.AddListener(resetValueAction);

		// disconnectLeftAction = new UnityAction<JObject>(disconnectLeft);
        // disconnectLeftEvent = new UpdateJsonEvent();
        // disconnectLeftEvent.AddListener(disconnectLeftAction);

		// disconnectRightAction = new UnityAction<JObject>(disconnectRight);
        // disconnectRightEvent = new UpdateJsonEvent();
        // disconnectRightEvent.AddListener(disconnectRightAction);
    }

    private void getBoardDataHandlerObject()
    {
        boardDataHandler = GameObject.Find("BoardDataHandler").GetComponent<BoardDataHandler>();
    }

	// public void resetValue(JObject _data)
	// {
	// 	string resetValueString = _data.ToString(Formatting.None) + "\n";
	// 	Debug.Log("[JsonHandler] 1. json to send : " + resetValueString);
	// 	wifi.sendDataEvent.Invoke(resetValueString);
	// 	statusButton.dataWaitingEvent.Invoke(resetValueString);
	// }

	// public void disconnectLeft(JObject _data)
	// {
	// 	string disconnectLeftString = _data.ToString(Formatting.None) + "\n";
	// 	Debug.Log("[JsonHandler] 2. json to send : " + disconnectLeftString);
	// 	wifi.sendDataEvent.Invoke(disconnectLeftString);
	// 	statusButton.dataWaitingEvent.Invoke(disconnectLeftString);
	// }

	// public void disconnectRight(JObject _data)
	// {
	// 	string disconnectRightString = _data.ToString(Formatting.None) + "\n";
	// 	Debug.Log("[JsonHandler] 3. json to send : " + disconnectRightString);
	// 	wifi.sendDataEvent.Invoke(disconnectRightString);
	// 	statusButton.dataWaitingEvent.Invoke(disconnectRightString);
	// }

    public void updateJsonData(ComponentBase _data)
    {
		JObject resultJObjectToSend = _data.changed;

		// if(resultJObjectToSend.GetValue("disconnectAll") != null) {
		// 	resetValue((JObject)resultJObjectToSend.GetValue("resetValue"));
		// 	disconnectLeft((JObject)resultJObjectToSend.GetValue("disconnectLeft"));
		// 	disconnectRight((JObject)resultJObjectToSend.GetValue("disconnectRight"));
		// } else {
			string resultJObjectToSendString = resultJObjectToSend.ToString(Formatting.None) + "\n";
			//Debug.Log("[JsonHandler] json to send : " + resultJObjectToSendString);
			wifi.sendDataEvent.Invoke(resultJObjectToSendString);
			statusButton.dataWaitingEvent.Invoke(resultJObjectToSendString);
		// }

		JObject resultJObjectInternal = buildJsonObjectInternal(_data.type, _data.componentName, _data.id, _data.value, _data.connection);
        boardDataHandler.updateBoardDataEvent.Invoke(resultJObjectInternal);
		_data.changed = new JObject();
    }

	public JObject buildJsonObjectInternal(string _fixed, string _name, int _id, int _value, JArray _connection)
	{
		JObject vcJson = new JObject();

		if(_fixed != "fix") {
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