using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class onDrag : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Vector3 GetCurrentMousePosition()
    {
        //float distance = 1200;//GameObject.Find(comm.getSourcePin()).transform.position.z;
        float distance = Camera.main.nearClipPlane;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
	
	public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z);
    }
}
