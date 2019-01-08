using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ConstraintsHandler : MonoBehaviour {
    public BoardDataHandler boardDataHandler;
    public ResistorConstraints resistorConstriants;
    public CapacitorConstraints capacitorConstriants;
	public UnityAction<JObject> handleConstraintsAction;
	public DataReceivedEvent handleConstraintsEvent;
	Dictionary<string, string> constrainsDB = new Dictionary<string, string>();

    void Start() {
        gameObject.SetActive(true);
		handleConstraintsAction = new UnityAction<JObject>(handleConstraints);
        handleConstraintsEvent = new DataReceivedEvent();
        handleConstraintsEvent.AddListener(handleConstraintsAction);
    }

    public void handleConstraints(JObject data) {
        string component = (string)data.GetValue("name");
        float value = (float)data.GetValue("value");

        Debug.Log("handleConstraints( " + component + ", " + value + " )");

        if(containsConstrainSource(component)) {
            if(component.Contains("resistor")) {
                //Debug.Log("resistor constraints");
                resistorConstriants.handler(value, component, getConstrainTarget(component));
            }
            if(component.Contains("capacitor")) {
                //Debug.Log("capacitor constraints");
                capacitorConstriants.handler(value, component, getConstrainTarget(component));
            }
        } else if(containsConstrainTarget(component)) {
            if(component.Contains("resistor")) {
                //Debug.Log("resistor constraints");
                resistorConstriants.handler(value, component, getConstrainSource(component));
            }
            if(component.Contains("capacitor")) {
                capacitorConstriants.handler(value, component, getConstrainSource(component));
                //Debug.Log("capacitor constraints");
            }
        }
    }

    public void addConstrain(string source, string target) {
        if(!containsConstrainSource(source) && !containsConstrainSource(target) && !containsConstrainTarget(source) && !containsConstrainTarget(target))
            constrainsDB.Add(source, target);
        else {
            removeConstrain(source);
            removeConstrain(target);
            constrainsDB.Add(source, target);
        }
        foreach (KeyValuePair<string, string> kvp in constrainsDB)
            Debug.Log("constrainsDB < " + kvp.Key + ", " + kvp.Value + " >");
    }

    public void clearConstraintsDB() {
        constrainsDB.Clear();
    }

    public void removeConstrain(string source) {
        constrainsDB.Remove(source);
    }

    public bool containsConstrainSource(string source) {
        return constrainsDB.ContainsKey(source);
    }

    public bool containsConstrainTarget(string target) {
        return constrainsDB.ContainsValue(target);
    }

    public string getConstrainSource(string target) {
        string result = Util.KeyByValue(constrainsDB, target);
        Debug.Log("getConstrainSource( " + target + " ) = " + result);
        return result;
    }

    public string getConstrainTarget(string source) {
        string result = "";
        if (constrainsDB.ContainsKey(source)){
            result = constrainsDB[source];
        } else {
            result = "NA";
        }
        return result;
    }

    public string getBoardId() {
        return boardDataHandler.getBoardId();
    }
}
