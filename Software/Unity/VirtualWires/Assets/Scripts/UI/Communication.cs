using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Communication : MonoBehaviour
{
    public VWJson vw;
    private int key = 1;
    private float time;
    private bool deleteState;
    private bool dragState;
    private bool popupState;

    private string component;

    public Button pauseButton;

    Dictionary<int, string> vwDB = new Dictionary<int, string>();

    void Start()
    {
        deleteState = false;
        dragState = false;
        popupState = false;
    }
    
    public GameObject getPauseButton() {
        return pauseButton.gameObject;
    }
    
    public bool getPopupState()
    {
        return popupState;
    }

    public void setPopupState(bool state)
    {
        popupState = state;
    }

    public bool getDragState()
    {
        return dragState;
    }

    public void setDragState(bool state)
    {
        dragState = state;
    }

    public void setDeleteWireState(bool state, string name)
    {
        deleteState = state;
        component = name;
    }

    public string getComponentInDeleteState()
    {
        return component;
    }

    public bool getDeleteWireState()
    {
        return deleteState;
    }

    public void setStartTime(float t)
    {
        time = t;
    }

    public float getStartTime()
    {
        return time;
    }

    public void setSourcePin(string pin)
    {
        vw.source = pin;
    }

    public void setTargetPin(string pin)
    {
        vw.target = pin;
    }

    public string getSourcePin()
    {
        Debug.Log(vw.source);
        return vw.source;
    }

    public string getTargetPin()
    {
        return vw.target;
    }

    public void resetData()
    {
        vw.source = null;
        vw.target = null;
    }

    // string swapSourcetarget()
    // {
    //     string result = vw.source;
    //     vw.source = vw.target;
    //     vw.target = result;
    //     result = vw.SaveToString();
    //     return result;
    // }
}