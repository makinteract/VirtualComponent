using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BoardTypeDropdown : MonoBehaviour {
    public Communication communication;
	public Dropdown dropdown;
	public BoardUI boardUI;
	public Sprite DefaultPinSprite;

	void Start() {
		dropdown.onValueChanged.AddListener(delegate {
			SelectBoardType(dropdown);
		});
	}

	void Update()
	{
	}

	private void SelectBoardType(Dropdown boardType)
	{
		int boardTypeSelected = boardType.value;
		if(!communication.getDeleteWireState()) {
			switch(boardTypeSelected) {
				case 0: {
					ResetAllComponents();
					break;
				}
				case 1: {
					ResetAllComponents();
					string path = @"/storage/emulated/0/Android/data/com.kaist.virtualcomponent/files/Json/board01.json";
					boardUI.setupBoard(path);
					break;
				}
				case 2: {
					ResetAllComponents();
					string path = @"/storage/emulated/0/Android/data/com.kaist.virtualcomponent/files/Json/board02.json";
					boardUI.setupBoard(path);
					break;
				}
				case 3: {
					ResetAllComponents();
					string path = @"/storage/emulated/0/Android/data/com.kaist.virtualcomponent/files/Json/board03.json";
					boardUI.setupBoard(path);
					break;
				}
			}
		}
	}
	
	private void ResetAllComponents() {
		GameObject[] pinsTemp = GameObject.FindGameObjectsWithTag("pin");
		foreach(GameObject pinObj in pinsTemp) {
			pinObj.GetComponent<Button>().image.sprite = DefaultPinSprite;;
		}

		GameObject[] temp = GameObject.FindGameObjectsWithTag("component");
        foreach(GameObject componentObj in temp)
        {
			//GameObject pinTemp = null;
			//pinTemp = GameObject.Find(componentObj.GetComponent<Component>().left.BoardPin);
			//pinTemp.GetComponent<Button>().image.sprite = defaultBoardPinSprite;
			GameObject[] wireTemp = GameObject.FindGameObjectsWithTag("wire");
			foreach(GameObject wireObj in wireTemp)
			{
				if( wireObj.name.Contains(componentObj.name) )
				{
					LineRenderer lr = wireObj.GetComponent<LineRenderer>();
					lr.enabled = false;
					//lr.SetVertexCount(0);
					lr.positionCount = 0;
					Destroy(wireObj);
				}
			}
        }

		foreach(GameObject componentObj in temp)
        {
			Destroy(componentObj);
        }
	}
}