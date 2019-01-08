using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;

public class BoardDataHandler : MonoBehaviour {
	JObject board;
    public UnityAction<JObject> updateBoardDataAction;
    public UpdateJsonEvent updateBoardDataEvent;

	public void Start()
    {
        updateBoardDataAction = new UnityAction<JObject>(updateBoardData);
        updateBoardDataEvent = new UpdateJsonEvent();
        updateBoardDataEvent.AddListener(updateBoardDataAction);
    }

    public void updateBoardData(JObject _data)
    {
		int updatedComponent = 0;
		Debug.Log("[BoardDataHandler] invoked - numberOfComponents" + (int)board["numberOfComponents"]);
		for(int i = 0; i < (int)board["numberOfComponents"]; i++) {
			if((int)board["components"][i]["id"] == (int)_data.GetValue("id"))
			{
				if(_data.Property("value") != null) {
					board["components"][i]["value"] = _data.GetValue("value");
				}
				board["components"][i]["connections"] = _data.GetValue("connections");
				updatedComponent = i;
			}
		}
		Debug.Log("Board Json : " + board["components"][updatedComponent].ToString());
    }

	void Update () {
		
	}

	public void setBoardJson(JObject _board)
	{
		board = (JObject)_board.DeepClone();
	}

	public JObject getBoardJson()
	{
		return board;
	}

	public string getBoardName() {
		string name = (string)board.GetValue("boardName");
		return name;
	}

	public string getBoardId() {
		string id = (string)board.GetValue("boardID");
		return id;
	}
}