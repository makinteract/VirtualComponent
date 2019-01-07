using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ComponentButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler
{
    public Sprite DeleteModePinSprite;
    public Sprite ConnectedPinSprite;
    public Sprite DefaultPinSprite;
    public Sprite DeleteModeComponentSprite;
    public Sprite DefaultComponentSprite;
    //private ModalPanel modalPanel;
    private UnityAction resistorSaveAction;
    private UnityAction resistorCancelAction;
    private DeleteConfirmPanel deleteConfirmPanel;
    private SelectValuePanel selectCapacitorValuePanel;
    private SelectSingleValuePanel selectInductorValuePanel;
    private SliderValuePanel setResistorValuePanel;
    private UnityAction deleteYesAction;
    private UnityAction deleteCancelAction;
    private UnityAction capacitorSaveAction;
    private UnityAction capacitorCancelAction;
    private UnityAction inductorSaveAction;
    private UnityAction inductorCancelAction;
    private float componentValue;
	private Communication comm;
    private DrawVirtualWire wire;
	private bool deleteState;
    private string unit;

    bool resistorWindowDrag;
    bool capacitorWindowDrag;
    bool inductorWindowDrag;
    void Setup() {
        resistorWindowDrag = false;
        capacitorWindowDrag = false;
        inductorWindowDrag = false;
    }

	public void Start()
	{
		//Debug.Log("ComponentButton.cs - Start()");
		setWireObject();
		setCommunicationObject();
        deleteState = false;
	}

	public void setWireObject()
    {
        wire = GameObject.Find("DrawVirtualWires").GetComponent<DrawVirtualWire>();
        //wire = temp.GetComponent<ComponentObject>().getWireObject();
		//Debug.Log(wire.name);
    }

    public void setCommunicationObject()
    {
		comm = GameObject.Find("Communication").GetComponent<Communication>();
        //comm = temp.GetComponent<ComponentObject>().getCommunicationObject();
		//Debug.Log(comm.name);
    }

    void Awake () {
        setResistorValuePanel = SliderValuePanel.Instance();
        resistorSaveAction = new UnityAction (resistorSaveFunction);
        resistorCancelAction = new UnityAction (resistorCancelFunction);

        deleteConfirmPanel = DeleteConfirmPanel.Instance();
        deleteYesAction = new UnityAction (DeleteYesFunction);
        deleteCancelAction = new UnityAction (DeleteCancelFunction);

        selectCapacitorValuePanel = SelectValuePanel.Instance();
        capacitorSaveAction = new UnityAction (capacitorSaveFunction);
        capacitorCancelAction = new UnityAction (capacitorCancelFunction);

        selectInductorValuePanel = SelectSingleValuePanel.Instance();
        inductorSaveAction = new UnityAction (inductorSaveFunction);
        inductorCancelAction = new UnityAction (inductorCancelFunction);
    }

    public ComponentBase getComponentDataObject() {
        ComponentBase temp = transform.parent.GetComponent<Component>().getComponentData();
        return transform.parent.GetComponent<Component>().getComponentData();   
    }
     

    //  Send to the Modal Panel to set up the Buttons and Functions to call
    public void pop () {
        //Debug.Log("poped = " + comm.getPopupState());
        if(!comm.getPopupState() && !comm.getDeleteWireState()){
            if(transform.parent.name.Contains("resistor")) {
                setResistorValueWindow();
                comm.setPopupState(true);
                resistorWindowDrag = true;
            } else if(transform.parent.name.Contains("capacitor")) {
                setCapacitorValueWindow();
                comm.setPopupState(true);
                capacitorWindowDrag = true;
            } else if(transform.parent.name.Contains("inductor")) {
                setInductorValueWindow();
                comm.setPopupState(true);
                inductorWindowDrag = true;
            }
        }
    }

    public void setCapacitorValueWindow() {
        Capacitor capacitor = (Capacitor)getComponentDataObject();
        //Debug.Log("deleteOptionWindow " + transform.parent.name);
        selectCapacitorValuePanel.Choice (capacitorSaveAction, capacitorCancelAction, (int)componentValue);
        selectCapacitorValuePanel.setTitle(transform.parent.name);
        selectCapacitorValuePanel.setPosition(new Vector3(transform.position.x+150, transform.position.y+100, transform.position.z));
        selectCapacitorValuePanel.setUnit("nF");
        selectCapacitorValuePanel.setOptionValues(capacitor.optionValues);
        //Debug.Log("cc = " + (int)componentValue);
        selectCapacitorValuePanel.setSelectedToggle((int)componentValue);
        // if(selectCapacitorValuePanel.getSumOfSelectedValue() == 0)
        //     selectCapacitorValuePanel.resetToggles();
    }

    void capacitorSaveFunction () {
        comm.setPopupState(false);
        componentValue = selectCapacitorValuePanel.getSumOfSelectedValue();
        //seletedToggles = selectCapacitorValuePanel.getSelectedToggles();
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = componentValue.ToString() + "nF";
        comm.setPopupState(false);
        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = (int)componentValue;
        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
        capacitorWindowDrag = false;
    }

    void capacitorCancelFunction () {
        comm.setPopupState(false);
        capacitorWindowDrag = false;
    }

    public void setInductorValueWindow() {
        Inductor inductor = (Inductor)getComponentDataObject();
        //Debug.Log("deleteOptionWindow " + transform.parent.name);
        selectInductorValuePanel.Choice (inductorSaveAction, inductorCancelAction, (int)componentValue);
        selectInductorValuePanel.setTitle(transform.parent.name);
        selectInductorValuePanel.setPosition(new Vector3(transform.position.x+150, transform.position.y+100, transform.position.z));
        selectInductorValuePanel.setUnit("nF");
        selectInductorValuePanel.setOptionValues(inductor.optionValues);
        //Debug.Log("cc = " + (int)componentValue);
        selectInductorValuePanel.setSelectedToggle((int)componentValue);
        // if(selectCapacitorValuePanel.getSumOfSelectedValue() == 0)
        //     selectCapacitorValuePanel.resetToggles();
    }

    void inductorSaveFunction () {
        comm.setPopupState(false);
        componentValue = selectInductorValuePanel.getSelectedValue();
        //seletedToggles = selectCapacitorValuePanel.getSelectedToggles();
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = componentValue.ToString() + "nF";
        comm.setPopupState(false);
        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = (int)componentValue;
        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
        inductorWindowDrag = false;
    }

    void inductorCancelFunction () {
        comm.setPopupState(false);
        inductorWindowDrag = false;
    }

    // private Vector3 GetCurrentMousePosition()
    // {
    //     float distance = 1100;
    //     Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
    //     return Camera.main.ScreenToWorldPoint(mousePosition);
    // }

    // public void onMove(AxisEventData eventData) {
    //     Debug.Log("onMove");
    //     if(deleteState) {
    //         deleteConfirmPanel.setPosition(new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z));
    //     } else if(resistorWindowDrag) {
    //         setResistorValuePanel.setPosition(new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z));
    //     } else if(capacitorWindowDrag) {
    //         selectCapacitorValuePanel.setPosition(new Vector3(GetCurrentMousePosition().x, transform.position.y, GetCurrentMousePosition().z));
    //     }
    // }

    public void setResistorValueWindow()
    {
        Debug.Log("setResistorValueWindow ()");
        Resistor resistor = (Resistor)getComponentDataObject();
        Debug.Log("1 setResistorValueWindow ()");
        setResistorValuePanel.Choice (resistorSaveAction, resistorCancelAction);
        setResistorValuePanel.setUnit("K");
        setResistorValuePanel.setMinMax(resistor.minValue, resistor.totSteps);
        setResistorValuePanel.setSliderValue(componentValue, resistor.stepResolution, "K");
        setResistorValuePanel.setTitle(transform.parent.name);
        setResistorValuePanel.setPosition(new Vector3(transform.position.x+150, transform.position.y+100, transform.position.z));
    }

    void resistorSaveFunction () {
        Resistor resistor = (Resistor)getComponentDataObject();
        componentValue = setResistorValuePanel.getSliderValue();
        int value = (int)componentValue * resistor.stepResolution;
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = value.ToString() + "K";
        comm.setPopupState(false);
        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = value;
        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
    }

    void resistorCancelFunction () {
        comm.setPopupState(false);
    }

    public void deleteOptionWindow() {
        //Debug.Log("deleteOptionWindow " + transform.parent.name);
        deleteConfirmPanel.Choice (deleteYesAction, deleteCancelAction);
        deleteConfirmPanel.setTitle("Reset " + transform.parent.name + " ?");
        deleteConfirmPanel.setPosition(new Vector3(transform.position.x+150, transform.position.y+100, transform.position.z));
    }

    void DeleteYesFunction () {
        wire.removeWireWithComponent(transform.parent.name);
        ExitDeleteMode(DefaultPinSprite, true);
        comm.resetData();
        transform.parent.GetComponent<Component>().resetComponentData();
        // Todo: arduino에게 변경된 Json 보내기 (connection, value가 reset 됨)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        componentValue = result.value;
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = componentValue.ToString();
        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);

        wire.resetSourceObj();
        wire.resetTargetObj();
    }

    void DeleteCancelFunction () {
        ExitDeleteMode(ConnectedPinSprite, false);
        comm.resetData();
        wire.resetSourceObj();
        wire.resetTargetObj();
    }

    private string getTargetComponentPinName(string targetComponentPinName)
    {
        //string targetComponentPinName = comm.getTargetPin();
        int seperator = targetComponentPinName.IndexOf("-");
        string componentPinName = targetComponentPinName.Substring(seperator+1, targetComponentPinName.Length-seperator-1);
        //Debug.Log("[pin.cs] 10 getTargetComponentPinName() " + componentPinName);
        return componentPinName;
    }

    private string getTargetComponentName(string targetComponentPinName)
    {
        int seperator = targetComponentPinName.IndexOf("-");
        string componentName = targetComponentPinName.Substring(0, seperator);
        return componentName;
    }

    private GameObject getTargetComponentPinObject(string targetComponentPinName)
    {
        GameObject temp = GameObject.Find(getTargetComponentName(targetComponentPinName));
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            //Debug.Log("[pin.cs] 7 " + obj.name);
            if(obj.name == getTargetComponentPinName(targetComponentPinName)) {
                resultObj = obj.gameObject;
            }
        }
        //Debug.Log("[Pin.cs] 8 " + "getTargetComponentPinObject = " + resultObj.name);
        //Debug.Log("[Pin.cs] 9 " + "getTargetComponentPinObject = " + resultObj.transform.parent.name);
        return resultObj;
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

    public void EnterDeleteMode()
    {
        //Debug.Log("[Component.cs]" + "Enter Delete Mode");
        comm.setSourcePin(transform.parent.name);
		comm.setDeleteWireState(true, transform.parent.name);
        deleteState = true;
		
		getChildObject(transform.parent.name, name).GetComponent<Button>().image.sprite = DeleteModeComponentSprite;

        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if( wireObj.name.Contains(transform.parent.name) )
            {
				int start = wireObj.name.IndexOf(":") + 1;
				int end = wireObj.name.IndexOf(",");
				string sourcePinName = wireObj.name.Substring(start, end - start);
				//Debug.Log("Component.cs - 500000 " + sourcePinName);
				GameObject.Find(sourcePinName).GetComponent<Button>().image.sprite = DeleteModePinSprite;
				// int seperator = wireObj.name.IndexOf(",");
				// string targetComponentPinName = wireObj.name.Substring(seperator+1, wireObj.name.Length-seperator-1);
				// getTargetComponentPinObject(targetComponentPinName).GetComponent<Button>().image.sprite = DeleteModePinSprite;
            }
        }
    }

    public void ExitDeleteMode(Sprite sprite, bool delete)
    {
        //Debug.Log("[Component.cs]" + "Exit Delete Mode");
        comm.setDeleteWireState(false, transform.parent.name);
        deleteState = false;
		getChildObject(transform.parent.name, name).GetComponent<Button>().image.sprite = DefaultComponentSprite;

        GameObject[] temp = GameObject.FindGameObjectsWithTag("wire");
        foreach(GameObject wireObj in temp)
        {
            if( wireObj.name.Contains(transform.parent.name) )
            {
				int start = wireObj.name.IndexOf(":") + 1;
				int end = wireObj.name.IndexOf(",");
				string sourcePinName = wireObj.name.Substring(start, end - start);
				//Debug.Log("Component.cs - 500000 " + sourcePinName);
				GameObject.Find(sourcePinName).GetComponent<Button>().image.sprite = sprite;
                // string pinName = wireObj.name.Substring(4, wireObj.name.Length-4-name.Length);
                // Debug.Log(pinName);
                // Button btTempPin = GameObject.Find(pinName).GetComponent<Button>();
                // btTempPin.image.sprite = sprite;
            }
        }
    }

	public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Component.cs - OnPointerEnter");
		if (Input.GetMouseButtonDown(0))
        {
			comm.setStartTime(Time.time);
		}
    }

	public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            float releasedTime = Time.time;
            float pressedTime = comm.getStartTime();
            //Debug.Log("[pin.cs] " + "pressed time = " + pressedTime);
            //Debug.Log("[pin.cs] " + "release time = " + releasedTime);

			if ((releasedTime - pressedTime) > 0.3)
			{
				//long press
				if(!comm.getDeleteWireState() && !comm.getPopupState())
					EnterDeleteMode();
			} else {
				//short press
				if(deleteState) {
					deleteOptionWindow();
				} else {
                    if(!transform.parent.GetComponent<Component>().fixedComponent) pop();
				}
			}
		}
	}
}