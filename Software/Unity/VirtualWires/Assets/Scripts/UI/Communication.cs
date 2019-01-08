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
    public Image freezeAll;

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

    public void setBoardPin(string pin)
    {
        vw.boardPin = pin;
    }

    public void setComponentPin(string pin)
    {
        vw.componentPin = pin;
    }

    public string getBoardPin()
    {
        return vw.boardPin;
    }

    public string getComponentPin()
    {
        return vw.componentPin;
    }

    public void resetData()
    {
        vw.boardPin = null;
        vw.componentPin = null;
    }

    public void setAwgIP(string _awgIp)
    {
        vw.awgIp = _awgIp;
    }

    public string getAwgIP()
    {
        return vw.awgIp;
    }
}