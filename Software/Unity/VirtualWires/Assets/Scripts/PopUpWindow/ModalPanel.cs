using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{
    public Slider valueSlider;
    public Button saveButton;
    //public Button noButton;
    public Button cancelButton;
    public GameObject modalPanelObject;
    //private float value;
    
    private static ModalPanel modalPanel;

    private string unit;
    //private float minValue;
    //private float maxValue;
    
    public static ModalPanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Update() {
        //transform.position = 
        //setSliderValue(getSliderValue(), unit);
    }
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction saveEvent, UnityAction cancelEvent) {
        modalPanelObject.SetActive (true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener (saveEvent);
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

        valueSlider.gameObject.SetActive(true);
        valueSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(unit);});
    }
    public void ValueChangeCheck(string unit)
    {
        //Debug.Log("modalpanel.cs -" + valueSlider.value);
        //Debug.Log("modalpanel.cs -" + unit);
        //valueSlider.minValue = min;
        //valueSlider.maxValue = max;
        int value = (int)valueSlider.value;
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = value.ToString() + unit;
    }

    public void setUnit(string type)
    {
        unit = type;
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

    public void setSliderValue(float val, string unit)
    {
        valueSlider.value = val;
        int value = (int)valueSlider.value;
        Text valueText = GameObject.Find("Value").GetComponent<Text>();
        valueText.text = value.ToString() + unit;
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
        //float distance = 1200;//GameObject.Find(comm.getSourcePin()).transform.position.z;
        float distance = Camera.main.nearClipPlane;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}