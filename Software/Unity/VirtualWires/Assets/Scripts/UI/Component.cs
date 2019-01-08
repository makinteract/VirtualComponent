using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Vuforia;

public class Component : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler//, IPointerEnterHandler//, IPointerUpHandler
{
    private Vector3 wireEndPosition;
    private bool drag;
    public bool fixedComponent;
    //ComponentFactory component;
    private ComponentBase data;
    private string initDataJsonString;
    // private BoardDataHandler boardDataHandler;
    private JsonHandler jsonHandler;
    public UnityAction<ComponentBase> updateComponentDataAction;
    public UpdateDataEvent updateComponentDataEvent;
    //public Button pauseButton;
    
    public void Awake()
    {
    }

    public void Setup()
    {
    }

	public void Start()
    {
        getJsonHandlerObject();
        updateComponentDataAction = new UnityAction<ComponentBase>(updateComponentData);
        updateComponentDataEvent = new UpdateDataEvent();
        updateComponentDataEvent.AddListener(updateComponentDataAction);
        if(data.type == "fix") {
            fixedComponent = true;
        } else {
            fixedComponent = false;
        }
        drag = true;
    }

    public void updateComponentData(ComponentBase _data)
    {
        //Debug.Log("ComponentDataHandler");
        data = _data;
        jsonHandler.updateJsonEvent.Invoke(_data);
    }

    private void getJsonHandlerObject()
    {
        jsonHandler = GameObject.Find("JsonHandler").GetComponent<JsonHandler>();
    }

    public ComponentBase getComponentData()
    {
        return data;
    }

    public void setComponentData(ComponentBase _data)
    {
        data = _data;
    }

    public void setComponentInitData(string _data)
    {
        initDataJsonString = _data;
    }

    public void resetComponentData()
    {
        JObject initData = JObject.Parse(initDataJsonString);

        // JObject resetValueObject = new JObject();
        // JObject disconnectLeftWireObject = new JObject();
        // JObject disconnectRightWireObject = new JObject();

        // string component = "";
        
        // need to send inital value
        if(!fixedComponent)
            data.value = (int)initData["value"];
        
        // if(name.Contains("resistor")) component = "R";
        // else if (name.Contains("capacitor")) component = "C";
        // else if (name.Contains("inductor")) component = "L";

        // resetValueObject.Add("set", component);
        // resetValueObject.Add("id", data.id);
        // resetValueObject.Add("val", data.value);
        // data.changed.Add("resetValue", resetValueObject);

        // // disconnect left wire
        // disconnectLeftWireObject.Add("set", "X");
        // disconnectLeftWireObject.Add("on", 0);
        // disconnectLeftWireObject.Add("M", data.connection[0]["M"]);
        // disconnectLeftWireObject.Add("B", data.connection[0]["B"]);
        // data.changed.Add("disconnectLeft", disconnectLeftWireObject);

        // // need to disconnect right wire
        // disconnectRightWireObject.Add("set", "X");
        // disconnectRightWireObject.Add("on", 0);
        // disconnectRightWireObject.Add("M", data.connection[1]["M"]);
        // disconnectRightWireObject.Add("B", data.connection[1]["B"]);
        // data.changed.Add("disconnectRight", disconnectRightWireObject);

        // board information reset
        data.connection[0]["B"] = initData["connections"][0]["B"];
        data.connection[1]["B"] = initData["connections"][1]["B"];

        //jsonHandler.updateJsonEvent.Invoke(data);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        EnterPauseState();
    }

    private Vector3 getTargetComponentPinPosition(string pinName)
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        Vector3 result = Vector3.zero;
        foreach(Transform obj in children)     
        {
            if(obj.name == pinName) {
                result = obj.position;
            }
        }
        //Debug.Log("ComponentObject.cs - getTargetComponentPinPosition = " + result);
        return result;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //drag = true;
        if(drag) {
            transform.position = new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z);

            GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
            foreach(GameObject wireObj in temp)
            {
                if(wireObj.name.Contains(name))
                {
                    if(wireObj.name.Contains("Left")) {
                        wireEndPosition = getTargetComponentPinPosition("LeftPin");
                        wireEndPosition.x += 10;
                    }
                    else if(wireObj.name.Contains("Right")) {
                        wireEndPosition = getTargetComponentPinPosition("RightPin");
                        wireEndPosition.x -= 10;
                    }
                    LineRenderer wireLineRender = wireObj.GetComponent<LineRenderer>();
                    wireLineRender.SetPosition(1, wireEndPosition);
                }
            }
        }
    }
	
    public void setDragState(bool state) {
        drag = state;
    }

    public bool getDragState() {
        return drag;
    }

    void EnterPauseState()
    {
        Communication comm = GameObject.Find("Communication").GetComponent<Communication>();
        comm.pauseButton.gameObject.SetActive(true);
		VuforiaRenderer.Instance.Pause(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //drag = false;
        //VuforiaRenderer.Instance.Pause(false);
    }

    private Vector3 GetCurrentMousePosition()
    {
        //float distance = 1100;
        //float distance = Camera.main.nearClipPlane;
        float distance = Camera.main.transform.position.y - transform.position.y;
        
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}