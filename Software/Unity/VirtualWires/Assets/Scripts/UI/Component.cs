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
        if(data.type == "fixed") {
            fixedComponent = true;
        } else {
            fixedComponent = false;
        }
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
        data.value = (int)initData["value"];
        data.connection[0]["boardPin"] = initData["connections"][0]["boardPin"];
        data.connection[1]["boardPin"] = initData["connections"][1]["boardPin"];

        Debug.Log("resetComponentData() value = " + data.value);
        Debug.Log("resetComponentData() name = " + data.componentName);
        Debug.Log(data.connection.ToString());
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
        drag = true;
        transform.position = new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z);

        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if(wireObj.name.Contains(name))
            {
                if(wireObj.name.Contains("Left")) {
                    wireEndPosition = getTargetComponentPinPosition("LeftPin");
                }
                else if(wireObj.name.Contains("Right")) {
                    wireEndPosition = getTargetComponentPinPosition("RightPin");
                }
                LineRenderer wireLineRender = wireObj.GetComponent<LineRenderer>();
                wireLineRender.SetPosition(1, wireEndPosition);
            }
        }
    }
	
    void EnterPauseState()
    {
        Communication comm = GameObject.Find("Communication").GetComponent<Communication>();
        comm.pauseButton.gameObject.SetActive(true);
		VuforiaRenderer.Instance.Pause(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;
        //VuforiaRenderer.Instance.Pause(false);
    }

    private Vector3 GetCurrentMousePosition()
    {
        float distance = 1100;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}