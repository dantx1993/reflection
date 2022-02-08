using System;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionStorage
{
    private static Dictionary<Type, List<object>> _storage = new Dictionary<Type, List<object>>();
    public static Dictionary<Type, List<object>> Storage => _storage;

    public static void Bind(object myObject)
    {
        if (!Storage.ContainsKey(myObject.GetType()))
        {
            Storage.Add(myObject.GetType(), new List<object>());
        }
        if(Storage[myObject.GetType()].Contains(myObject)) return;
        Storage[myObject.GetType()].Add(myObject);
    }
    public static void Unbind(object myObject)
    {
        if (!Storage.ContainsKey(myObject.GetType())) return;
        Storage[myObject.GetType()].Remove(myObject);
    }

    public static List<object> GetObjects(Type type)
    {
        if (!Storage.ContainsKey(type)) return null;
        return Storage[type];
    }
}
