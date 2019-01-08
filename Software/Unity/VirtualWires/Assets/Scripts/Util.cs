using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static string changeUnit(float value, string component) {
        string result = "";
        int calcUnit = 0;
        float temp = (float)value;
        while(temp > 999.99) {
            temp = temp/1000;
            calcUnit++;
        }
        result = temp.ToString("N1");
        switch(component){
            case "resistor": {
                switch(calcUnit) {
                    case 1: result += " K"; break;
                    case 2: result += " M"; break;
                }
                break;
            }
            case "capacitor": {
                switch(calcUnit) {
                    case 0: result += " p"; break;
                    case 1: result += " n"; break;
                    case 2: result += " u"; break;
                }
                break;
            }
            case "inductor": {
                switch(calcUnit) {
                    case 0: result += " n"; break;
                    case 1: result += " u"; break;
                }
                break;
            }
            case "DC":
            case "ADC":
            case "zenerdiode":
            case "diode": {
                switch(calcUnit) {
                    case 0: result += " mV"; break;
                    case 1: result += " V"; break;
                }
                break;
            }
            case "amplitude": {
                switch(calcUnit) {
                    case 0: result += " mVpp"; break;
                    case 1: result += " Vpp"; break;
                }
                break;
            }
            case "frequency": {
                switch(calcUnit) {
                    case 0: result += " mhz"; break;
                    case 1: result += " hz"; break;
                    case 2: result += " Khz"; break;
                    case 3: result += " Mhz"; break;
                }
                break;
            }
        }
        return result;
    }

    public static Vector3 GetCurrentMousePosition(Vector3 input, GameObject targetObject)
    {
        //float distance = 1100;
        //float distance = Camera.main.nearClipPlane;
        float distance = Camera.main.transform.position.y - targetObject.transform.position.y;
        Vector3 mousePosition = new Vector3(input.x, input.y, distance);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public static GameObject getChildObject(string ParentObjectName, string ChildObjectName)
    {
        GameObject temp = GameObject.Find(ParentObjectName);
        GameObject resultObj = null;
        
        Transform[] children = temp.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name == ChildObjectName) {
                resultObj = obj.gameObject;
            }
        }
        return resultObj;
    }

    public static GameObject getChildObject(GameObject ParentObject, string ChildObjectName)
    {
        GameObject resultObj = null;
        
        Transform[] children = ParentObject.GetComponentsInChildren<Transform>();
        foreach(Transform obj in children)     
        {
            if(obj.name == ChildObjectName) {
                resultObj = obj.gameObject;
            }
        }
        return resultObj;
    }

    public static string KeyByValue(Dictionary<string, string> dict, string val)
    {
        string key = null;
        foreach (KeyValuePair<string, string> pair in dict)
        {
            if (pair.Value == val)
            { 
                key = pair.Key; 
                break; 
            }
        }
        return key;
    }
}