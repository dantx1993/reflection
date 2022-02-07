using System;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtension
{
    public static object GetFieldValue(this object thisObject, string fieldName, out Type typeOfObject, BindingFlags bindingAttr = BindingFlags.Default)
    {
        Type type = thisObject.GetType();
        if (bindingAttr == BindingFlags.Default)
        {
            bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        FieldInfo fieldInfo = type.GetField("coin", bindingAttr);
        if (fieldInfo == null)
        {
            Debug.LogError($"Can't Find Field Name: {fieldName}");
            typeOfObject = null;
            return null;
        }
        var value = fieldInfo.GetValue(thisObject);
        typeOfObject = fieldInfo.FieldType;
        return value;
    }
    public static bool SetFieldValue(this object thisObject, string fieldName, string valueString, BindingFlags invokeAttr = BindingFlags.Default, Binder binder = null, CultureInfo culture = null)
    {
        Type type = thisObject.GetType();
        if(invokeAttr == BindingFlags.Default)
        {
            invokeAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        FieldInfo fieldInfo = type.GetField("coin", invokeAttr);
        if(fieldInfo == null)
        {
            Debug.LogError($"Can't Find Field Name: {fieldName}");
            return false;
        }
        TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
        object value = null;
        try
        {
            value = typeConverter.ConvertFromString(valueString);
        }
        catch(Exception ex1)
        {
            Debug.LogError(ex1);
            return false;
        }
        try
        {
            fieldInfo.SetValue(thisObject, value, invokeAttr, binder, culture);
        }
        catch(Exception ex2)
        {
            Debug.LogError(ex2);
            return false;
        }
        return true;
    }
}
