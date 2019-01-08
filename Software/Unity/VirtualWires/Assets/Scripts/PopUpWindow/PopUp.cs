// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Events;
// using System.Collections;

// //  This script will be updated in Part 2 of this 2 part series.
// public class PopUp : MonoBehaviour
// {
//     private ModalPanel modalPanel;
//     //private DisplayManager displayManager;

//     private UnityAction mySaveAction;
//     //private UnityAction myNoAction;
//     private UnityAction myCancelAction;

//     void Awake () {
//         modalPanel = ModalPanel.Instance ();
//         //displayManager = DisplayManager.Instance ();

//         mySaveAction = new UnityAction (SaveFunction);
//         //myNoAction = new UnityAction (TestNoFunction);
//         myCancelAction = new UnityAction (CancelFunction);
//     }

//     //  Send to the Modal Panel to set up the Buttons and Functions to call
//     public void pop () {
//         modalPanel.Choice (SaveFunction, CancelFunction);
// //      modalPanel.Choice ("Would you like a poke in the eye?\nHow about with a sharp stick?", myYesAction, myNoAction, myCancelAction);
//     }

//     //  These are wrapped into UnityActions
//     void SaveFunction () {
//         //displayManager.DisplayMessage ("Heck yeah! Yup!");
//     }

//     void CancelFunction () {
//         //displayManager.DisplayMessage ("I give up!");
//     }
// }