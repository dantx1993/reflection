using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonExtension
{
    public static string ToJson(this object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }

    public static string ToJsonFormat(this object data)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        return jsonData;
    }

    public static T ToObject<T>(this string jsonData)
    {
        T data = JsonConvert.DeserializeObject<T>(jsonData);
        return data;
    }

    public static object ToObject(this string jsonData, Type type)
    {
        return JsonConvert.DeserializeObject(jsonData, type);
    }
}
