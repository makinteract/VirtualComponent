using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vuforia;

public class Pin : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler// required interface when using the OnPointerEnter method.
{
    public Communication comm;
    public DrawVirtualWire wire;
    //public BezierCurve wire;
    //Do this when the cursor enters the rect area of this selectable UI object.

    private DeleteConfirmPanel deleteConfirmPanel;
    private UnityAction deleteYesAction;
    private UnityAction deleteCancelAction;
    public Sprite ConnectedPinSprite;
    public Sprite DefaultPinSprite;
    public GameObject connectedTo;

    void Start() {
        connectedTo = null;
    }

    void Awake () {
        deleteConfirmPanel = DeleteConfirmPanel.Instance();
        deleteYesAction = new UnityAction (DeleteYesFunction);
        deleteCancelAction = new UnityAction (DeleteCancleFunction);
    }

     public void deleteOptionWindow() {
        deleteConfirmPanel.Choice (deleteYesAction, deleteCancelAction);
        deleteConfirmPanel.setTitle("Delete this Wire?");
        deleteConfirmPanel.setPosition(new Vector3(transform.position.x+150, transform.position.y, transform.position.z));
    }

    public Component getComponentObject(string _name) {
        return GameObject.Find(_name).GetComponent<Component>();
    }

    void notifyUpdateComponentData(bool _connected) {
        Component targetComponentObject = getComponentObject(connectedTo.transform.parent.name);
        
        ComponentBase result = targetComponentObject.getComponentData();
        
        if(_connected) {
            if(name.Contains("-")) {
                name = name.Replace("-","");
            }
            int boardPin = int.Parse(name.Substring(3, name.Length-3)) / 10;

            result.changed.Add("set", "X");
            result.changed.Add("on", 1);

            if(connectedTo.name == "LeftPin") {
                result.connection[0]["B"] = boardPin;
                //Debug.Log("left connected " + boardPin);
                //Debug.Log("result = " + result.connection[0]["B"]);
                result.changed.Add("M", result.connection[0]["M"]);
                result.changed.Add("B", result.connection[0]["B"]);
            } 
            else if(connectedTo.name == "RightPin") {
                result.connection[1]["B"] = boardPin;
                //Debug.Log("right connected " + boardPin);
                //Debug.Log("result = " + result.connection[1]["B"]);
                result.changed.Add("M", result.connection[1]["M"]);
                result.changed.Add("B", result.connection[1]["B"]);
            }
        } else {
            // update delete
            result.changed.Add("set", "X");
            result.changed.Add("on", 0);
            if(connectedTo.name == "LeftPin") {
                result.changed.Add("M", result.connection[0]["M"]);
                result.changed.Add("B", result.connection[0]["B"]);
                result.connection[0]["B"] = null;
            } 
            else if(connectedTo.name == "RightPin") {
                result.changed.Add("M", result.connection[1]["M"]);
                result.changed.Add("B", result.connection[1]["B"]);
                result.connection[1]["B"] = null;
            }
        }
        targetComponentObject.updateComponentDataEvent.Invoke(result);
    }

    void DeleteYesFunction () {
        ExitDeleteMode(DefaultPinSprite, true);
        GameObject.Find(name).GetComponent<Button>().image.sprite = DefaultPinSprite; 
        string connectedComponent = connectedTo.transform.parent.name;
        //comm.setDeleteWireState(false, "");
        //comm.setTargetPin(name); ====???????
        comm.setBoardPin(name);
        // Todo: arduino에게 Json 보내기 (Connection 변경)
        // Notify connected info ComponentDataHandler -> notify BoardDataHandler
        //                                            -> notify JsonHandler
        notifyUpdateComponentData(false);

        //Debug.Log("*** comm.getComponentPin() = " + comm.getComponentPin());
        //Debug.Log("*** comm.getBoardPin() = " + comm.getBoardPin());

        //wire.removeWire(comm.getComponentPin(), comm.getBoardPin());
        wire.removeWire(connectedComponent, name);
        comm.resetData();
        wire.resetBoardPinObj();
        wire.resetComponentPinObj();
        connectedTo = null;
    }

    void DeleteCancleFunction () {
        ExitDeleteMode(ConnectedPinSprite, false);
        comm.setDeleteWireState(false, "");
        comm.resetData();
        wire.resetBoardPinObj();
        wire.resetComponentPinObj();
    }

    public bool PinsInDeleteState()
    {
        bool active = false;
        string component = comm.getComponentInDeleteState();

        //Debug.Log("***component = " + component);
        //Debug.Log("***connectedTo = " + connectedTo.name);
        //Debug.Log("***connectedTo = " + connectedTo.transform.parent.name);

        if(component == connectedTo.transform.parent.name) {
            active = true;
        }

        return active;

        // GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");

        // foreach(GameObject wireObj in temp)
        // {
        //     if( wireObj.name.Contains(component) )
        //     {
        //         int start = wireObj.name.IndexOf(":") + 1;
        //         int end = wireObj.name.IndexOf(",");
        //         string pinName = wireObj.name.Substring(start, end - start);
        //         Debug.Log("pinName? " + pinName);
        //         if(pinName == name) {
        //             active = true;
        //         }
        //     }
        // }
        // return active;
    }

