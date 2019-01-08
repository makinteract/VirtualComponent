using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void pause() {
		VuforiaRenderer.Instance.Pause(true);
		gameObject.SetActive(true);
	}

	public void play() {
		VuforiaRenderer.Instance.Pause(false);
		//Button temp = GetComponent<Button>();
		//temp.enabled = false;
		//temp.image.CrossFadeAlpha(0,2,false);
		gameObject.SetActive(false);
	}
}