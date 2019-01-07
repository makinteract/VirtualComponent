using UnityEngine;

public class VWJson : MonoBehaviour
{
    public string source;
    public string target;
    
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}