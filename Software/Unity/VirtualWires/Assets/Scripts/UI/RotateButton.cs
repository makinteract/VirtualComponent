using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateButton : MonoBehaviour {
	private Vector3 wireEndPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Rotate() {
		Vector3 targetPos = getChildObject(transform.parent.name, "Component").transform.position;
		GameObject leftPinObject = getChildObject(transform.parent.name, "LeftPin");
		leftPinObject.transform.RotateAround(targetPos, new Vector3 (0, 1, 0), 90);
		GameObject RightPinObject = getChildObject(transform.parent.name, "RightPin");
		RightPinObject.transform.RotateAround(targetPos, new Vector3 (0, 1, 0), 90);
		gameObject.transform.RotateAround(targetPos, new Vector3 (0, 1, 0), 90);

		GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if(wireObj.name.Contains(transform.parent.name))
            {
                if(wireObj.name.Contains("Left")) {
                    wireEndPosition = leftPinObject.transform.position;
                    wireEndPosition.x += 10;
                }
                else if(wireObj.name.Contains("Right")) {
                    wireEndPosition = RightPinObject.transform.position;
                    wireEndPosition.x -= 10;
                }
                LineRenderer wireLineRender = wireObj.GetComponent<LineRenderer>();
                wireLineRender.SetPosition(1, wireEndPosition);
            }
        }
	}

	private GameObject getChildObject(string ParentObjectName, string ChildObjectName)
    {
        GameObject temp = GameObject.Find(ParentObjectName);
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            //Debug.Log("[pin.cs] 7 " + obj.name);
            if(obj.name == ChildObjectName) {
                resultObj = obj.gameObject;
            }
        }
        //Debug.Log("[Pin.cs] 88 " + "getChildObject = " + resultObj.name);
        //Debug.Log("[Pin.cs] 99 " + "getChildObject = " + resultObj.transform.parent.name);
        return resultObj;
    }
}