	private GameObject getComponentObject(string ParentObjectName, string ChildObjectName)
    {
        GameObject temp = GameObject.Find(ParentObjectName);
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name == "Component") {
                resultObj = obj.gameObject;
            }
        }
        //Debug.Log("[Pin.cs] 88 " + "getComponentObject = " + resultObj.name);
        //Debug.Log("[Pin.cs] 99 " + "parent = " + resultObj.transform.parent.name);
        return resultObj;
    }

    public void ExitDeleteMode(Sprite sprite, bool delete)
    {
        string component = comm.getComponentInDeleteState();

        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        if(delete) {
            string connectedComponentName = connectedTo.transform.parent.name;
            GameObject componentButton = getComponentObject(connectedComponentName, connectedTo.name);
            componentButton.GetComponent<ComponentButton>().ExitDeleteMode(ConnectedPinSprite, delete);
            GameObject.Find(name).GetComponent<Button>().image.sprite = sprite;
        } else {
            string connectedComponentName = connectedTo.transform.parent.name;
            GameObject componentButton = getComponentObject(connectedComponentName, connectedTo.name);
            componentButton.GetComponent<ComponentButton>().ExitDeleteMode(sprite, delete);
        }   
    }

    public void setCommunicationObject(Communication obj)
    {
        comm = obj;
    }

    public void setWireObject(DrawVirtualWire obj)
    {
        wire = obj;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // VuforiaRenderer.Instance.Pause(true);
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("[pin.cs] " + "mouse button down: " + name);
            comm.setStartTime(Time.time);

            if (!comm.getDeleteWireState())
            {
                comm.setBoardPin(name);
                GameObject temp = GameObject.Find(name);
                if(temp != null)
                    wire.setBoardPinObj(temp);
                else Debug.Log("cannot find board pin object");
            }
        } else
        {
            if(!comm.getDeleteWireState())
                comm.setBoardPin(name);
        }
    }

    private bool pinAlreadyWired(string name)
    {
        //string targetComponentPinName = comm.getComponentPin();

        bool result = false;
        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if( wireObj.name.Contains(name))
            {
                result = true;
            }
        }
        return result;
    }

    private string getTargetComponentPinName(string targetComponentPinName)
    {
        int seperator = targetComponentPinName.IndexOf("-");
        string componentPinName = targetComponentPinName.Substring(seperator+1, targetComponentPinName.Length-seperator-1);
        return componentPinName;
    }

    private string getTargetComponentName(string targetComponentPinName)
    {
        int seperator = targetComponentPinName.IndexOf("-");
        string componentName = targetComponentPinName.Substring(0, seperator);
        return componentName;
    }

    private GameObject getTargetComponentPinObject(string targetComponentPinName)
    {
        GameObject temp = GameObject.Find(getTargetComponentName(targetComponentPinName));
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name == getTargetComponentPinName(targetComponentPinName)) {
                resultObj = obj.gameObject;
            }
        }
        return resultObj;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // VuforiaRenderer.Instance.Pause(false);
        if (Input.GetMouseButtonUp(0))
        {
            float releasedTime = Time.time;
            float pressedTime = comm.getStartTime();

            string boardPinName = null;
            string componentPinName = null;
            string boardPin = null;
            string compPin = null;

            if(comm.getBoardPin() != null)
            {
                boardPinName = comm.getBoardPin();
                boardPin = boardPinName.Substring(0, 3);
            }
            if(comm.getComponentPin() != null)
            {
                componentPinName = comm.getComponentPin();
                compPin = componentPinName.Substring(0, 3);
            }
            if(comm.getDeleteWireState())
            {
                if(PinsInDeleteState()) {
                    //deleteOptionWindow();
                    DeleteYesFunction();
                }
            } else if((componentPinName == null) || (componentPinName == "")) {
                wire.resetBoardPinObj();
                wire.resetComponentPinObj();
                comm.resetData();
            } else {   // drag released
                if (boardPin == "Pin" && compPin == "Pin")
                {
                    if(!comm.getDeleteWireState()) {
                        wire.resetBoardPinObj();
                        wire.resetComponentPinObj();
                        comm.resetData();
                    }
                }
                else if( (boardPin != compPin) && !pinAlreadyWired(componentPinName) && !pinAlreadyWired(boardPinName))
                {
                    comm.setComponentPin(componentPinName);
                    wire.setComponentPinObj(getTargetComponentPinObject(componentPinName));
                    // Todo: arduino에게 Json 보내기 (value 변경)
                    // Notify connected info ComponentDataHandler -> notify BoardDataHandler
                    //                                            -> notify JsonHandler
                    notifyUpdateComponentData(true);
                } else {
                    wire.resetBoardPinObj();
                    wire.resetComponentPinObj();
                    comm.resetData();
                }
            }
        }
    }
}