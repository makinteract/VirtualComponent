using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;

public class CapacitorConstraints : MonoBehaviour {
	public JsonHandler jsonHandler;
	public void handler(float value, string source, string target) {
		//Debug.Log("** handler CapacitorConstraints Class");
		Component sourceComponent = GameObject.Find(source).GetComponent<Component>();
		Component targetComponent = GameObject.Find(target).GetComponent<Component>();
		ComponentBase srcData = sourceComponent.getComponentData();
		ComponentBase data = targetComponent.getComponentData();
		// if(target.Contains("capacitor")) {
		// 	data.changed.Add("set", "C");
		// 	data.changed.Add("id", data.id);
		// 	data.changed.Add("val", (int)value);
		// 	data.value = (int)value;
		// 	jsonHandler.updateJsonEvent.Invoke(data);

		// 	Capacitor capacitor = (Capacitor) data;
		// 	//int convertedValue = resistor.minValue + (int)value * resistor.stepResistance;
		// 	//Util.getChildObject(target, "ValueText").GetComponent<Text>().text = Util.changeUnit(convertedValue, "capacitor");
		// } else
		if(target.Contains("resistor")) {
			//Debug.Log("capacitor constraint - resistor");
			if( (GameObject.Find("FrequencyConstraintsValue").activeSelf) && (GameObject.Find("SelectValuePanelTitle").GetComponent<Text>().text == source) ) {
				//Debug.Log("this panel is right");
				string sourceValueText = Util.getChildObject(source, "ValueText").GetComponent<Text>().text;
				double sourceValue = double.Parse(sourceValueText.Substring(0, sourceValueText.Length-2));
				if(sourceValueText.EndsWith("p"))
					sourceValue *= 0.000000000001;
				else if(sourceValueText.EndsWith("n"))
					sourceValue *= 0.000000001;
				else if(sourceValueText.EndsWith("u"))
					sourceValue *= 0.000001;

				string targetValueText = Util.getChildObject(target, "ValueText").GetComponent<Text>().text;
				float targetValue = float.Parse(targetValueText.Substring(0, targetValueText.Length-2));
				if(targetValueText.EndsWith("K"))
					targetValue *= 1000;
				else if(targetValueText.EndsWith("M"))
					targetValue *= 1000000;
				float RCFomulaResult = RCFormula(targetValue, sourceValue);
				Text FrequencyConstraintsValue = Util.getChildObject("SelectValuePanel", "FrequencyConstraintsValue").GetComponent<Text>();
				if(RCFomulaResult == -1) FrequencyConstraintsValue.text = "infinity";
				else FrequencyConstraintsValue.text = Util.changeUnit(RCFormula(targetValue, sourceValue)*1000, "frequency");
				//GameObject.Find("FrequencyConstraintsValue").GetComponent<Text>().text = Util.changeUnit(RCFormula(targetValue, sourceValue)*1000, "frequency");
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
