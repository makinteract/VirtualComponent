using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Collections.Generic;

//  This script will be updated in Part 2 of this 2 part series.
public class SelectSingleValuePanel : MonoBehaviour
{
    public ToggleGroupCustom toggleGroup;
    public Button onButton;
    public Button editButton;
    //public Button closeButton;
    public Button plusFrequencyButton;
    public Button minusFrequencyButton;
    public Button plusAmplitudeButton;
    public Button minusAmplitudeButton;
    public Button plusDcOffsetButton;
    public Button minusDcOffsetButton;
    public GameObject modalPanelObject;
    public Slider frequencySlider;
    public Slider amplitudeSlider;
    public Slider dcOffsetSlider;
    private int selected;
    private string unit;
    private string[] waveTypes;
    private float frequency;
    private float amplitude;
    private float dcOffset;
    private string wavetype;

    private static SelectSingleValuePanel modalPanel;
    
    private List<ToggleCustom> selectedToggles;// = new List<ToggleCustom>();

    public static SelectSingleValuePanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (SelectSingleValuePanel)) as SelectSingleValuePanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Start() {
        //value = 0;
    }

    void Update() {
        //transform.position = 
    }

    public ToggleCustom currentSelection {
        get { return toggleGroup.ActiveToggles().FirstOrDefault(); }
    }

    public void PlusMinus() {
        plusFrequencyButton.onClick.RemoveAllListeners();
        plusFrequencyButton.onClick.AddListener (delegate {plusButton("frequency");});
        minusFrequencyButton.onClick.RemoveAllListeners();
        minusFrequencyButton.onClick.AddListener (delegate {minusButton("frequency");});
        plusAmplitudeButton.onClick.RemoveAllListeners();
        plusAmplitudeButton.onClick.AddListener (delegate {plusButton("amplitude");});
        minusAmplitudeButton.onClick.RemoveAllListeners();
        minusAmplitudeButton.onClick.AddListener (delegate {minusButton("amplitude");});
        plusDcOffsetButton.onClick.RemoveAllListeners();
        plusDcOffsetButton.onClick.AddListener (delegate {plusButton("dcOffset");});
        minusDcOffsetButton.onClick.RemoveAllListeners();
        minusDcOffsetButton.onClick.AddListener (delegate {minusButton("dcOffset");});

        plusFrequencyButton.gameObject.SetActive (true);
        minusFrequencyButton.gameObject.SetActive (true);
        plusAmplitudeButton.gameObject.SetActive (true);
        minusAmplitudeButton.gameObject.SetActive (true);
        plusDcOffsetButton.gameObject.SetActive (true);
        minusDcOffsetButton.gameObject.SetActive (true);
    }
    
    public void plusButton(string type) {
        switch(type){
            case "frequency":
            if(frequencySlider.value + 100 > frequencySlider.maxValue)  frequencySlider.value = frequencySlider.maxValue;
            else frequencySlider.value = (int)frequencySlider.value + 100;
            break;
            case "amplitude":
            if(amplitudeSlider.value + 100 > amplitudeSlider.maxValue)  amplitudeSlider.value = amplitudeSlider.maxValue;
            amplitudeSlider.value = (int)amplitudeSlider.value + 100;
            break;
            case "dcOffset":
            if(dcOffsetSlider.value + 100 > dcOffsetSlider.maxValue)  dcOffsetSlider.value = dcOffsetSlider.maxValue;
            if( (dcOffsetSlider.value > 0 && dcOffsetSlider.value < 100) || (dcOffsetSlider.value < 0 && dcOffsetSlider.value > -100) ) dcOffsetSlider.value = 0;
            dcOffsetSlider.value = (int)dcOffsetSlider.value + 100;
            break;
        }
    }

    public void minusButton(string type) {
        switch(type){
            case "frequency":
            if(frequencySlider.value > 100) frequencySlider.value = (int)frequencySlider.value - 100;
            else frequencySlider.value = frequencySlider.minValue;
            break;
            case "amplitude":
            if(amplitudeSlider.value > 100) amplitudeSlider.value = (int)amplitudeSlider.value - 100;
            else amplitudeSlider.value = amplitudeSlider.minValue;
            break;
            case "dcOffset":
            if(dcOffsetSlider.value > -1400) dcOffsetSlider.value = (int)dcOffsetSlider.value - 100;
            else dcOffsetSlider.value = dcOffsetSlider.minValue;
            break;
        }
    }
    
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction yesEvent, UnityAction cancelEvent, UnityAction editConnectionEvent, int _val) {
        modalPanelObject.SetActive (true);
        
        onButton.onClick.RemoveAllListeners();
        onButton.onClick.AddListener (yesEvent);
        //saveButton.onClick.AddListener (ClosePanel);
        
        // closeButton.onClick.RemoveAllListeners();
        // closeButton.onClick.AddListener (cancelEvent);
        // closeButton.onClick.AddListener (ClosePanel);

        onButton.gameObject.SetActive (true);
        // closeButton.gameObject.SetActive (true);

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(editConnectionEvent);
        editButton.gameObject.SetActive (true);

        frequencySlider.gameObject.SetActive(true);
        frequencySlider.onValueChanged.AddListener(delegate {FrequencyChangeCheck();});

        amplitudeSlider.gameObject.SetActive(true);
        amplitudeSlider.onValueChanged.AddListener(delegate {AmplitudeChangeCheck();});

        dcOffsetSlider.gameObject.SetActive(true);
        dcOffsetSlider.onValueChanged.AddListener(delegate {DcOffsetChangeCheck();});


        toggleGroup.gameObject.SetActive(true);

        List<ToggleCustom> togglesInGroup = toggleGroup.getTogglesInGroup();
        foreach(ToggleCustom t in togglesInGroup){
            t.onValueChanged.AddListener(delegate {ToggleValueChanged();});
        }
    }

    public void FrequencyChangeCheck() {
        frequency = (frequencySlider.value/100)*100;

        setFrequency(frequency);
        Text valueText = GameObject.Find("FreqValue").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)frequency, "frequency");

    }

    public void AmplitudeChangeCheck() {
        amplitude = (amplitudeSlider.value/100)*100;

        setAmplitude(amplitude);
        Text valueText = GameObject.Find("AmpValue").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)amplitude, "amplitude");
    }

    public void DcOffsetChangeCheck() {
        dcOffset = (dcOffsetSlider.value/100)*100;

        setDcOffset(dcOffset);
        Text valueText = GameObject.Find("DcOffsetValue").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)dcOffset, "DC");
    }

    public void setMinMax(Slider slider, float min, float max)
    {
        slider.minValue = min;
        slider.maxValue = max;
    }

    public float getFrequency() {
        return frequency;
    }

    public void setFrequency(float _frequency) {
        frequency = _frequency;
    }

    public float getAmplitude() {
        return amplitude;
    }

    public void setAmplitude(float _amplitude) {
        amplitude = _amplitude;
    }

    public float getDcOffset() {
        return dcOffset;
    }

    public void setDcOffset(float _dcOffset) {
        dcOffset = _dcOffset;
    }

    public string getWaveType() {
        return wavetype;
    }

    public void setWaveType(string _wavetype) {
        wavetype = _wavetype;
    }
    
    public void ToggleValueChanged()
    {
        string _value = "";
        int _selected = 0;
        foreach(ToggleCustom t in toggleGroup.ActiveToggles()){
            _selected = int.Parse(t.name.Substring(7, 1))-1;
            _value = waveTypes[int.Parse(t.name.Substring(7, 1))-1];
            //Debug.Log("waveType[ " + _selected + " ] = " + _value);
        }
        setWaveType(_value);
        setSelectedToggle(_selected);
    }

    public void setPrevSetting(int _selectedToggle, float _frequency, float _amplitude, float _dcOffset)
    {
        frequencySlider.value = _frequency;
        amplitudeSlider.value = _amplitude;
        dcOffsetSlider.value = _dcOffset;
        ToggleCustom toggle = GameObject.Find("Toggle0"+_selectedToggle).GetComponent<ToggleCustom>();
        toggle.isOn = true;
    }
    
    public int getSelectedToggle() {
        return selected;
    }

    public void setSelectedToggle(int _selected) {
        selected = _selected;
    }

