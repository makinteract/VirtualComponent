using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

public class VoltmeterPanel : MonoBehaviour
{
    public Button refreshButton;
    public Button editButton;
    public GameObject modalPanelObject;

    private static VoltmeterPanel modalPanel;
    
    public static VoltmeterPanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (VoltmeterPanel)) as VoltmeterPanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Update() {
    }

    public void Choice (UnityAction refreshEvent, UnityAction editConnectionEvent) {
        modalPanelObject.SetActive (true);
        
        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener (refreshEvent);
        refreshButton.gameObject.SetActive (true);

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