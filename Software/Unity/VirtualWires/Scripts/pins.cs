using UnityEngine;
using System.Collections;
using Vuforia;
using System;

public class ButtonPopup : MonoBehaviour, ITrackableEventHandler
{

    float native_width = 1920f;
    float native_height = 1080f;
    public Texture btntexture;
    public Texture LogoTexture;
    public Texture MobiliyaTexture;


    private TrackableBehaviour mTrackableBehaviour;

    private bool mShowGUIButton = false;


    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(
    TrackableBehaviour.Status previousStatus,
    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
        newStatus == TrackableBehaviour.Status.TRACKED ||
        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            mShowGUIButton = true;
        }
        else
        {
            mShowGUIButton = false;
        }
    }

    void OnGUI()
    {

        //set up scaling
        float rx = Screen.width / native_width;
        float ry = Screen.height / native_height;

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

        Rect mButtonRect = new Rect(1920 - 215, 5, 210, 110);
        GUIStyle myTextStyle = new GUIStyle(GUI.skin.textField);
        myTextStyle.fontSize = 50;
        myTextStyle.richText = true;

        GUI.DrawTexture(new Rect(5, 1080 - 115, 350, 110), LogoTexture);
        GUI.DrawTexture(new Rect(1530, 970, 350, 110), MobiliyaTexture);


        if (!btntexture) // This is the button that triggers AR and UI camera On/Off
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }

        if (mShowGUIButton)
        {

            GUI.Label(new Rect(40, 25, 350, 70), "<b> G E 9 1 0 0 C </b>", myTextStyle);

            //GUI.Box (new Rect (0,0,100,50), "Top-left");
            //GUI.Box (new Rect (1920 - 100,0,100,50), "Top-right");
            //GUI.Box (new Rect (0,1080- 50,100,50), "Bottom-left");
            //GUI.Box (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Bottom right");

            // draw the GUI button
            if (GUI.Button(mButtonRect, btntexture))
            {
                // do something on button click 
                OpenVideoActivity();
            }
        }
    }

    public void OpenVideoActivity()
    {
        var androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var jo = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
        // Accessing the class to call a static method on it
        var jc = new AndroidJavaClass("com.mobiliya.gepoc.StartVideoActivity");
        // Calling a Call method to which the current activity is passed
        jc.CallStatic("Call", jo);
    }

}