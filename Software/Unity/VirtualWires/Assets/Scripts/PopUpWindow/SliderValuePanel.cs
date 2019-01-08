using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//  This script will be updated in Part 2 of this 2 part series.
public class SliderValuePanel : MonoBehaviour
{
    public ConstraintsHandler constraintsHandle;
    public Dropdown constraintsDropdown;
    public Slider valueSlider;
    public Button saveButton;
    //public Button noButton;
    // public Button cancelButton;
    public Button editButton;
    // public Button constraintsSaveButton;
    public GameObject modalPanelObject;
    //private float value;
    
    private static SliderValuePanel modalPanel;

    //private string unit;
    private int unitPerStep;
    private int offset;
    private int dropdownSelectedValue;
    //private float minValue;
    //private float maxValue;
    
    public static SliderValuePanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (SliderValuePanel)) as SliderValuePanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Start() {
        dropdownSelectedValue = 0;
    }
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction saveEvent, UnityAction cancelEvent, UnityAction editConnectionEvent, List<string> dropOptions, string componentName) {
        modalPanelObject.SetActive (true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener (saveEvent);
        saveButton.gameObject.SetActive (true);
        //saveButton.onClick.AddListener (ClosePanel);
        
        // cancelButton.onClick.RemoveAllListeners();
        // cancelButton.onClick.AddListener (cancelEvent);
        // cancelButton.onClick.AddListener (ClosePanel);
        // cancelButton.gameObject.SetActive (true);

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(editConnectionEvent);
        editButton.gameObject.SetActive (true);

        // constraintsSaveButton.onClick.RemoveAllListeners();
        // constraintsSaveButton.onClick.AddListener(constraintsSaveEvent);
        // constraintsSaveButton.gameObject.SetActive (true);

        valueSlider.gameObject.SetActive(true);
        valueSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(unitPerStep, offset);});

        if(dropOptions[0] != null) {
            constraintsDropdown.gameObject.SetActive(true);
            GameObject.Find("ConstraintsLabel").GetComponent<Text>().text = "Constraints on";
            constraintsDropdown.ClearOptions();
            constraintsDropdown.AddOptions(dropOptions);
            constraintsDropdown.onValueChanged.AddListener(delegate {
                SelectConstraints(dropOptions, componentName);
            });
        } else {
            constraintsDropdown.gameObject.SetActive(false);
            GameObject.Find("ConstraintsLabel").GetComponent<Text>().text = "";
        }
    }

    public void SelectConstraints(List<string> _dropOptions, string _componentName) {
        dropdownSelectedValue = constraintsDropdown.value;
        // Text frequencyConstraintsLabel = Util.getChildObject("SliderValuePanel", "FrequencyConstraintsLabel").GetComponent<Text>();
        // Text frequencyConstraintsValue = Util.getChildObject("SliderValuePanel", "FrequencyConstraintsValue").GetComponent<Text>();
        // if(dropdownSelectedValue == 0) {
        //     Debug.Log("remove constraints for " + _componentName);
        //     constraintsHandle.removeConstrain(_componentName);
        //     frequencyConstraintsLabel.text = "";
        //     //Util.getChildObject("SliderValuePanel", "FrequencyConstraintsValue").GetComponent<Text>().text = "";
        //     frequencyConstraintsValue.text = "";
        // } else if(_dropOptions[dropdownSelectedValue].Contains("resistor")) {
        //     constraintsHandle.addConstrain(_componentName, _dropOptions[dropdownSelectedValue]);
        //     frequencyConstraintsLabel.text = "";
        //     frequencyConstraintsValue.text = "";
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
        // if(constraintsHandle.containsConstrainSource(component) || constraintsHandle.containsConstrainTarget(component)) {
        //     Debug.Log("YJ getConstraintsSelected() dropdownSelectedValue = " + dropdownSelectedValue);
        //     return dropdownSelectedValue;
        // }
        // else return 0;
        return dropdownSelectedValue;
    }

    public void ValueChangeCheck(int _unitPerStep, int _offset)
    {
        int value = _offset + (int)valueSlider.value * _unitPerStep;
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)value, "resistor");
    }

    public void setMinMax(float min, float max)
    {
        valueSlider.minValue = min;
        valueSlider.maxValue = max;
    }
    
    public float getSliderValue()
    {
        return valueSlider.value;
    }

    public void setSliderValue(float _offset, float _index, int _unitPerStep, string _unit)
    {
        unitPerStep = _unitPerStep;
        offset = (int) _offset;
        valueSlider.value = _index;
        int value = (int)_offset + (_unitPerStep*(int)_index);
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = Util.changeUnit((float)value, "resistor"); //value.ToString() + _unit;

        //Debug.Log("setSliderValue()");
        //Debug.Log("3 result value = " + value);
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