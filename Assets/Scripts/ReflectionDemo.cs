using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionDemo : MonoBehaviour
{
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.P))    
        {
            // Type type = Type.GetType("Player");
            // List<object> instances = ReflectionStorage.GetObjects(type);
            // if(instances == null) return;
            // instances.ForEach(instanceObject =>
            // {
            //     Type fieldType;
            //     object value = instanceObject.GetFieldValue("coin", out fieldType);
            //     Debug.Log($"{fieldType}: {value}");
            //     instanceObject.SetFieldValue("coin", "1000");
            // });
            // Type fieldTypeRef;
            // var result = GetFieldValueByCheatCode("Player.coin", out fieldTypeRef);
            // result.ForEach(value =>
            // {
            //     Debug.Log($"{fieldTypeRef}: {value}");
            // });
            // SetFieldValueByCheatCode("Player.coin 1000");
            // bool check;
            // AnalyzedCheatCode analyzedCheatCode = CheatCode.AnalysisCheatCode("[GO][Player1.GetComponent<Player>.checks<0>].GetValue()", out check);
            // if(!check)
            // {
            //     Debug.Log("Wrong Cheat Code");
            // }
            // Debug.Log(analyzedCheatCode.ToJsonFormat());
            // Dictionary<string, int> myDict = new Dictionary<string, int>();
            // Debug.Log(myDict.GetType().GetGenericArguments().ToJsonFormat());

            List<object> instances = ReflectionStorage.GetObjects(typeof(Test));
            if (instances == null || instances.Count <= 0)
            {
                Debug.LogError($"Can't Find Any Binding Instance of {typeof(Test)}");
                return;
            }

            Debug.Log(instances.Count);

            instances.ForEach(instance =>
            {
                Type type;
                object value = instance.GetFieldValue("myList", out type);
                Debug.Log(value.ToJsonFormat());
            });

            Test test = new Test();
            Type type1;
            Debug.Log(test.GetFieldValue("test", out type1));
        }
    }

    private List<object> GetFieldValueByCheatCode(string cheatCode, out Type fieldTypeRef)
    {
        fieldTypeRef = null;
        List<object> result = new List<object>();
        string[] types = cheatCode.Split(new char[] { '.' }, 2);
        if(types.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return result;
        }
        Type type = Type.GetType(types[0]);
        if(type == null)
        {
            Debug.Log($"Can't Find Type ({types[0]}) in Project");
            return result;
        }
        List<object> instances = ReflectionStorage.GetObjects(type);
        if (instances == null || instances.Count <= 0)
        {
            Debug.LogError($"Can't Find Any Binding Instance of {type}");
            return result;
        }
        foreach (var instanceObject in instances)
        {
            Type fieldType;
            object value = instanceObject.GetFieldValue(types[1], out fieldType);
            result.Add(value);
            if (fieldTypeRef == null)
            {
                fieldTypeRef = fieldType;
            }
        }
        return result;
    }
    private void SetFieldValueByCheatCode(string cheatCode)
    {
        List<object> result = new List<object>();
        string[] types = cheatCode.Split(new char[] { '.' }, 2);
        if (types.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return;
        }
        string[] fieldAndValue = types[1].Split(new char[] { ' ' }, 2);
        if (fieldAndValue.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return;
        }
        Type type = Type.GetType(types[0]);
        if (type == null)
        {
            Debug.Log($"Can't Find Type ({types[0]}) in Project");
            return;
        }
        List<object> instances = ReflectionStorage.GetObjects(type);
        if (instances == null || instances.Count <= 0)
        {
            Debug.LogError($"Can't Find Any Binding Instance of {type}");
            return;
        }
        foreach (var instanceObject in instances)
        {
            instanceObject.SetFieldValue(fieldAndValue[0], fieldAndValue[1]);
        }
    }
}
