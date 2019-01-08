using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EditTogglePanel : MonoBehaviour {
    public Button editButton;
    public GameObject modalPanelObject;

    private static EditTogglePanel modalPanel;
    
    public static EditTogglePanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (EditTogglePanel)) as EditTogglePanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Update() {
    }

    public void Choice (UnityAction editConnectionEvent) {
        modalPanelObject.SetActive (true);

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(editConnectionEvent);
        editButton.gameObject.SetActive (true);
    }

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    public void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}