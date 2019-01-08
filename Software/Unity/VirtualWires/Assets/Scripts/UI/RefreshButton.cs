using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;

public class RefreshButton : MonoBehaviour {
	public BoardUI boardUI;
	public Sprite DefaultPinSprite;
	public WifiConnection wifi;
	public UnityAction<JObject> dataReceivedAction;
	public DataReceivedEvent dataReceivedEvent;
	public PauseButton pauseButton;
	public Communication comm;
	public StatusButton status;
	public ConstraintsHandler constraintsHandle;

	private DeleteConfirmPanel refreshConfirmPanel;
	private UnityAction refreshYesAction;
    private UnityAction refreshCancelAction;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(true);
		dataReceivedAction = new UnityAction<JObject>(reloadBoard);
        dataReceivedEvent = new DataReceivedEvent();
        dataReceivedEvent.AddListener(dataReceivedAction);
	}
	
	void Awake() {
		refreshConfirmPanel = DeleteConfirmPanel.Instance();
        refreshYesAction = new UnityAction (refresh);
        refreshCancelAction = new UnityAction (refreshCancel);
	}

	public void refreshCancel() {
		//Debug.Log("Cancel refresh");
	}

	public void reloadBoard(JObject queryResult) {
		Debug.Log("reloadBoard()");
		//after get board name, load the board json from internal storage.
		string boardID = "";
		string path = "";

		boardID = (string)queryResult.GetValue("BoardID");
		path = @"/storage/emulated/0/Android/data/com.kaist.virtualcomponent/files/Json/"+ boardID + ".json";
		Debug.Log(path);
		boardUI.setupBoard(path);

		//send command to reset all connection
		wifi.sendDataEvent.Invoke(Query.resetAllConnections);
		gameObject.SetActive(true);
		VuforiaRenderer.Instance.Pause(false);
	}
	
	public void refresh() {
		gameObject.SetActive(false);
		ResetAllComponents();
		pauseButton.play();
		comm.setPopupState(false);
		constraintsHandle.clearConstraintsDB();
		//send query to get board name
		wifi.sendDataEvent.Invoke(Query.getBoardID);
		status.dataWaitingEvent.Invoke(Query.getBoardID);
	}

	// popup을 띄우는 함수를 바로 콜하자
	public void refreshConfirmWindow() {
		refreshConfirmPanel.Choice (refreshYesAction, refreshCancelAction);
        refreshConfirmPanel.setTitle("Reload your initial board?");
        refreshConfirmPanel.setPosition(new Vector3(transform.position.x, transform.position.y-100, transform.position.z));
	}

	//for test
	// public void refresh() {
	// 	ResetAllComponents();
	// 	pauseButton.play();
	// 	string path = @"/storage/emulated/0/Android/data/com.kaist.virtualcomponent/files/Json/102.json";
	// 	boardUI.setupBoard(path);
	// }

	private void ResetAllComponents() {
		GameObject[] pinsTemp = GameObject.FindGameObjectsWithTag("pin");
		foreach(GameObject pinObj in pinsTemp) {
			pinObj.GetComponent<Button>().image.sprite = DefaultPinSprite;;
		}

		GameObject[] temp = GameObject.FindGameObjectsWithTag("component");
        foreach(GameObject componentObj in temp)
        {
			//GameObject pinTemp = null;
			//pinTemp = GameObject.Find(componentObj.GetComponent<Component>().left.BoardPin);
			//pinTemp.GetComponent<Button>().image.sprite = defaultBoardPinSprite;
			GameObject[] wireTemp = GameObject.FindGameObjectsWithTag("wire");
			foreach(GameObject wireObj in wireTemp)
			{
				if( wireObj.name.Contains(componentObj.name) )
				{
					LineRenderer lr = wireObj.GetComponent<LineRenderer>();
					lr.enabled = false;
					//lr.SetVertexCount(0);
					lr.positionCount = 0;
					Destroy(wireObj);
				}
			}
        }

		foreach(GameObject componentObj in temp)
        {
			Destroy(componentObj);
        }
	}
}