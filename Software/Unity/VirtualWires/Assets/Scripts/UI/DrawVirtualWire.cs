using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DrawVirtualWire : MonoBehaviour
{
    private LineRenderer line;
    public Material material;
    private GameObject srcObj;
    private GameObject tgtObj;
    public Sprite ConnectedPinSprite;

    public void Reset(LineRenderer lr)
    {
        //Debug.Log("[DrawVirtualWire.cs] " + lr.name);
        //lr.positionCount = 0;
        //lr.SetVertexCount(0);
        lr.enabled = false;
    }

    void Start() {
        srcObj = null;
        tgtObj = null;
        //EventMessenger
    }

    void Update()
    {
        if(srcObj != null && tgtObj != null)
        {
            line = new GameObject("Wire" + ":" + srcObj.name + "," + tgtObj.transform.parent.name + "-" + tgtObj.name).AddComponent<LineRenderer>();
            line.material = material;
            //line.SetVertexCount(2);
            line.positionCount = 2;
            //line.SetWidth(4, 4);
            line.startWidth = 4;
            line.endWidth = 4;
            line.SetPosition(0, srcObj.transform.position);
            line.SetPosition(1, new Vector3(tgtObj.transform.position.x, tgtObj.transform.position.y-5, tgtObj.transform.position.z));
            line.tag = "wire";
            line = null;
            Button btTempPin = srcObj.GetComponent<Button>();
            btTempPin.image.sprite = ConnectedPinSprite;
            // updateConnectionData(srcObj, tgtObj);
            // srcObj.GetComponent<Pin>().connectedTo = tgtObj;
            resetTargetObj();
            resetSourceObj();
        }
    }

    // void updateConnectionData(GameObject src, GameObject tgt) {
    //     src.GetComponent<Pin>().connectedTo = tgt;
    // }

    public void removeWireWithComponent(string targetComponent)
    {
        //Debug.Log("[DrawVirtualWire.cs] " + "removeWireWithComponent");
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

    public void setSourceObj(GameObject obj)
    {
        srcObj = obj;
    }

    public void setTargetObj(GameObject obj)
    {
        tgtObj = obj;
        if(srcObj != null)
            srcObj.GetComponent<Pin>().connectedTo = tgtObj;
    }

    public void resetTargetObj()
    {
        tgtObj = null;
    }

    public void resetSourceObj()
    {
        srcObj = null;
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