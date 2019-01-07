using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ComponentPins : MonoBehaviour, IPointerEnterHandler//, IPointerClickHandler, IPointerUpHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler, 
{
    public Communication comm;
    public DrawVirtualWire wire;
    private bool alreadyWired = false;

    public void setCommunicationObject()
    {
		comm = GameObject.Find("Communication").GetComponent<Communication>();
        //comm = temp.GetComponent<ComponentObject>().getCommunicationObject();
		//Debug.Log(comm.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //setWireObject();
		setCommunicationObject();

        //Debug.Log("[ComponentPins.cs] " + "OnPointerEnter");
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("[ComponentPins.cs] " + "mouse button down: " + name);
        } else
        {
            if(!comm.getDeleteWireState())
                comm.setTargetPin(transform.parent.name + "-" + name);
        }
    }
}