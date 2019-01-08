using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CameraFocusController : MonoBehaviour {

    // Use this for initialization
	void Start () {
        VuforiaARController vuforia = VuforiaARController.Instance;

        if (vuforia != null)
        {
            vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
            vuforia.RegisterOnPauseCallback(OnPaused);
        }
    }

    private void OnVuforiaStarted()
    {
        SetAutoFocus();
        //SetMacroFocus();
    }

    private void OnPaused(bool paused)
    {
        if (!paused) // resumed
        {
            // Set again autofocus mode when app is resumed
            //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            //SetMacroFocus();
            SetAutoFocus();
        }
    }

    private void SetAutoFocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void SetMacroFocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_MACRO);
        //Debug.Log("macro");
    }
}