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
    public ToggleGroupCustom toggleGroup;
    public Button saveButton;
    //public Button noButton;
    public Button cancelButton;
    public GameObject modalPanelObject;
    private int value;
    private string unit;
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
    }

    void Update() {
        //transform.position = 
    }

    public ToggleCustom currentSelection {
        get { return toggleGroup.ActiveToggles().FirstOrDefault(); }
    }
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction yesEvent, UnityAction cancelEvent, int _val) {
        modalPanelObject.SetActive (true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener (yesEvent);
        saveButton.onClick.AddListener (ClosePanel);
        
        //noButton.onClick.RemoveAllListeners();
        //noButton.onClick.AddListener (noEvent);
        //noButton.onClick.AddListener (ClosePanel);
        
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener (cancelEvent);
        cancelButton.onClick.AddListener (ClosePanel);

        //this.question.text = question;

        //this.iconImage.gameObject.SetActive (false);
        saveButton.gameObject.SetActive (true);
        //noButton.gameObject.SetActive (true);
        cancelButton.gameObject.SetActive (true);

        toggleGroup.gameObject.SetActive(true);

        List<ToggleCustom> togglesInGroup = toggleGroup.getTogglesInGroup();
        foreach(ToggleCustom t in togglesInGroup){
            t.onValueChanged.AddListener(delegate {ToggleValueChanged(unit);});
        }
    }
    
    public void ToggleValueChanged(string unit)
    {
        int _value = 0;
        selectedToggles = new List<ToggleCustom>();
        foreach(ToggleCustom t in toggleGroup.ActiveToggles()){
            _value += optionValues[int.Parse(t.name.Substring(7, 1))-1];
        }
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = _value.ToString() + unit;
        value = _value;
    }

    public List<ToggleCustom> getSelectedToggles()
    {
        return selectedToggles;
    }

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
    }

    public int getSumOfSelectedValue()
    {
        //Debug.Log("value = " + value);
        return value;
    }

    public void setSelectedValue(int _val)
    {
        value = _val;
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
                    obj.gameObject.GetComponent<Text>().text = _optionValues[numberOfLables++].ToString();
                }
            }
        }
    }
    
    public void setUnit(string type)
    {
        unit = type;
    }

    public void setTitle(string title)
    {
        Text titleText = GameObject.Find("Title").GetComponent<Text>();
        titleText.text = title;
    }

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
    private Vector3 GetCurrentMousePosition()
    {
        float distance = 1200;//GameObject.Find(comm.getSourcePin()).transform.position.z;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}