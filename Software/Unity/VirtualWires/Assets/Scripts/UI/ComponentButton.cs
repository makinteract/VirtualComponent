using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class ComponentButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler//,IPointerEnterHandler
{
    public Sprite DeleteModePinSprite;
    public Sprite ConnectedPinSprite;
    public Sprite DefaultPinSprite;
    public Sprite DeleteModeComponentSprite;
    public Sprite DefaultComponentSprite;
    public Sprite onGoingButtonSprite;
    public Sprite offButtonSprite;
    public Sprite editOnSprite;
    public Sprite editOffSprite;
    public Sprite refreshingVoltmeter;
    //private ModalPanel modalPanel;
    private UnityAction resistorSaveAction;
    private UnityAction resistorCancelAction;
    private DeleteConfirmPanel deleteConfirmPanel;
    private SelectSingleValuePanel settingAwgPanel;
    private SelectValuePanel selectCapacitorValuePanel;
    private SelectValuePanel selectInductorValuePanel;
    private SliderValuePanel setResistorValuePanel;
    private VoltmeterPanel readVoltmeterPanel;
    private EditTogglePanel editTogglePanel;
    private UnityAction resetAllYesAction;
    private UnityAction resetAllCancelAction;
    private UnityAction resetAllStateAction;
    //private UnityAction constraintsSaveAction;
    private UnityAction capacitorSaveAction;
    private UnityAction capacitorCancelAction;
    private UnityAction inductorSaveAction;
    private UnityAction inductorCancelAction;
    private UnityAction awgOnAction;
    private UnityAction awgCloseAction;
    private UnityAction adcRefreshAction;
    private float componentValue;
    private ConstraintsHandler constraintsHandle;
	private Communication comm;
    private DrawVirtualWire wire;
    private WifiConnection wifi;
    private HttpRequest http;
	private bool deleteState;
    private string unit;
    private string awgIP;
    bool awgOn;
    private JObject awgData;

    bool resistorWindowDrag;
    bool capacitorWindowDrag;
    bool inductorWindowDrag;

    int capacitorToggleValue;
    int inductorToggleValue;
    int awgToggleValue;
    int constraintsSelectedValue;

    private bool editOn;
    private bool voltmeterPop;

    private bool editButonActive;

    private List<string> constraintsComponentList;

    void Setup() {
        resistorWindowDrag = false;
        capacitorWindowDrag = false;
        inductorWindowDrag = false;

        capacitorToggleValue = 0;
        inductorToggleValue = 0;
        constraintsSelectedValue = 0;

        editOn = false;
        voltmeterPop = false;
        editButonActive = false;
    }

	public void Start()
	{
        setWireObject();
		setCommunicationObject();
        setWifiObject();
        setHttpRequestObject();
        setConstraintsHandleObject();

        awgData = new JObject();
        awgData.Add("frequency", 0.0);
        awgData.Add("amplitude", 0.0);
        awgData.Add("dcOffset", 0.0);
        awgData.Add("selectedToggle", 0);

        awgOn = true;
        editOn = true;

        closeActivePopup();

        GameObject.Find("EditToggleButton").GetComponent<Button>().onClick.AddListener(HandleDeleteMode);
	}

    void Awake () {
        setResistorValuePanel = SliderValuePanel.Instance();
        resistorSaveAction = new UnityAction (resistorSaveFunction);
        resistorCancelAction = new UnityAction (resistorCancelFunction);        

        deleteConfirmPanel = DeleteConfirmPanel.Instance();
        resetAllYesAction = new UnityAction (resetAllYesFunction);
        resetAllCancelAction = new UnityAction (resetAllCancelFunction);

        selectCapacitorValuePanel = SelectValuePanel.Instance();
        capacitorSaveAction = new UnityAction (capacitorSaveFunction);
        capacitorCancelAction = new UnityAction (capacitorCancelFunction);

        selectInductorValuePanel = SelectValuePanel.Instance();
        inductorSaveAction = new UnityAction (inductorSaveFunction);
        inductorCancelAction = new UnityAction (inductorCancelFunction);

        settingAwgPanel = SelectSingleValuePanel.Instance();
        awgOnAction = new UnityAction (awgOnFunction);
        awgCloseAction = new UnityAction (awgCloseFunction);

        readVoltmeterPanel = VoltmeterPanel.Instance();
        adcRefreshAction = new UnityAction (adcRefreshFunction);

        editTogglePanel = EditTogglePanel.Instance();

        resetAllStateAction = new UnityAction (HandleDeleteMode);

        //constraintsSaveAction = new UnityAction (constraintsSaveFunction);
    }

    public void setWifiObject() {
        wifi = GameObject.Find("WifiConnection").GetComponent<WifiConnection>();
    }

    public void setHttpRequestObject() {
        http = GameObject.Find("HttpRequest").GetComponent<HttpRequest>();
    }

	public void setWireObject()
    {
        wire = GameObject.Find("DrawVirtualWires").GetComponent<DrawVirtualWire>();
        //wire = temp.GetComponent<ComponentObject>().getWireObject();
		//Debug.Log(wire.name);
    }

    public void setConstraintsHandleObject() {
        constraintsHandle = GameObject.Find("ConstraintsHandler").GetComponent<ConstraintsHandler>();
    }

    public void setCommunicationObject()
    {
		comm = GameObject.Find("Communication").GetComponent<Communication>();
        //comm = temp.GetComponent<ComponentObject>().getCommunicationObject();
		//Debug.Log(comm.name);
    }

    public ComponentBase getComponentDataObject() {
        ComponentBase temp = transform.parent.GetComponent<Component>().getComponentData();
        return transform.parent.GetComponent<Component>().getComponentData();   
    }
    
    public void closeActivePopup()
    {
        if(setResistorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SliderEditButton").GetComponent<Button>().image.sprite = editOffSprite;
            ExitDeleteMode(ConnectedPinSprite, false);
            setResistorValuePanel.ClosePanel();
        }
        if(selectCapacitorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOffSprite;
            ExitDeleteMode(ConnectedPinSprite, false);
            selectCapacitorValuePanel.ClosePanel();
        }
        if(selectInductorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOffSprite;
            ExitDeleteMode(ConnectedPinSprite, false);
            selectInductorValuePanel.ClosePanel();
        }
        if(settingAwgPanel.modalPanelObject.activeSelf) {
            GameObject.Find("AwgEditButton").GetComponent<Button>().image.sprite = editOffSprite;
            ExitDeleteMode(ConnectedPinSprite, false);
            settingAwgPanel.ClosePanel();
        }
    }

    //  Send to the Modal Panel to set up the Buttons and Functions to call
    public void pop () {
        //Debug.Log("poped");
        if(!comm.getPopupState() && !comm.getDeleteWireState()){
            if(transform.parent.name.Contains("ADC")) {
                    //closeActivePopup();
                    readAdcValueWindow();
                    //comm.setPopupState(true);
            } else if(transform.parent.GetComponent<Component>().fixedComponent) {
                //closeActivePopup();
                //Debug.Log("editToggleWindow");
                editToggleWindow();
                //comm.setPopupState(true);
            } else {
                if(transform.parent.name.Contains("resistor")) {
                    closeActivePopup();
                    setResistorValueWindow();
                    //comm.setPopupState(true);
                    resistorWindowDrag = true;
                } else if(transform.parent.name.Contains("capacitor")) {
                    closeActivePopup();
                    setCapacitorValueWindow();
                    //comm.setPopupState(true);
                    capacitorWindowDrag = true;
                } else if(transform.parent.name.Contains("inductor")) {
                    closeActivePopup();
                    setInductorValueWindow();
                    //comm.setPopupState(true);
                    inductorWindowDrag = true;
                } else if(transform.parent.name.Contains("AWG")) {
                    closeActivePopup();
                    setAwgValueWindow();
                    //comm.setPopupState(true);
                }
            }
        }
    }

    public void readAdcValueWindow() {
        readVoltmeterPanel.Choice(adcRefreshAction, resetAllStateAction);
        readVoltmeterPanel.setPosition(new Vector3(transform.position.x+200, transform.position.y+100, transform.position.z+10));
    }

    public void adcRefreshFunction() {
        if(GameObject.Find("ReadVoltageButton")) {
            GameObject.Find("ReadVoltageButton").GetComponent<Button>().image.sprite = refreshingVoltmeter;
        }
        wifi.sendDataEvent.Invoke(Query.getVoltage);
    }

    public void editToggleWindow() {
        editTogglePanel.Choice(resetAllStateAction);
        editTogglePanel.setPosition(new Vector3(transform.position.x+150, transform.position.y+100, transform.position.z+10));
    }

    public void setAwgValueWindow() {
        Awg awg = (Awg)getComponentDataObject();
        awgIP = awg.ip;
        comm.setAwgIP(awgIP);
        settingAwgPanel.Choice(awgOnAction, awgCloseAction, resetAllStateAction, (int)componentValue);
        settingAwgPanel.PlusMinus();
        //settingAwgPanel.setTitle("Wave Generator");
        settingAwgPanel.setPosition(new Vector3(135, 210, 225));
        settingAwgPanel.setOptionValues(awg.waveTypes); // now it is fixed menu
        //settingAwgPanel.setSelectedToggle(awgToggleValue);
        settingAwgPanel.setMinMax(settingAwgPanel.frequencySlider, 1000, 1000000000);
        settingAwgPanel.setMinMax(settingAwgPanel.amplitudeSlider, 0, 3000);
        settingAwgPanel.setMinMax(settingAwgPanel.dcOffsetSlider, (float)-1500, (float)1500);
        //Debug.Log("PrevSetting awgData :");
        //Debug.Log(awgData);
        settingAwgPanel.setPrevSetting((int)awgData["selectedToggle"], (float)awgData["frequency"], (float)awgData["amplitude"], (float)awgData["dcOffset"]);
    }

    public void awgSaveCurrSetting(int _selectedToggle, float _frequency, float _amplitude, float _dcOffset) {
        awgData["frequency"] = _frequency;
        awgData["amplitude"] = _amplitude;
        awgData["dcOffset"] = _dcOffset;
        awgData["selectedToggle"] = _selectedToggle;
    }

    public void awgOnFunction() {
        //comm.setPopupState(false);
        //componentValue = settingAwgPanel.getSelectedValue();
        float frequency = settingAwgPanel.getFrequency();
        float amplitude = settingAwgPanel.getAmplitude();
        float dcOffset = settingAwgPanel.getDcOffset();
        string waveType = settingAwgPanel.getWaveType();

        awgSaveCurrSetting(settingAwgPanel.getSelectedToggle(), frequency, amplitude, dcOffset);

        string resultText = waveType + "\n(";
        resultText += Util.changeUnit(frequency, "frequency") + ", ";
        resultText += Util.changeUnit(amplitude, "amplitude") + ", ";
        resultText += Util.changeUnit(dcOffset, "DC") + ")";

        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = resultText;

        JObject awgCommandJson = JObject.Parse(Query.awgCommand);
        awgCommandJson["awg"]["1"][0]["signalType"] = waveType;
        awgCommandJson["awg"]["1"][0]["signalFreq"] = (int)frequency;
        awgCommandJson["awg"]["1"][0]["vpp"] = (int)amplitude;
        awgCommandJson["awg"]["1"][0]["vOffset"] = (int)dcOffset;

        if(waveType == null) awgCommandJson["awg"]["1"][0]["signalType"] = "sine";
        else if(waveType == "dc") {
            awgCommandJson["awg"]["1"][0]["signalFreq"] = 0;
            awgCommandJson["awg"]["1"][0]["vpp"] = 0;
            settingAwgPanel.frequencySlider.value = 0;
            settingAwgPanel.amplitudeSlider.value = 0;
        }

        Debug.Log(awgCommandJson);

        String awgCommandString = awgCommandJson.ToString() + "\r\n";
        
        if(awgOn) { // on
            http.postJson(awgIP, awgCommandString);
            // after http.postJson received feedback ok, then automatically send awg run command from AwgHander.
            GameObject.Find("OnOffButton").GetComponent<Button>().image.sprite = onGoingButtonSprite;
            awgOn = false;
        } else { // off
            http.postJson(awgIP, Query.awgStop);
            awgOn = true;
            GameObject.Find("OnOffButton").GetComponent<Button>().image.sprite = offButtonSprite;
            // change button image
        }
        // // Todo: arduino에게 Json 보내기 (value 변경)
        // // Notify value info ComponentDataHandler -> notify BoardDataHandler
        // //                                        -> notify JsonHandler
        // ComponentBase result = getComponentDataObject();
        // result.value = selectCapacitorValuePanel.getSumOfSelectedBinaryValue(); //(int)componentValue;
        // result.changed.Add("set", "C");
        // result.changed.Add("id", result.id);
        // result.changed.Add("val", result.value);

        // transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
        // capacitorWindowDrag = false;
        // awgToggleValue = result.value;
    }

    public void awgCloseFunction() {
        //comm.setPopupState(false);
        //settingAwgPanel.setSelectedValue((int)componentValue);
    }

    // public void awgPlusFunction(){

    // }

    // public void awgMinusFunction(){

    // }

    public void setCapacitorValueWindow() {
        Capacitor capacitor = (Capacitor)getComponentDataObject();

        constraintsComponentList = new List<string>{"--------------"};
        //Debug.Log("constraintsHandle.getBoardId() = " + constraintsHandle.getBoardId());
        if(constraintsHandle.getBoardId() == "101") { // resistor 8
            //constraintsComponentList = new List<string> {"Resistor0", "Resistor1", "Resistor2", "Resistor3", "Resistor4", "Resistor5", "Resistor6", "Resistor7"};
            constraintsComponentList.Add("resistor0");
            constraintsComponentList.Add("resistor1");
            constraintsComponentList.Add("resistor2");
            constraintsComponentList.Add("resistor3");
            constraintsComponentList.Add("resistor4");
            constraintsComponentList.Add("resistor5");
            constraintsComponentList.Add("resistor6");
            constraintsComponentList.Add("resistor7");
            constraintsComponentList.Remove(transform.parent.name);
        } else if(constraintsHandle.getBoardId() == "102") {
            //constraintsComponentList = new List<string> {"Resistor0", "Resistor1", "Capacitor2", "Capacitor3", "Voltmeter"};
            constraintsComponentList.Add("resistor0");
            constraintsComponentList.Add("resistor1");
            constraintsComponentList.Remove(transform.parent.name);
        }

        //Debug.Log("deleteOptionWindow " + transform.parent.name);
        selectCapacitorValuePanel.Choice (capacitorSaveAction, capacitorCancelAction, resetAllStateAction, constraintsComponentList, transform.parent.name);
        selectCapacitorValuePanel.setTitle("SelectValuePanelTitle", transform.parent.name);
        selectCapacitorValuePanel.setPosition(new Vector3(0, 210, 250));
        selectCapacitorValuePanel.setComponent("capacitor");
        selectCapacitorValuePanel.setOptionValues(capacitor.optionValues);
        //Debug.Log("cc = " + (int)componentValue);
        selectCapacitorValuePanel.setSelectedToggle(capacitorToggleValue);
        selectCapacitorValuePanel.setSelectedIndexValue(capacitorToggleValue);

        if(constraintsSelectedValue == 0) {
            for(int i=0; i<constraintsComponentList.Count; i++) {
                if(constraintsComponentList[i] == constraintsHandle.getConstrainSource(transform.parent.name))
                    constraintsSelectedValue = i;
            }
        } else if(!constraintsHandle.containsConstrainSource(transform.parent.name) && !constraintsHandle.containsConstrainTarget(transform.parent.name)) {
            constraintsSelectedValue = 0;
        }

        //Debug.Log("constraintsSelectedValue = " + constraintsSelectedValue);

        if(constraintsSelectedValue>0) setFrequencyInfoDisplay(true, "SelectValuePanel");
        else setFrequencyInfoDisplay(false, "SelectValuePanel");

        selectCapacitorValuePanel.setConstraintsSelected(constraintsSelectedValue);
        // if(selectCapacitorValuePanel.getSumOfSelectedValue() == 0)
        //     selectCapacitorValuePanel.resetToggles();
        //Debug.Log("x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
    }

    void capacitorSaveFunction () {
        //comm.setPopupState(false);
        componentValue = selectCapacitorValuePanel.getSumOfSelectedValue();
        //seletedToggles = selectCapacitorValuePanel.getSelectedToggles();
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = Util.changeUnit(componentValue, "capacitor");
        //comm.setPopupState(false);

        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = selectCapacitorValuePanel.getSumOfSelectedBinaryValue(); //(int)componentValue;
        result.changed.Add("set", "C");
        result.changed.Add("id", result.id);
        result.changed.Add("val", result.value);
        result.changed.Add("state", "userRequest");

        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
        capacitorWindowDrag = false;
        capacitorToggleValue = result.value;

        constraintsSelectedValue = selectCapacitorValuePanel.getConstraintsSelected();

        //Text frequencyConstraintsLabel = Util.getChildObject("SelectValuePanel", "FrequencyConstraintsLabel").GetComponent<Text>();
        
        if(constraintsSelectedValue == 0) {
            constraintsHandle.removeConstrain(transform.parent.name);
            setFrequencyInfoDisplay(false, "SelectValuePanel");
            //frequencyConstraintsLabel.text = "";
            //Util.getChildObject("SelectValuePanel", "FrequencyConstraintsValue").GetComponent<Text>().text = "";
        } else {
            constraintsHandle.addConstrain(transform.parent.name, constraintsComponentList[constraintsSelectedValue]);
            setFrequencyInfoDisplay(true, "SelectValuePanel");
            //frequencyConstraintsLabel.text = "Frequency : ";
            //GameObject.Find("FrequencyConstraintsLabel").GetComponent<Text>().text = "Frequency : ";
        }
    }

    void setFrequencyInfoDisplay(bool set, string panelName) {
        Text frequencyConstraintsLabel = Util.getChildObject(panelName, "FrequencyConstraintsLabel").GetComponent<Text>();
        Text frequencyConstraintsValue = Util.getChildObject(panelName, "FrequencyConstraintsValue").GetComponent<Text>();

        if(set) {
            frequencyConstraintsLabel.text = "Frequency : ";
        } else {
            frequencyConstraintsLabel.text = "";
            frequencyConstraintsValue.text = "";
        }
    }

    void capacitorCancelFunction () {
        //comm.setPopupState(false);
        capacitorWindowDrag = false;
        selectCapacitorValuePanel.setSelectedValue((int)componentValue);
        selectCapacitorValuePanel.setSelectedIndexValue(capacitorToggleValue);
    }

    // void constraintsSaveFunction() {
    //     if(transform.parent.name.Contains("capacitor")) {
    //         constraintsSelectedValue = selectCapacitorValuePanel.getConstraintsSelected(transform.parent.name);
    //     } else if(transform.parent.name.Contains("resistor")) {
    //         constraintsSelectedValue = setResistorValuePanel.getConstraintsSelected(transform.parent.name);
    //     }
    // }

    public void setInductorValueWindow() {
        Inductor inductor = (Inductor)getComponentDataObject();
        //Debug.Log("deleteOptionWindow " + transform.parent.name);
        selectInductorValuePanel.Choice (inductorSaveAction, inductorCancelAction, resetAllStateAction);
        selectInductorValuePanel.setTitle("SelectValuePanelTitle", transform.parent.name);
        selectInductorValuePanel.setPosition(new Vector3(0, 210, 250));
        selectInductorValuePanel.setComponent("inductor");
        selectInductorValuePanel.setOptionValues(inductor.optionValues);
        //Debug.Log("cc = " + (int)componentValue);
        selectInductorValuePanel.setSelectedToggle(inductorToggleValue);
        selectInductorValuePanel.setSelectedIndexValue(inductorToggleValue);
        selectInductorValuePanel.setDropdownActive(false);
        // if(selectCapacitorValuePanel.getSumOfSelectedValue() == 0)
        //     selectCapacitorValuePanel.resetToggles();
        //Debug.Log("x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
    }

    void inductorSaveFunction () {
        //comm.setPopupState(false);
        componentValue = selectInductorValuePanel.getSumOfSelectedValue();
        //seletedToggles = selectCapacitorValuePanel.getSelectedToggles();
        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = Util.changeUnit(componentValue, "inductor");
        //comm.setPopupState(false);
        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = selectInductorValuePanel.getSumOfSelectedBinaryValue();//(int)componentValue;
        
        result.changed.Add("set", "L");
        result.changed.Add("id", result.id);
        result.changed.Add("val", result.value);

        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);
        inductorWindowDrag = false;
        inductorToggleValue = result.value;
    }

    void inductorCancelFunction () {
        //comm.setPopupState(false);
        inductorWindowDrag = false;
        selectInductorValuePanel.setSelectedValue((int)componentValue);
        selectInductorValuePanel.setSelectedIndexValue(inductorToggleValue);
    }

    // private int findItemsInList(List<string> list, string toFind) {
    //     for (int i = 0; i < list.Count; i++) 
    //     if (list[i] == toFind) return i;
    //     return -1;
    // }

    public void setResistorValueWindow()
    {
        //Debug.Log("setResistorValueWindow ()");
        constraintsComponentList = new List<string>{"--------------"};
        Resistor resistor = (Resistor)getComponentDataObject();
        //Debug.Log("constraintsHandle.getBoardId() = " + constraintsHandle.getBoardId());
        if(constraintsHandle.getBoardId() == "101") { // resistor 8
            //constraintsComponentList = new List<string> {"Resistor0", "Resistor1", "Resistor2", "Resistor3", "Resistor4", "Resistor5", "Resistor6", "Resistor7"};
            constraintsComponentList.Add("resistor0");
            constraintsComponentList.Add("resistor1");
            constraintsComponentList.Add("resistor2");
            constraintsComponentList.Add("resistor3");
            constraintsComponentList.Add("resistor4");
            constraintsComponentList.Add("resistor5");
            constraintsComponentList.Add("resistor6");
            constraintsComponentList.Add("resistor7");
            constraintsComponentList.Remove(transform.parent.name);
        } else if(constraintsHandle.getBoardId() == "102") {
            //constraintsComponentList = new List<string> {"Resistor0", "Resistor1", "Capacitor2", "Capacitor3", "Voltmeter"};
            constraintsComponentList.Add("resistor0");
            constraintsComponentList.Add("resistor1");
            constraintsComponentList.Add("capacitor2");
            constraintsComponentList.Add("capacitor3");
            //constraintsComponentList.Add("voltmeter");
            //if(transform.parent.name == "ADC6") constraintsComponentList.Remove("voltmeter6");
            //else
            constraintsComponentList.Remove(transform.parent.name);
        }

        setResistorValuePanel.Choice (resistorSaveAction, resistorCancelAction, resetAllStateAction, constraintsComponentList, transform.parent.name);
        //setResistorValuePanel.setUnit("K");
        //setResistorValuePanel.setMinMax(resistor.minValue, resistor.totSteps);
        setResistorValuePanel.setMinMax(0, resistor.totSteps-1);
        //setResistorValuePanel.setSliderValue(componentValue*resistor.stepResistance, resistor.stepResistance, "K");
        setResistorValuePanel.setSliderValue(resistor.minValue, resistor.value, resistor.stepResistance, "K");
        setResistorValuePanel.setTitle("SliderValuePanelTitle", transform.parent.name);
        setResistorValuePanel.setPosition(new Vector3(-100,210,200));
        
        if(constraintsSelectedValue == 0) {
            if(constraintsHandle.containsConstrainTarget(transform.parent.name)) {
                for(int i=0; i<constraintsComponentList.Count; i++) {
                    if(constraintsComponentList[i] == constraintsHandle.getConstrainSource(transform.parent.name))
                        constraintsSelectedValue = i;
                }
            }
        } else if(!constraintsHandle.containsConstrainSource(transform.parent.name) && !constraintsHandle.containsConstrainTarget(transform.parent.name)) {
            constraintsSelectedValue = 0;
        }

        //Debug.Log("constraintsSelectedValue = " + constraintsSelectedValue);

        if(constraintsSelectedValue>0) setFrequencyInfoDisplay(true, "SliderValuePanel");
        else setFrequencyInfoDisplay(false, "SliderValuePanel");

        setResistorValuePanel.setConstraintsSelected(constraintsSelectedValue);
        //setResistorValuePanel.setPosition(new Vector3(transform.position.x,transform.position.y+100, transform.position.z));
        //Debug.Log("x = " + (transform.position.x) + " y = " + (transform.position.y+100) + " z = " + transform.position.z);
    }

    void resistorSaveFunction () {
        Resistor resistor = (Resistor)getComponentDataObject();
        componentValue = setResistorValuePanel.getSliderValue();
        int value = resistor.minValue + (int)componentValue * resistor.stepResistance;

        getChildObject(transform.parent.name, "ValueText").GetComponent<Text>().text = Util.changeUnit(value, "resistor");
        //comm.setPopupState(false);
        // Todo: arduino에게 Json 보내기 (value 변경)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler
        ComponentBase result = getComponentDataObject();
        result.value = (int)componentValue;

        result.changed.Add("set", "R");
        result.changed.Add("id", result.id);
        result.changed.Add("val", result.value);
        result.changed.Add("state", "userRequest");

        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(result);

        constraintsSelectedValue = setResistorValuePanel.getConstraintsSelected();

        //Text frequencyConstraintsLabel = Util.getChildObject("SliderValuePanel", "FrequencyConstraintsLabel").GetComponent<Text>();
        //Text frequencyConstraintsValue = Util.getChildObject("SliderValuePanel", "FrequencyConstraintsValue").GetComponent<Text>();
        if(constraintsSelectedValue == 0) {
            //Debug.Log("remove constraints for " + _componentName);
            constraintsHandle.removeConstrain(transform.parent.name);
            //frequencyConstraintsLabel.text = "";
            //frequencyConstraintsValue.text = "";
            setFrequencyInfoDisplay(false, "SliderValuePanel");
        } else if(constraintsComponentList[constraintsSelectedValue].Contains("resistor")) {
            constraintsHandle.addConstrain(transform.parent.name, constraintsComponentList[constraintsSelectedValue]);
            setFrequencyInfoDisplay(false, "SliderValuePanel");
            //frequencyConstraintsLabel.text = "";
            //frequencyConstraintsValue.text = "";
        } else {
            constraintsHandle.addConstrain(transform.parent.name, constraintsComponentList[constraintsSelectedValue]);
            //frequencyConstraintsLabel.text = "Frequency : ";
            setFrequencyInfoDisplay(true, "SliderValuePanel");
            //GameObject.Find("FrequencyConstraintsLabel").GetComponent<Text>().text = "Frequency : ";
        }
    }

    void resistorCancelFunction () {
        //comm.setPopupState(false);
    }

    public void deleteOptionWindow() {
        //string title = "Reset " + transform.parent.name + " ?";
        deleteConfirmPanel.Choice (resetAllYesAction, resetAllCancelAction);
        deleteConfirmPanel.setTitle("Reset " + transform.parent.name + " ?");
        deleteConfirmPanel.setPosition(new Vector3(transform.position.x+200, transform.position.y+100, transform.position.z+10));
    }

    void resetAllYesFunction () {
        wire.removeWireWithComponent(transform.parent.name);
        ExitDeleteMode(DefaultPinSprite, true);
        //Debug.Log("after exit deletemode in resetAllYesFunction()");

        //{"set":"X2", "on": 1, "value": [{"M":0, "B":1}, {"M":1, "B":2}]}

        ComponentBase data = getComponentDataObject();
        JArray connection = new JArray();
        JObject left = new JObject();
        JObject right = new JObject();
        left.Add("M", data.connection[0]["M"]);
        left.Add("B", data.connection[0]["B"]);
        right.Add("M", data.connection[1]["M"]);
        right.Add("B", data.connection[1]["B"]);

        connection.Add(left);
        connection.Add(right);

        data.changed.Add("set", "X2");
        data.changed.Add("on", 0);
        data.changed.Add("value", connection);

        int savedValue = data.value;
        Debug.Log("reset component json: ");
        Debug.Log(data.changed);

        comm.resetData();
        transform.parent.GetComponent<Component>().resetComponentData();
        ComponentBase initData = getComponentDataObject();
        initData.value = savedValue;
        // Todo: arduino에게 변경된 Json 보내기 (connection, value가 reset 됨)
        // Notify value info ComponentDataHandler -> notify BoardDataHandler
        //                                        -> notify JsonHandler

        transform.parent.GetComponent<Component>().updateComponentDataEvent.Invoke(data);
        wire.resetBoardPinObj();
        wire.resetComponentPinObj();
    }

    void resetAllCancelFunction () {
        //ExitDeleteMode(ConnectedPinSprite, false);
        comm.resetData();
        wire.resetBoardPinObj();
        wire.resetComponentPinObj();
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

    public void HandleDeleteMode() {
        if(editOn) {
            EnterDeleteMode();
            editOn = false;
        } else {
            ExitDeleteMode(ConnectedPinSprite, false);
            editOn = true;
        }
    }

    public void EnterDeleteMode()
    {
        if(setResistorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SliderEditButton").GetComponent<Button>().image.sprite = editOnSprite;
        }
        if(selectCapacitorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOnSprite;
        }
        if(selectInductorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOnSprite;
        }
        if(settingAwgPanel.modalPanelObject.activeSelf) {
            GameObject.Find("AwgEditButton").GetComponent<Button>().image.sprite = editOnSprite;
        }
        if(readVoltmeterPanel.modalPanelObject.activeSelf) {
            GameObject.Find("VoltmeterEditButton").GetComponent<Button>().image.sprite = editOnSprite;
        }
        if(editTogglePanel.modalPanelObject.activeSelf) {
            GameObject.Find("EditToggleButton").GetComponent<Button>().image.sprite = editOnSprite;
        }

        //Debug.Log("[Component.cs]" + "Enter Delete Mode");
        //comm.setSourcePin(transform.parent.name); ==>?????
        comm.setComponentPin(transform.parent.name);
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
        if(setResistorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SliderEditButton").GetComponent<Button>().image.sprite = editOffSprite;
        }
        if(selectCapacitorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOffSprite;
        }
        if(selectInductorValuePanel.modalPanelObject.activeSelf) {
            GameObject.Find("SelectEditButton").GetComponent<Button>().image.sprite = editOffSprite;
        }
        if(settingAwgPanel.modalPanelObject.activeSelf) {
            GameObject.Find("AwgEditButton").GetComponent<Button>().image.sprite = editOffSprite;
        }
        if(readVoltmeterPanel.modalPanelObject.activeSelf) {
            GameObject.Find("VoltmeterEditButton").GetComponent<Button>().image.sprite = editOffSprite;
        }
        if(editTogglePanel.modalPanelObject.activeSelf) {
            GameObject.Find("EditToggleButton").GetComponent<Button>().image.sprite = editOffSprite;
        }

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

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("*****ComponentButton down");
        comm.setStartTime(Time.time);
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
                    //Debug.Log("component Name = " + transform.parent.name);
                    if(transform.parent.name.Contains("ADC")) {
                        //wifi.sendDataEvent.Invoke(Query.getVoltage);
                        if(voltmeterPop) {
                            readVoltmeterPanel.ClosePanel();
                            voltmeterPop = false;
                        } else {
                            pop();
                            // if(GameObject.Find("ReadVoltageButton")) {
                            //     GameObject.Find("ReadVoltageButton").GetComponent<Button>().image.sprite = refreshingVoltmeter;
                            // }
                            voltmeterPop = true;
                        }
                    } else if(transform.parent.GetComponent<Component>().fixedComponent) {
                        //Debug.Log("fixed");
                        if(editButonActive) {
                            editTogglePanel.ClosePanel();
                            editButonActive = false;
                        } else {
                            pop();
                            editButonActive = true;
                        }
                        //HandleDeleteMode();
                    } else pop();
				}
			}
		}
	}
}