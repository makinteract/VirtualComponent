// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;

// public class ComponentButton : MonoBehaviour
// {
//     public GameObject prefabButton;
//     public Communication communication;
//     public DrawVirtualWire wire;
//     public RectTransform ParentPanel;
//     public Button thisComponent;
//     private int count = 0;

//     //public GameObject window;

//     private ModalPanel modalPanel;

//     void Awake () {
//         modalPanel = ModalPanel.Instance ();
//     }
    
//     void Start () {
//         thisComponent.onClick.AddListener(TaskOnClick);
//     }

//     void TaskOnClick()
//     {
//         count++;
//         Debug.Log("You have clicked the button!");
//         Vector3 pos = new Vector3(500, 100, -20);
//         Debug.Log("[ComponentButton.cs] " + thisComponent.name);

//         if(!communication.getDeleteWireState())
//         {
//             if(thisComponent.name == "Resistor"){
//                 Debug.Log("[ComponentButton.cs] " + "Resistor in");
//                 GameObject goButton = (GameObject)Instantiate(prefabButton);
//                 goButton.transform.SetParent(ParentPanel, false);
//                 goButton.transform.position = new Vector3(10,ParentPanel.transform.position.y+100,30);
//                 goButton.transform.localScale = new Vector3(0.6f, 0.6f, 1);
//                 goButton.tag = "component";
//                 goButton.GetComponent<ComponentObject>().setCommunicationObject(communication);
//                 goButton.GetComponent<ComponentObject>().setWireObject(wire);
//                 Button tempButton = goButton.GetComponent<Button>();
//                 tempButton.name = "Resistor_" + count;
//                 //tempButton.GetComponent<Text>().text = tempButton.name; --> problem!! nullreferenceexception
//                 //tempButton.GetComponent<Text>().text = modalPanel.getSliderValue().ToString();
//                 tempButton.onClick.AddListener(() => ButtonClicked(tempButton.name));
//             }

//             if(thisComponent.name == "Capacitor"){
//                 Debug.Log("[ComponentButton.cs] " + "Capacitor in");
//                 GameObject goButton = (GameObject)Instantiate(prefabButton);
//                 goButton.transform.SetParent(ParentPanel, false);
//                 goButton.transform.position = new Vector3(10,ParentPanel.transform.position.y+100,30);
//                 goButton.transform.localScale = new Vector3(0.6f, 0.6f, 1);
//                 goButton.tag = "component";
//                 goButton.GetComponent<ComponentObject>().setCommunicationObject(communication);
//                 goButton.GetComponent<ComponentObject>().setWireObject(wire);
//                 Button tempButton = goButton.GetComponent<Button>();
//                 tempButton.name = "Capacitor_" + count;
//                 //tempButton.GetComponent<Text>().text = tempButton.name; problem!
//                 //tempButton.GetComponent<Text>().text = modalPanel.getSliderValue().ToString();
//                 tempButton.onClick.AddListener(() => ButtonClicked(tempButton.name));
//             }
//         }
//     }

//     void ButtonClicked(string btnName)
//     {
//         Debug.Log ("Button clicked = " + btnName);
//         //window.SetActive(true);
//     }
// }