using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinusButton : MonoBehaviour {
	public Slider frequencySlider; 
	public Slider amplitudeSlider;
	public Slider dcOffsetSlider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void minusValue() {
		switch(transform.parent.name) {
			case "FrequencySlider":
			frequencySlider.value -= 100;
			break;
			case "AmplitudeSlider":
			amplitudeSlider.value -= 100;
			break;
			case "DCOffsetSlider":
			dcOffsetSlider.value -= 100;
			break;
		}
	}
}
