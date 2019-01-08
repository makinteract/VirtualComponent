using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class AwgHandler : MonoBehaviour {
	public UnityAction<string,string> awgStartAction;
    public SendAwgStartEvent awgStartEvent;
	public HttpRequest http;
	public Sprite onButtonSprite;

	public void Start()
	{
        awgStartAction = new UnityAction<string,string>(awgStart);
        awgStartEvent = new SendAwgStartEvent();
        awgStartEvent.AddListener(awgStartAction);
	}

	void awgStart(string _awgip, string _feedback) {
		//Debug.Log("*****awgStartEvent");
        http.postJson(_awgip, Query.awgRun);
		GameObject.Find("OnOffButton").GetComponent<Button>().image.sprite = onButtonSprite;
    }
}
