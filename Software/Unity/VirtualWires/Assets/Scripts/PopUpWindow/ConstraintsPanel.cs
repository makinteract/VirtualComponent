using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConstraintsPanel : MonoBehaviour {
    public Button editButton;
    public Dropdown sourceDropdown;
    public Dropdown targetDropdown;
    public GameObject modalPanelObject;

    private static ConstraintsPanel modalPanel;
    
    public static ConstraintsPanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (ConstraintsPanel)) as ConstraintsPanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    void Update() {
    }

    public void Choice (int sourceCount, int targetCount) {
        modalPanelObject.SetActive (true);

        sourceDropdown.onValueChanged.AddListener(delegate {
			SelectSource(sourceDropdown, sourceCount);
		});
        targetDropdown.onValueChanged.AddListener(delegate {
			//SelectTarget(targetDropdown, targetCount);
		});
    }

    private void SelectSource(Dropdown source, int count)
	{
		int sourceSelected = source.value;
		//if(!communication.getDeleteWireState()) {
			switch(sourceSelected) {
				case 0: {
                    //disconnect constrains
					break;
				}
				case 1: {
					break;
				}
				case 2: {
					break;
				}
				case 3: {
					break;
				}
                case 4: {
					break;
				}
                case 5: {
					break;
				}
                case 6: {
					break;
				}
                case 7: {
					break;
				}
                case 8: {
					break;
				}
			}
		//}
	}

    public void setPosition(Vector3 pos)
    {
        modalPanelObject.transform.position = pos;
    }

    public void ClosePanel () {
        modalPanelObject.SetActive (false);
    }
}