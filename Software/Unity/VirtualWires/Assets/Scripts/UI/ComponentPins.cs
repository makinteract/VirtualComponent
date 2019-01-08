using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ComponentPins : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler//, IPointerClickHandler, IPointerUpHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler, 
{
    public Communication comm;
    public DrawVirtualWire wire;
    private bool alreadyWired = false;

    public void Start() {
        setWireObject();
		setCommunicationObject();
    }
    
    public void setCommunicationObject()
    {
		comm = GameObject.Find("Communication").GetComponent<Communication>();
        //comm = temp.GetComponent<ComponentObject>().getCommunicationObject();
		//Debug.Log(comm.name);
    }

    public void setWireObject()
    {
        wire = GameObject.Find("DrawVirtualWires").GetComponent<DrawVirtualWire>();
        //wire = temp.GetComponent<ComponentObject>().getWireObject();
		//Debug.Log(wire.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.transform.parent.GetComponent<Component>().setDragState(false);
            if (!comm.getDeleteWireState())
            {
                comm.setComponentPin(transform.parent.name + "-" + name);
                //Debug.Log("OnPointerEnter() - button down - componentPin = " + comm.getComponentPin());
                wire.setComponentPinObj(gameObject);
            }
        } else
        {
            if(!comm.getDeleteWireState())
                comm.setComponentPin(transform.parent.name + "-" + name);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        this.transform.parent.GetComponent<Component>().setDragState(true);
        // VuforiaRenderer.Instance.Pause(false);
        if (Input.GetMouseButtonUp(0))
        {
            string componentPinName = null;
            string boardPinName = null;
            string boardPin = null;
            string compPin = null;

            if(comm.getComponentPin() != null)
            {
                componentPinName = comm.getComponentPin();
                //Debug.Log("componentPinName = " + componentPinName);
                compPin = componentPinName.Substring(0, 3);
                //Debug.Log("compPin = " + compPin);
            }
            if(comm.getBoardPin() != null)
            {
                boardPinName = comm.getBoardPin();
                //Debug.Log("boardPinName = " + boardPinName);
                boardPin = boardPinName.Substring(0, 3);
                //Debug.Log("boardPin = " + boardPin);
            }
            if((boardPinName == null) || (boardPinName == "")) {
                wire.resetBoardPinObj();
                wire.resetComponentPinObj();
                comm.resetData();
            } else {
                if( (compPin != boardPin) && !pinAlreadyWired(boardPinName) && !pinAlreadyWired(componentPinName))
                {
                    //Debug.Log("come to right space...");
                    //Debug.Log("boardPinName = " + boardPinName);
                    GameObject temp = GameObject.Find(boardPinName);
                    if(temp != null) {
                        //Debug.Log("target object = " + temp.name);  /// pin 111 을 이상하게 찾는 듯 null은 아닌데 값이 이상함
                        comm.setBoardPin(boardPinName);
                        wire.setBoardPinObj(temp);
                        // Todo: arduino에게 Json 보내기 (value 변경)
                        // Notify connected info ComponentDataHandler -> notify BoardDataHandler
                        //                                            -> notify JsonHandler
                        notifyUpdateComponentData(true);
                    } else {
                        Debug.Log("cannot find board pin object");
                    }
                } else {
                    wire.resetBoardPinObj();
                    wire.resetComponentPinObj();
                    comm.resetData();
                }
            }
        }
    }

    private bool pinAlreadyWired(string name)
    {
        //Debug.Log("pin name = " + name);
        bool result = false;
        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if( wireObj.name.Contains(name))
            {
                Debug.Log("already wired");
                result = true;
            }
        }
        return result;
    }

    public Component getComponentObject(string _name) {
        return GameObject.Find(_name).GetComponent<Component>();
    }

    void notifyUpdateComponentData(bool _connected) {
        Component changedComponent = getComponentObject(transform.parent.name);
        ComponentBase result = changedComponent.getComponentData();
        if(_connected) {
            string boardPinName = comm.getBoardPin();
            //Debug.Log(boardPinName);
            if(boardPinName.Contains("-")) {
                boardPinName = boardPinName.Replace("-","");
                //Debug.Log("converted to = " + boardPinName);
            }
            int boardPin = int.Parse(boardPinName.Substring(3, boardPinName.Length-3)) / 10;
            
            //Debug.Log("boardPin = " + boardPin);

            result.changed.Add("set", "X");
            result.changed.Add("on", 1);

            if(transform.name == "LeftPin") {
                result.connection[0]["B"] = boardPin;
                //Debug.Log("left connected " + boardPin);
                //Debug.Log("result = " + result.connection[0]["B"]);
                result.changed.Add("M", result.connection[0]["M"]);
                result.changed.Add("B", result.connection[0]["B"]);
            } 
            else if(transform.name == "RightPin") {
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

            if(transform.name == "LeftPin") {
                result.changed.Add("M", result.connection[0]["M"]);
                result.changed.Add("B", result.connection[0]["B"]);
                result.connection[0]["B"] = null;
            } 
            else if(transform.name == "RightPin") {
                result.changed.Add("M", result.connection[1]["M"]);
                result.changed.Add("B", result.connection[1]["B"]);
                result.connection[1]["B"] = null;
            }
        }
        changedComponent.updateComponentDataEvent.Invoke(result);
    }
}