using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Vuforia;

public class DrawLine : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Communication comm;
    public Material material;
    private LineRenderer lineRenderer;
    private Vector3 mousePosition;
    private Vector3 wireEndPosition;

    Vector2 startPoint;
    Vector2 endPoint;
    bool drag;

    public void setCommunicationObject(Communication obj)
    {
        comm = obj;
    }

    void EnterPauseState()
    {
        //Communication comm = GameObject.Find("Communication").GetComponent<Communication>();
        comm.pauseButton.gameObject.SetActive(true);
		VuforiaRenderer.Instance.Pause(true);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        EnterPauseState();
        lineRenderer.positionCount = 1;
        //comm.setSourcePin(name);
        // if(GameObject.Find(comm.getSourcePin())) {
        //     lineRenderer.SetPosition(0, GameObject.Find(name).transform.position);
        // } else {
            lineRenderer.SetPosition(0, GetCurrentMousePosition());
        // }
    }

    public void OnDrag(PointerEventData eventData)
    {
        drag = true;
        wireEndPosition = GetCurrentMousePosition();
        lineRenderer.enabled = true;
        //lineRenderer.SetVertexCount(2);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, wireEndPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lineRenderer.enabled = false;
        //lineRenderer.SetVertexCount(0);
        lineRenderer.positionCount = 0;
        drag = false;
        //VuforiaRenderer.Instance.Pause(false);
    }

    public void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.SetWidth(4, 4);
        lineRenderer.startWidth = 4;
        lineRenderer.endWidth = 4;
        lineRenderer.enabled = false;
        lineRenderer.material = material;
        //lineRenderer.SetVertexCount(0);
        lineRenderer.positionCount = 0;
    }

    private Vector3 GetCurrentMousePosition()
    {
        float distance = 1500;//GameObject.Find(comm.getSourcePin()).transform.position.z;
        mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    /*
    private Vector3 GetCurrentMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);


        Vector3 result = new Vector3(0,0,0);

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            result = ray.GetPoint(rayDistance);
            Debug.Log("current mouse position = " + result.x + ", " + result.y + ", " + result.z);
        }
        return result;
    }*/
}