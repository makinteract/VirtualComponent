using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Collections.Generic;

//  This script will be updated in Part 2 of this 2 part series.
public class SelectValuePanel : MonoBehaviour
{
    public ConstraintsHandler constraintsHandle;
    public Dropdown constraintsDropdown;
    public ToggleGroupCustom toggleGroup;
    public Button saveButton;
    //public Button noButton;
    // public Button cancelButton;
    public Button editButton;
    // public Button constraintsSaveButton;
    public GameObject modalPanelObject;
    private int dropdownSelectedValue;
    private int value;
    private int indexSumValue;
    private string component;
    private int[] optionValues;

    private static SelectValuePanel modalPanel;
    
    private List<ToggleCustom> selectedToggles;// = new List<ToggleCustom>();

    public static SelectValuePanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (SelectValuePanel)) as SelectValuePanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Start() {
        value = 0;
        dropdownSelectedValue = 0;
    }

    void Update() {
        //transform.position = 
    }

    public ToggleCustom currentSelection {
        get { return toggleGroup.ActiveToggles().FirstOrDefault(); }
    }

    // public void ResetComponent(UnityAction resetAllEvent) {
    //     resetButton.onClick.RemoveAllListeners();
    //     resetButton.onClick.AddListener(resetAllEvent);
    //     resetButton.gameObject.SetActive (true);
    // }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction yesEvent, UnityAction cancelEvent, UnityAction editConnectionEvent) {
        modalPanelObject.SetActive (true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener (yesEvent);
        //saveButton.onClick.AddListener (ClosePanel);
        saveButton.gameObject.SetActive (true);
        
        // cancelButton.onClick.RemoveAllListeners();
        // cancelButton.onClick.AddListener (cancelEvent);
        // cancelButton.onClick.AddListener (ClosePanel);
        // cancelButton.gameObject.SetActive (true);

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(editConnectionEvent);
        editButton.gameObject.SetActive (true);

        toggleGroup.gameObject.SetActive(true);

        List<ToggleCustom> togglesInGroup = toggleGroup.getTogglesInGroup();
        foreach(ToggleCustom t in togglesInGroup){
            t.onValueChanged.AddListener(delegate {ToggleValueChanged(component);});
        }
    }

    public void Choice (UnityAction yesEvent, UnityAction cancelEvent, UnityAction editConnectionEvent, List<string> dropOptions, string componentName) {
        modalPanelObject.SetActive (true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener (yesEvent);
        //saveButton.onClick.AddListener (ClosePanel);
        
        // cancelButton.onClick.RemoveAllListeners();
        // cancelButton.onClick.AddListener (cancelEvent);
        // cancelButton.onClick.AddListener (ClosePanel);

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(editConnectionEvent);
        editButton.gameObject.SetActive (true);
        
        // constraintsSaveButton.onClick.RemoveAllListeners();
        // constraintsSaveButton.onClick.AddListener(constraintsSaveEvent);
        // constraintsSaveButton.gameObject.SetActive (true);

        saveButton.gameObject.SetActive (true);
        //noButton.gameObject.SetActive (true);
        // cancelButton.gameObject.SetActive (true);

        toggleGroup.gameObject.SetActive(true);

        List<ToggleCustom> togglesInGroup = toggleGroup.getTogglesInGroup();
        foreach(ToggleCustom t in togglesInGroup){
            t.onValueChanged.AddListener(delegate {ToggleValueChanged(component);});
        }

        if(dropOptions[0] != null) {
            constraintsDropdown.gameObject.SetActive(true);
            GameObject.Find("ConstraintsLabel").GetComponent<Text>().text = "Constraints on";
            constraintsDropdown.ClearOptions();
            constraintsDropdown.AddOptions(dropOptions);
            constraintsDropdown.onValueChanged.AddListener(delegate {
                SelectConstraints(dropOptions, componentName);
            });
        } else {
            // constraintsDropdown.gameObject.SetActive(false);
            // GameObject.Find("ConstraintsLabel").GetComponent<Text>().text = "";
            // GameObject.Find("FrequencyConstraintsLabel").GetComponent<Text>().text = "";
            setDropdownActive(false);
        }
    }
    
    public void ToggleValueChanged(string component)
    {
        int _value = 0;
        int _indexSumValue = 0;
        int[] indexBinaryValue = {1,2,4,8,16,32,64,128};
        //selectedToggles = new List<ToggleCustom>();
        foreach(ToggleCustom t in toggleGroup.ActiveToggles()){
            int toggleNumber = int.Parse(t.name.Substring(7, 1))-1;
            _value += optionValues[toggleNumber];
            _indexSumValue += indexBinaryValue[toggleNumber];
        }
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)_value, component);
        value = _value;
        indexSumValue = _indexSumValue;
    }

    public void SelectConstraints(List<string> _dropOptions, string _componentName) {
        dropdownSelectedValue = constraintsDropdown.value;
        // Text frequencyConstraintsLabel = Util.getChildObject("SelectValuePanel", "FrequencyConstraintsLabel").GetComponent<Text>();
        
        // if(dropdownSelectedValue == 0) {
        //     constraintsHandle.removeConstrain(_componentName);
        //     frequencyConstraintsLabel.text = "";
        //     Util.getChildObject("SelectValuePanel", "FrequencyConstraintsValue").GetComponent<Text>().text = "";
        // } else {
        //     constraintsHandle.addConstrain(_componentName, _dropOptions[dropdownSelectedValue]);
        //     frequencyConstraintsLabel.text = "Frequency : ";
        //     //GameObject.Find("FrequencyConstraintsLabel").GetComponent<Text>().text = "Frequency : ";
        // }
    }

    public void setConstraintsSelected(int _val) {
        dropdownSelectedValue = _val;
        constraintsDropdown.value = _val;
    }
    
    public int getConstraintsSelected() {
        return dropdownSelectedValue;
    }

    public void setSelectedToggle(int _value)
    {
        indexSumValue = _value;
        int[] selected = new int[8];
        string binary = Convert.ToString(indexSumValue, 2);

        //Debug.Log("indexSumValue dec = " + indexSumValue);
        //Debug.Log("indexSumValue bin = " + binary);
        //Debug.Log("length = " + binary.Length);

        for(int i=0; i<binary.Length; i++) {
            selected[i] = Convert.ToInt32(binary.ElementAt(binary.Length-1-i).ToString());
            //Debug.Log("selected[" + i + "] = " + selected[i]);
        }
        for(int i = binary.Length; i<8; i++) {
            selected[i] = 0;
            //Debug.Log("? selected[" + i + "] = " + selected[i]);
        }
        
        foreach(ToggleCustom t in toggleGroup.getTogglesInGroup()) {
            int index = int.Parse(t.name.Last().ToString()) - 1;
            //Debug.Log("selected[" + index + "] = " + selected[index]);
        
            if(selected[index] == 1) {
                t.isOn = true;
            } else if(selected[index] == 0) {
                t.isOn = false;
            }
        }
    }

    public void setDropdownActive(bool state)
    {
        constraintsDropdown.gameObject.SetActive(state);
        Util.getChildObject("SelectValuePanel", "ConstraintsLabel").GetComponent<Text>().text = "";
        Util.getChildObject("SelectValuePanel", "FrequencyConstraintsLabel").GetComponent<Text>().text = "";
        Util.getChildObject("SelectValuePanel", "FrequencyConstraintsValue").GetComponent<Text>().text = "";
    }

    public int getSumOfSelectedValue()
    {
        //Debug.Log("value = " + value);
        return value;
    }

    public int getSumOfSelectedBinaryValue()
    {
        //Debug.Log("value = " + value);
        return indexSumValue;
    }

    public void setSelectedValue(int _val)
    {
        value = _val;
    }

    public void setSelectedIndexValue(int _val)
    {
        indexSumValue = _val;
    }

    public void resetToggles()
    {
        toggleGroup.SetAllTogglesOff();
    }

    public void setOptionValues(int[] _optionValues)
    {
        optionValues = _optionValues;
        int numberOfLables = 0;

        // make checkboxes
        Transform[] children = modalPanelObject.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name.Contains("ToggleValue")) {
                if(_optionValues[numberOfLables] == 0) {
                    GameObject temp = obj.parent.gameObject;//.SetActive(false);
                    temp.SetActive(false);
                    toggleGroup.UnregisterToggle(temp.GetComponent<ToggleCustom>());
                } else {
                    obj.gameObject.GetComponent<Text>().text = Util.changeUnit((float)_optionValues[numberOfLables++], component);
                }
            }
        }
    }
    
    public void setComponent(string type)
    {
        component = type;
    }

    public void setTitle(string target, string title)
    {
        Text titleText = GameObject.Find(target).GetComponent<Text>();
        titleText.text = title;
    }

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    public void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}