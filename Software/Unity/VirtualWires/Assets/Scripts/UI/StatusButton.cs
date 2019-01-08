using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;

public class StatusButton : MonoBehaviour {
    public static StatusButton instance;
	public Sprite doneSprite;
	public Sprite waitSprite;
	public Sprite notOkSprite;
	public Image freezeAll;
	public UnityAction<JObject> dataReceivedAction;
	public DataReceivedEvent dataReceivedEvent;
	public UnityAction<string> dataWaitingAction;
	public SendDataEvent dataWaitingEvent;
	public WifiConnection wifi;

	bool status;

	private string command = "";

    void Awake()
    {
        if (StatusButton.instance == null)
            StatusButton.instance = this;
    }
    // Use this for initialization
    void Start()
    {
		status = true;
        //gameObject.SetActive(true);
		dataReceivedAction = new UnityAction<JObject>(done);
        dataReceivedEvent = new DataReceivedEvent();
        dataReceivedEvent.AddListener(dataReceivedAction);

		dataWaitingAction = new UnityAction<string>(wait);
        dataWaitingEvent = new SendDataEvent();
        dataWaitingEvent.AddListener(dataWaitingAction);
    }

	void done(JObject obj) {
		//gameObject.SetActive(true);
		string feedback = (string)obj.GetValue("status");
		status = true;
		gameObject.GetComponent<Button>().image.sprite = doneSprite;
		freezeAll.gameObject.SetActive(false);
	}

	void wait(string str) {
		//Debug.Log("waiting status");
		status = false;
		gameObject.GetComponent<Button>().image.sprite = waitSprite;
		freezeAll.gameObject.SetActive(true);
		// StartCoroutine(ShowReady());
		command = str;
		//Debug.Log(command);
	}

	public void resend() {
		Debug.Log("resend: " + command);
		if(!status)
			wifi.sendDataEvent.Invoke(command);
	}

    // IEnumerator ShowReady()
    // {
    //     int count = 0;
    //     while (count < 3)
    //     {
	// 		// texture 바꾸기
    //         gameObject.SetActive(true);
    //         yield return new WaitForSeconds(0.5f);
    //         gameObject.SetActive(false);
    //         yield return new WaitForSeconds(0.5f);
    //         count++;
    //     }
    // }
}