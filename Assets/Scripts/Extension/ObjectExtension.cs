using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtension
{
    public static object GetStaticField(this Type thisType, string fieldName, out Type typeOfObject, BindingFlags bindingAttr = BindingFlags.Static)
    {
        if (bindingAttr == BindingFlags.Static)
        {
            bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        }

        FieldInfo fieldInfo = thisType.GetField(fieldName, bindingAttr);

        if (fieldInfo == null)
        {
            Debug.LogError($"Can't Find Field Name: {fieldName}");
            typeOfObject = null;
            return null;
        }

        var value = fieldInfo.GetValue(null);
        typeOfObject = fieldInfo.FieldType;
        return value;
    }

    public static object GetFieldValue(this object thisObject, string fieldName, out Type typeOfObject, BindingFlags bindingAttr = BindingFlags.Default)
    {
        Type type = thisObject.GetType();
        if (bindingAttr == BindingFlags.Default)
        {
            bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        FieldInfo fieldInfo = type.GetField(fieldName, bindingAttr);
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
    public static bool SetFieldValue(this object thisObject, string fieldName, string valueString, string keyString = "", BindingFlags invokeAttr = BindingFlags.Default, Binder binder = null, CultureInfo culture = null)
    {
        Type type = thisObject.GetType();
        if(invokeAttr == BindingFlags.Default)
        {
            invokeAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        FieldInfo fieldInfo = type.GetField(fieldName, invokeAttr);
        if(fieldInfo == null)
        {
            Debug.LogError($"Can't Find Field Name: {fieldName}");
            return false;
        }
        TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
        object key = null;
        object value = null;
        List<Type> types = fieldInfo.FieldType.GetInterfaces().ToList();
        if(types.Contains(typeof(ICollection)))
        {
            
            if(string.IsNullOrEmpty(keyString))
            {
                try
                {
                    value = valueString.ToObject(fieldInfo.FieldType);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    return false;
                }
            }
            else
            {
                Type[] arguments = fieldInfo.FieldType.GetGenericArguments();
                if (types.Contains(typeof(IDictionary)))
                {
                    Type keyType = arguments[0];
                    typeConverter = TypeDescriptor.GetConverter(arguments[1]);
                    try
                    {
                        key = Convert.ChangeType(keyString, keyType);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return false;
                    }
                    if (!(fieldInfo.GetValue(thisObject) as IDictionary).Contains(key))
                    {
                        Debug.LogError($"Not Contains {key} in {fieldName}");
                        return false;
                    }
                    value = typeConverter.ConvertFromString(valueString);
                }
                else
                {
                    try
                    {
                        key = Int32.Parse(keyString);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return false;
                    }
                    if(arguments.Length > 0)
                    {
                        typeConverter = TypeDescriptor.GetConverter(arguments[0]);
                    }
                    else
                    {
                        Type elementType = fieldInfo.FieldType.GetElementType();
                        if(elementType != null)
                        {
                            typeConverter = TypeDescriptor.GetConverter(elementType);
                        }
                        else
                        {
                            Debug.Log("Something is Wrong");
                        }
                    }
                    value = typeConverter.ConvertFromString(valueString);
                }
            }
        }
        else
        {
            if(!string.IsNullOrEmpty(keyString))
            {
                Debug.LogError("Field isn't Collection");
                return false;
            }
            try
            {
                value = typeConverter.ConvertFromString(valueString);
            }
            catch(Exception ex1)
            {
                Debug.LogError(ex1);
                return false;
            }
        }
        try
        {
            if (!string.IsNullOrEmpty(keyString))
            {
                if(types.Contains(typeof(ICollection)))
                {
                    if (types.Contains(typeof(IDictionary)))
                    {
                        var dictionary = (IDictionary)fieldInfo.GetValue(thisObject);
                        dictionary[key] = value;
                    }
                    else
                    {
                        var array = (IList)fieldInfo.GetValue(thisObject);
                        array[(int)key] = value;
                    }
                }
            }
            else
            {
                fieldInfo.SetValue(thisObject, value, invokeAttr, binder, culture);
            }
        }
        catch(Exception ex2)
        {
            Debug.LogError(ex2);
            return false;
        }
        return true;
    }

    public static bool SetValue(this object thisObject, string valueString)
    {
        Type type = thisObject.GetType();
        object value = null;
        List<Type> interfaces = type.GetInterfaces().ToList();
        if (interfaces.Contains(typeof(ICollection)))
        {
            try
            {
                value = valueString.ToObject(type);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
        else
        {
            try
            {
                thisObject = Convert.ChangeType(valueString, type);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
        Debug.Log(thisObject.ToJsonFormat());
        return true;
    }
}
