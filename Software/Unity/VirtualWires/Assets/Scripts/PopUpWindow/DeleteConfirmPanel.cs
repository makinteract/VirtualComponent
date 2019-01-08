using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

//  This script will be updated in Part 2 of this 2 part series.
public class DeleteConfirmPanel : MonoBehaviour
{
    public Button saveButton;
    //public Button noButton;
    public Button cancelButton;
    public GameObject modalPanelObject;
    private int value;
    
    private static DeleteConfirmPanel modalPanel;
    
    public static DeleteConfirmPanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (DeleteConfirmPanel)) as DeleteConfirmPanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Update() {
        //transform.position = 
    }
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice (UnityAction yesEvent, UnityAction cancelEvent) {
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
    }
    
    public void setTitle(string title)
    {
        Text titleText = GameObject.Find("DeleteConfirmTitle").GetComponent<Text>();
        titleText.text = title;
        //Debug.Log(titleText.text);
    }

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}