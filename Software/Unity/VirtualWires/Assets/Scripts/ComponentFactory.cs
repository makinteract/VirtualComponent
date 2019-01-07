//using Boomlagoon.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class ComponentFactory
{
    public static ComponentBase Create(string component, JObject spec)
    {
        if((string)spec.GetValue("type") == "fixed") {
            return new FixedComponentCreator((string)spec.GetValue("type"), (int)spec.GetValue("id"), (string)spec.GetValue("componentType"), (int)spec.GetValue("pins"), (int)spec.GetValue("value"), (JArray)spec.GetValue("connections"));
        } else {
            //Debug.Log(spec);
            switch(component) {
                case "resistor": {
                    if(spec != null) {
                        return new ResistorCreator((string)spec.GetValue("type"), (int)spec.GetValue("id"), (string)spec.GetValue("componentType"), (int)spec.GetValue("pins"), (int)spec.GetValue("value"), (JArray)spec.GetValue("connections"), (int)spec.GetValue("stepResolution"), (int)spec.GetValue("totSteps"), (int)spec.GetValue("minValue"), (int)spec.GetValue("maxValue"));
                    } else {
                        Debug.Log("[ComponentFactory] Resistor - JObject spec is null");
                        return null;
                    }
                }
                case "capacitor": {
                    if(spec != null) {
                        JArray selectableArray = (JArray)spec.GetValue("optionValues");
                        int[] optionValues = selectableArray.ToObject<int[]>();

                        if(optionValues.Length < 5) {
                            List<int> tempList = new List<int>();
                            for(int i=0; i<optionValues.Length; i++){
                                tempList.Add(optionValues[i]);
                            }
                            for(int i=optionValues.Length; i<5; i++) {
                                tempList.Add(0);
                            }
                                
                            optionValues = tempList.ToArray();
                            // for(int i=0; i<selectables.Length; i++)
                            //     Debug.Log("selectables[" + i + "] = " + selectables[i]);
                        }
                        return new CapacitorCreator((string)spec.GetValue("type"), (int)spec.GetValue("id"), (string)spec.GetValue("componentType"), (int)spec.GetValue("pins"), (int)spec.GetValue("value"), (JArray)spec.GetValue("connections"), optionValues);
                    } else {
                        Debug.Log("[ComponentFactory] Capacitor - JObject spec is null");
                        return null;
                    }
                }
                case "inductor": {
                    if(spec != null) {
                        JArray selectableArray = (JArray)spec.GetValue("optionValues");
                        int[] optionValues = selectableArray.ToObject<int[]>();

                        if(optionValues.Length < 5) {
                            List<int> tempList = new List<int>();
                            for(int i=0; i<optionValues.Length; i++){
                                tempList.Add(optionValues[i]);
                            }
                            for(int i=optionValues.Length; i<5; i++) {
                                tempList.Add(0);
                            }
                                
                            optionValues = tempList.ToArray();
                            // for(int i=0; i<selectables.Length; i++)
                            //     Debug.Log("selectables[" + i + "] = " + selectables[i]);
                        }
                        return new InductorCreator((string)spec.GetValue("type"), (int)spec.GetValue("id"), (string)spec.GetValue("componentType"), (int)spec.GetValue("pins"), (int)spec.GetValue("value"), (JArray)spec.GetValue("connections"), optionValues);
                    } else {
                        Debug.Log("[ComponentFactory] inductor - JObject spec is null");
                        return null;
                    }
                }
                default:
                return null;
            }
        }
    }

    private sealed class ResistorCreator: Resistor
    {
        public ResistorCreator(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int _stepResolution, int _totSteps, int _minValue, int _maxValue): base(_type, _id, _componentName, _numberOfPins, _value, _connection, _stepResolution, _totSteps, _minValue, _maxValue)
        {
        }
    }

    private sealed class CapacitorCreator: Capacitor
    {
        public CapacitorCreator(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues): base(_type, _id, _componentName, _numberOfPins, _value, _connection, _optionValues)
        {
        }
    }

    private sealed class InductorCreator: Inductor
    {
        public InductorCreator(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues): base(_type, _id, _componentName, _numberOfPins, _value, _connection, _optionValues)
        {
        }
    }

    private sealed class FixedComponentCreator: FixedComponent
    {
        public FixedComponentCreator(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection): base(_type, _id, _componentName, _numberOfPins, _value, _connection)
        {
        }
    }
}

public class ComponentBase
{
    public string type;
    public int id;
    public string componentName;
    public int numberOfPins;
    public int value;
    public JArray connection;
    protected ComponentBase(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection)
    {
        type = _type;
        id = _id;
        componentName = _componentName;
        numberOfPins = _numberOfPins;
        value = _value;
        connection = _connection;
    }

    protected static ComponentBase Create(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection)
    {
        return new ComponentBase(_type, _id, _componentName, _numberOfPins, _value, _connection);
    }
}

public class FixedComponent : ComponentBase
{
    public string type;
    protected FixedComponent(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection): base(_type, _id, _componentName, _numberOfPins, _value, _connection)
    {
        type = _type;
    }

    protected static FixedComponent Create(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int _stepResolution, int _totSteps, int _minValue, int _maxValue)
    {
        return new FixedComponent(_type, _id, _componentName, _numberOfPins, _value, _connection);
    }
}

public class Resistor: ComponentBase
{
    public int stepResolution;
	public int totSteps;
    public int minValue;
    public int maxValue;
    
    protected Resistor(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int _stepResolution, int _totSteps, int _minValue, int _maxValue): base(_type, _id, _componentName, _numberOfPins, _value, _connection)
    {
        stepResolution = _stepResolution;
        totSteps = _totSteps;
        minValue = _minValue;
        maxValue = _maxValue;
    }

    protected static Resistor Create(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int _stepResolution, int _totSteps, int _minValue, int _maxValue)
    {
        return new Resistor(_type, _id, _componentName, _numberOfPins, _value, _connection, _stepResolution, _totSteps, _minValue, _maxValue);
    }
}

public class Capacitor: ComponentBase
{
    public int[] optionValues;
    protected Capacitor(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues): base(_type, _id, _componentName, _numberOfPins, _value, _connection)
    {
        optionValues = _optionValues;
    }

    protected static Capacitor Create(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues)
    {
        return new Capacitor(_type, _id, _componentName, _numberOfPins, _value, _connection, _optionValues);
    }
}

public class Inductor: ComponentBase
{
    public int[] optionValues;
    protected Inductor(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues): base(_type, _id, _componentName, _numberOfPins, _value, _connection)
    {
        optionValues = _optionValues;
    }

    protected static Inductor Create(string _type, int _id, string _componentName, int _numberOfPins, int _value, JArray _connection, int[] _optionValues)
    {
        return new Inductor(_type, _id, _componentName, _numberOfPins, _value, _connection, _optionValues);
    }
}