/*
    public void setSelectedToggle(int _value)
    {
        int n = (int)Mathf.Log10(_value) + 1;
        List<int> selected = new List<int>();
        for(int i=0; i<n; i++, _value/=10) {
            int result = _value % 10;
            selected.Add(result);
        }
        if(selected.Count < 5) {
            for(int i=selected.Count; i<5; i++)
                selected.Add(0);
        }
        
        foreach(ToggleCustom t in toggleGroup.getTogglesInGroup()) {
            int index = int.Parse(t.name.Last().ToString()) - 1;
            Debug.Log("selected[" + index + "] = " + selected[index]);
        
            if(selected[index] == 1) {
                t.isOn = true;
            } else if(selected[index] == 0) {
                t.isOn = false;
            }
        }
    } */

    // public int getSelectedValue()
    // {
    //     //Debug.Log("value = " + value);
    //     return value;
    // }

    // public void setSelectedValue(int _val)
    // {
    //     value = _val;
    // }

    public void resetToggles()
    {
        toggleGroup.SetAllTogglesOff();
    }

    public void setOptionValues(string[] _optionValues)
    {
        waveTypes = _optionValues;
        int numberOfLables = 0;
        // make checkboxes
        Transform[] children = modalPanelObject.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name.Contains("ToggleValue")) {
                if(_optionValues[numberOfLables] == "") {
                    GameObject temp = obj.parent.gameObject;//.SetActive(false);
                    temp.SetActive(false);
                    toggleGroup.UnregisterToggle(temp.GetComponent<ToggleCustom>());
                } else {
                    obj.gameObject.GetComponent<Text>().text = _optionValues[numberOfLables++];//.ToString();
                }
            }
        }
    }
    
    public void setUnit(string type)
    {
        unit = type;
    }

    // public void setTitle(string title)
    // {
    //     Text titleText = GameObject.Find("SelectSingleValueTitle").GetComponent<Text>();
    //     titleText.text = title;
    // }

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    public void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}