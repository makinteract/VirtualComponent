using UnityEngine;

public class VWJson : MonoBehaviour
{
    public string boardPin;
    public string componentPin;
    public string awgIp;
    
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}