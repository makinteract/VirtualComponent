using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;

public class ResistorConstraints : MonoBehaviour {
	public JsonHandler jsonHandler;

	public void handler(float value, string source, string target) {
		Component sourceComponent = GameObject.Find(source).GetComponent<Component>();
		Component targetComponent = GameObject.Find(target).GetComponent<Component>();
		ComponentBase srcData = sourceComponent.getComponentData();
		ComponentBase data = targetComponent.getComponentData();
		if(target.Contains("resistor")) {
			data.changed.Add("set", "R");
			data.changed.Add("id", data.id);
			data.changed.Add("val", (int)value);
			data.value = (int)value;
			jsonHandler.updateJsonEvent.Invoke(data);

			Resistor resistor = (Resistor) data;
			int convertedValue = resistor.minValue + (int)value * resistor.stepResistance;
			Util.getChildObject(target, "ValueText").GetComponent<Text>().text = Util.changeUnit(convertedValue, "resistor");
		} else if(target.Contains("capacitor")) {
			if( (GameObject.Find("FrequencyConstraintsValue").activeSelf) && (GameObject.Find("SliderValuePanelTitle").GetComponent<Text>().text == source) ) {
				Resistor resistor = (Resistor) srcData;
				int convertedValue = resistor.minValue + (int)value * resistor.stepResistance;
				//Debug.Log(Util.getChildObject(target, "ValueText").GetComponent<Text>().text);
				string targetValueText = Util.getChildObject(target, "ValueText").GetComponent<Text>().text;
				double targetValue = double.Parse(targetValueText.Substring(0, targetValueText.Length-2));
				if(targetValueText.EndsWith("p"))
					targetValue *= 0.000000000001;
				else if(targetValueText.EndsWith("n"))
					targetValue *= 0.000000001;
				else if(targetValueText.EndsWith("u"))
					targetValue *= 0.000001;
				float RCFomulaResult = RCFormula(convertedValue, targetValue);
				Text FrequencyConstraintsValue = GameObject.Find("FrequencyConstraintsValue").GetComponent<Text>();
				if(RCFomulaResult == -1) FrequencyConstraintsValue.text = "infinity";
				else FrequencyConstraintsValue.text = Util.changeUnit(RCFormula(convertedValue, targetValue)*1000, "frequency");
				//GameObject.Find("FrequencyConstraintsValue").GetComponent<Text>().text = Util.changeUnit(RCFormula(convertedValue, targetValue)*1000, "frequency");
			}
		}
	}

	public float RCFormula(float ResistorValue, double CapacitorValue) {
		//Debug.Log("ResistorValue = " + ResistorValue);
		//Debug.Log("CapacitorValue = " + CapacitorValue);
		float result = -1;
		if(ResistorValue != 0 && CapacitorValue != 0)
			result = (float)(1 / (Math.PI * 2 * ResistorValue * CapacitorValue));
		return result;
	}
}
