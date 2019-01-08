using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour {
	public GameObject prefabResistor;
	public GameObject prefabCapacitor;
	public GameObject prefabInductor;
	public GameObject prefabWire;
	public GameObject prefabLed;
	public GameObject prefabSwitch;
	public GameObject prefabPhotoresistor;
	public GameObject prefabDiode;
	public GameObject prefabZenerdiode;
	public GameObject prefabVoltmeter;
	public GameObject prefabFunctionGenerator;
	public GameObject prefabEtc;
    public RectTransform ParentPanel;
	public BoardDataHandler board;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	private GameObject getChildObject(GameObject ParentObject, string ChildObjectName)
    {
        GameObject resultObj = null;
        
        Transform[] children = ParentObject.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name == ChildObjectName) {
                resultObj = obj.gameObject;
            }
        }
        return resultObj;
    }

	public void setupBoard(string filePath)
	{
		JObject boardJson;

		using (StreamReader reader = File.OpenText(filePath))
		{
			boardJson = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
			board.GetComponent<BoardDataHandler>().setBoardJson(boardJson);
		}
		
		//Debug.Log(boardJson.GetValue("Components"));
		// string type = (string)boardJson.GetValue("modeType");
		string boardName = (string)boardJson.GetValue("boardName");
		Text boadNameText = GameObject.Find("BoardName").GetComponent<Text>();
		boadNameText.text = boardName;

		int numberOfComponent = (int)boardJson.GetValue("numberOfComponents");
		Debug.Log("numberOfComponent = " + numberOfComponent);
		for(int i=0; i<numberOfComponent; i++)
		{
			JObject componentData = (JObject)boardJson.GetValue("components")[i].DeepClone();
			string initData = componentData.ToString();
			string componentName = (string)componentData.GetValue("componentType");
			string uiComponentName = (string)componentData.GetValue("componentType")+(string)componentData.GetValue("id");
			string type = (string)componentData.GetValue("modeType");
			ComponentBase comp = ComponentFactory.Create(componentName, componentData);
			GameObject component = null;

			switch (componentName) {
				case "resistor":
					component = (GameObject)Instantiate(prefabResistor);
					if(type == "fix") {
						getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName); //comp.value.ToString();
					} else {
						Resistor resistor = (Resistor)comp;
						getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(resistor.minValue+(resistor.value*resistor.stepResistance), "resistor"); //.ToString();
					}
					break;
				case "capacitor":
					component = (GameObject)Instantiate(prefabCapacitor);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);//comp.value.ToString();
					break;
				case "inductor":
					component = (GameObject)Instantiate(prefabInductor);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);//comp.value.ToString();
					break;
				case "wire":
					component = (GameObject)Instantiate(prefabWire);
					//getChildObject(component, "ValueText").GetComponent<Text>().text = "";comp.value.ToString();
					break;
				case "led":
					component = (GameObject)Instantiate(prefabLed);
					//getChildObject(component, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					break;
				case "switch":
					component = (GameObject)Instantiate(prefabSwitch);
					//getChildObject(component, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					break;
				case "photoresistor":
					component = (GameObject)Instantiate(prefabPhotoresistor);
					//getChildObject(component, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					break;
				case "diode":
					component = (GameObject)Instantiate(prefabDiode);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);
					break;
				case "zenerdiode":
					component = (GameObject)Instantiate(prefabZenerdiode);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);
					break;
				case "ADC": // voltmeter
					component = (GameObject)Instantiate(prefabVoltmeter);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);
					break;
				case "AWG":	// function generator
					component = (GameObject)Instantiate(prefabFunctionGenerator);
					//getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);
					break;
				default:
					component = (GameObject)Instantiate(prefabEtc);
					getChildObject(component, "ValueText").GetComponent<Text>().text = Util.changeUnit(comp.value, componentName);
					break;
			}

			if(component) {
				component.tag = "component";
				component.name = uiComponentName;
				component.transform.SetParent(ParentPanel, false);
				if(i < 4) {
					component.transform.position = new Vector3(265,ParentPanel.transform.position.y+10,-80-i*110);
				} else {
					component.transform.position = new Vector3(415,ParentPanel.transform.position.y+10,-80-(i-4)*110);
				}
				component.transform.localScale = new Vector3(0.6f, 0.6f, 1);
				component.GetComponent<Component>().setComponentData(comp);
				component.GetComponent<Component>().setComponentInitData(initData);
			}
		}
	}
}