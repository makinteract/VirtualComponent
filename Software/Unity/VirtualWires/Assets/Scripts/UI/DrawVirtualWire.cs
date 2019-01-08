using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DrawVirtualWire : MonoBehaviour
{
    private LineRenderer line;
    public Material material;
    private GameObject boardPinObj;
    private GameObject componentPinObj;
    public Sprite ConnectedPinSprite;

    public void Reset(LineRenderer lr)
    {
        //Debug.Log("[DrawVirtualWire.cs] " + lr.name);
        //lr.positionCount = 0;
        //lr.SetVertexCount(0);
        lr.enabled = false;
    }

    void Start() {
        boardPinObj = null;
        componentPinObj = null;
        //EventMessenger
    }

    void Update()
    {
        if(boardPinObj != null && componentPinObj != null)
        {
            int xOffset = 0;
            line = new GameObject("Wire" + ":" + boardPinObj.name + "," + componentPinObj.transform.parent.name + "-" + componentPinObj.name).AddComponent<LineRenderer>();
            Debug.Log(line.name);

            line.material = material;
            line.positionCount = 2;
            line.startWidth = 4;
            line.endWidth = 4;
            line.SetPosition(0, boardPinObj.transform.position);
            
            if(componentPinObj.name.Contains("Left")) xOffset = 10;
            else if(componentPinObj.name.Contains("Right")) xOffset = -10;
            line.SetPosition(1, new Vector3(componentPinObj.transform.position.x+xOffset, componentPinObj.transform.position.y-5, componentPinObj.transform.position.z));

            line.tag = "wire";
            line = null;
            Button btTempPin = boardPinObj.GetComponent<Button>();
            btTempPin.image.sprite = ConnectedPinSprite;
            resetComponentPinObj();
            resetBoardPinObj();
        }
    }

    // void updateConnectionData(GameObject src, GameObject tgt) {
    //     src.GetComponent<Pin>().connectedTo = tgt;
    // }

    public void removeWireWithComponent(string targetComponent)
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if( wireObj.name.Contains(targetComponent) )
            {
                LineRenderer lr = wireObj.GetComponent<LineRenderer>();
                lr.enabled = false;
                //lr.SetVertexCount(0);
                lr.positionCount = 0;
                Destroy(wireObj);
            }
        }
        //GameObject compObjTemp = GameObject.Find(targetComponent);
        //Destroy(compObjTemp);
    }

    public void removeWire(string targetComponent, string  sourcePin)
    {
        //Debug.Log("[DrawVirtualWire.cs] " + "removeWire");
        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        GameObject targetTemp = GameObject.Find(targetComponent);

        foreach(GameObject wireObj in temp)
        {
            //Debug.Log("[DrawVirtualWire.cs] " + "foreach");
            //if there is a wire which is already connected to this target
            if( wireObj.name.Contains(targetComponent) && wireObj.name.Contains(sourcePin))
            {
                LineRenderer lr = wireObj.GetComponent<LineRenderer>();
                lr.enabled = false;
                //lr.SetVertexCount(0);
                lr.positionCount = 0;
                Destroy(wireObj);
            }
        }
        //Debug.Log("DrawVirtualWires.cs : removeWire() " + "target: " + targetComponent + "source: " + sourcePin);
    }

    public void setBoardPinObj(GameObject obj)
    {
        boardPinObj = obj;
        if(componentPinObj != null) {
            boardPinObj.GetComponent<Pin>().connectedTo = componentPinObj;
        }
    }

    public void setComponentPinObj(GameObject obj)
    {
        componentPinObj = obj;
        if(boardPinObj != null)
            boardPinObj.GetComponent<Pin>().connectedTo = componentPinObj;
    }

    public void resetComponentPinObj()
    {
        //Debug.Log("resetComponentPin");
        componentPinObj = null;
    }

    public void resetBoardPinObj()
    {
        //Debug.Log("resetBoardPin");
        boardPinObj = null;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}