using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatCode
{
    public static List<object> GetFieldValueByCheatCode(string cheatCode, out Type fieldTypeRef)
    {
        fieldTypeRef = null;
        List<object> result = new List<object>();
        string[] types = cheatCode.Split(new char[] { '.' }, 2);
        if (types.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return result;
        }
        Type type = Type.GetType(types[0]);
        if (type == null)
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
    public static bool SetFieldValueByCheatCode(string cheatCode)
    {
        List<object> result = new List<object>();
        string[] types = cheatCode.Split(new char[] { '.' }, 2);
        if (types.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return false;
        }
        string[] fieldAndValue = types[1].Split(new char[] { ' ' }, 2);
        if (fieldAndValue.Length < 2)
        {
            Debug.LogError("Wrong Cheat Code");
            return false;
        }
        Type type = Type.GetType(types[0]);
        if (type == null)
        {
            Debug.Log($"Can't Find Type ({types[0]}) in Project");
            return false;
        }
        List<object> instances = ReflectionStorage.GetObjects(type);
        if (instances == null || instances.Count <= 0)
        {
            Debug.LogError($"Can't Find Any Binding Instance of {type}");
            return false;
        }
        bool resultCheck = true;
        foreach (var instanceObject in instances)
        {
            resultCheck &= instanceObject.SetFieldValue(fieldAndValue[0], fieldAndValue[1]);
        }
        return resultCheck;
    }
}
