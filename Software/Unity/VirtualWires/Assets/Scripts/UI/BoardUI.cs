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
    public RectTransform ParentPanel;
	public BoardDataHandler board;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	private GameObject getChildObject(string ParentObjectName, string ChildObjectName)
    {
        GameObject temp = GameObject.Find(ParentObjectName);
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
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
		// string type = (string)boardJson.GetValue("type");
		int numberOfComponent = (int)boardJson.GetValue("numberOfComponents");
		for(int i=0; i<numberOfComponent; i++)
		{
			JObject componentData = (JObject)boardJson.GetValue("components")[i].DeepClone();
			string initData = componentData.ToString();
			string componentName = (string)componentData.GetValue("componentType");
			string uiComponentName = (string)componentData.GetValue("componentType")+(string)componentData.GetValue("id");
			string type = (string)componentData.GetValue("type");
			ComponentBase comp = ComponentFactory.Create(componentName, componentData);
			GameObject component = null;

			switch (componentName) {
				case "resistor":
					component = (GameObject)Instantiate(prefabResistor);
					if(type == "fixed") {
						getChildObject(component.name, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					} else {
						Resistor resistor = (Resistor)comp;
						getChildObject(component.name, "ValueText").GetComponent<Text>().text = resistor.maxValue.ToString();
					}
					break;
				case "capacitor":
					component = (GameObject)Instantiate(prefabCapacitor);
					getChildObject(component.name, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					break;
				case "inductor":
					component = (GameObject)Instantiate(prefabInductor);
					getChildObject(component.name, "ValueText").GetComponent<Text>().text = comp.value.ToString();
					break;
			}

			if(component) {
				component.tag = "component";
				component.name = uiComponentName;
				component.transform.SetParent(ParentPanel, false);
				if(i < 4) {
					component.transform.position = new Vector3(400,ParentPanel.transform.position.y+100,150-i*100);
				} else {
					component.transform.position = new Vector3(550,ParentPanel.transform.position.y+100,150-(i-4)*100);
				}
				component.transform.localScale = new Vector3(0.6f, 0.6f, 1);
				component.GetComponent<Component>().setComponentData(comp);
				component.GetComponent<Component>().setComponentInitData(initData);
			}
		}
	}
}