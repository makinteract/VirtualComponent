using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlusButton : MonoBehaviour {

	public Slider frequencySlider; 
	public Slider amplitudeSlider;
	public Slider dcOffsetSlider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void plusValue() {
		switch(transform.parent.name) {
			case "FrequencySlider":
			frequencySlider.value += 100;
			Debug.Log("FrequencySlider value = " + frequencySlider.value);
			break;
			case "AmplitudeSlider":
			amplitudeSlider.value += 100;
			Debug.Log("AmplitudeSlider value = " + amplitudeSlider.value);
			break;
			case "DCOffsetSlider":
			dcOffsetSlider.value += 100;
			Debug.Log("DCOffsetSlider value = " + dcOffsetSlider.value);
			break;
		}
	}
}
