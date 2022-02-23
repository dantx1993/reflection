using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CheatCode
{
    public static List<object> GetFieldValueByCheatCode(AnalyzedCheatCode cheatCode, out Type fieldTypeRef)
    {
        fieldTypeRef = null;
        int startIndex = 0;
        List<object> result = new List<object>();

        ResultObject resultComponent = new ResultObject();
        EActionType action;

        if(cheatCode.commandType == AnalyzedCheatCode.ECommandType.GO)
        {
            GameObject targetGO = GameObject.Find(cheatCode.target);
            if(targetGO == null)
            {
                Debug.LogError($"Can't find any GameObject with name: {cheatCode.target}");
                return null;
            }

            
            bool checkFlag = GetActionType(cheatCode.fields[0], out action);
            if(!checkFlag)
            {
                return null;
            }
            if(action == EActionType.NONE)
            {
                Debug.LogError("Wrong Cheat Code");
                return null;
            }

            resultComponent = new ResultObject();
            checkFlag = CheckTarget(cheatCode.fields[0], out resultComponent, new List<UnityEngine.Object>() { targetGO });
            if(!checkFlag)
            {
                return null;
            }

            startIndex = 1;

            goto StartCheatCode;
        }

        Type type = Type.GetType(cheatCode.target);
        if (type == null)
        {
            Debug.Log($"Can't Find Type ({type}) in Project");
            return result;
        }


        resultComponent.resultCsharpObject = type.GetStaticField(cheatCode.fields[0], out resultComponent.resultType);
        Debug.Log(resultComponent.resultType + ": " + resultComponent.resultCsharpObject.ToJsonFormat());
        startIndex = 1;

    StartCheatCode:

        for (int i = startIndex; i < cheatCode.fields.Count; i++)
        {
            ResultObject tempResultObject = new ResultObject(resultComponent);
            resultComponent = new ResultObject();
            bool checkFlag1 = GetActionType(cheatCode.fields[i], out action);
            bool checkFlag2;
            if (!checkFlag1)
            {
                return null;
            }
            if (action == EActionType.GETCOMPONENT || action == EActionType.GETCOMPONENTINCHILDREN)
            {
                if (tempResultObject.resultUnityObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                        targetComponents: new List<UnityEngine.Object>() { tempResultObject.resultUnityObject });
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else if (tempResultObject.resultUnityObjects != null && tempResultObject.resultUnityObjects.Count > 0)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                        targetComponents: tempResultObject.resultUnityObjects);
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else
                {
                    Debug.Log($"Can't GetComponent in CSharpObject {cheatCode.fields[i - 1]}.{cheatCode.fields[i]}");
                    return null;
                }
            }
            else
            {
                if (tempResultObject.resultUnityObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: new List<object>() { tempResultObject.resultUnityObject });
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else if (tempResultObject.resultUnityObjects != null && tempResultObject.resultUnityObjects.Count > 0)
                {
                    List<object> tempInstances = new List<object>();
                    tempResultObject.resultUnityObjects.ForEach(unityObject =>
                    {
                        tempInstances.Add(unityObject);
                    });
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                           instances: tempInstances);
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else if (tempResultObject.resultCsharpObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: new List<object>() { tempResultObject.resultCsharpObject });
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else if (tempResultObject.resultScharpObjects != null && tempResultObject.resultScharpObjects.Count > 0)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: tempResultObject.resultScharpObjects);
                    if (!checkFlag2)
                    {
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"Something is Wrong");
                    return null;
                }
            }
        }


        if (resultComponent.resultUnityObject != null)
        {
            result.Add(resultComponent.resultUnityObject);
            return null;
        }
        else if (resultComponent.resultUnityObjects != null && resultComponent.resultUnityObjects.Count > 0)
        {
            resultComponent.resultUnityObjects.ForEach(unityObject =>
            {
                result.Add(unityObject);
            });
            return result;
        }
        else if (resultComponent.resultCsharpObject != null)
        {
            fieldTypeRef = resultComponent.resultType;
            return new List<object>() { resultComponent.resultCsharpObject };
        }
        else if (resultComponent.resultScharpObjects != null && resultComponent.resultScharpObjects.Count > 0)
        {
            fieldTypeRef = resultComponent.resultType;
            return resultComponent.resultScharpObjects;
        }
        else
        {
            Debug.LogError($"Something is Wrong");
            return null;
        }
    }
    public static bool SetFieldValueByCheatCode(AnalyzedCheatCode cheatCode)
    {
        int startIndex = 0;
        List<object> result = new List<object>();

        ResultObject resultComponent = new ResultObject();
        EActionType action;

        if (cheatCode.commandType == AnalyzedCheatCode.ECommandType.GO)
        {
            GameObject targetGO = GameObject.Find(cheatCode.target);
            if (targetGO == null)
            {
                Debug.LogError($"Can't find any GameObject with name: {cheatCode.target}");
                return false;
            }


            bool checkFlag = GetActionType(cheatCode.fields[0], out action);
            if (!checkFlag)
            {
                return false;
            }
            if (action == EActionType.NONE)
            {
                Debug.LogError("Wrong Cheat Code");
                return false;
            }

            resultComponent = new ResultObject();
            checkFlag = CheckTarget(cheatCode.fields[0], out resultComponent, new List<UnityEngine.Object>() { targetGO });
            if (!checkFlag)
            {
                return false;
            }

            startIndex = 1;

            goto StartCheatCode;
        }

        Type type = Type.GetType(cheatCode.target);
        if (type == null)
        {
            Debug.Log($"Can't Find Type ({type}) in Project");
            return false;
        }
        List<object> instances = ReflectionStorage.GetObjects(type);
        if (instances == null || instances.Count <= 0)
        {
            Debug.LogError($"Can't Find Any Binding Instance of {type}");
            return false;
        }

        instances.ForEach(instance =>
        {
            resultComponent.resultScharpObjects.Add(instance);
        });
        Debug.Log(resultComponent.resultScharpObjects.Count);
        Debug.Log(resultComponent.resultScharpObjects[0].GetType());

    StartCheatCode:

        for (int i = startIndex; i < cheatCode.fields.Count - 1; i++)
        {
            ResultObject tempResultObject = new ResultObject(resultComponent);
            resultComponent = new ResultObject();
            bool checkFlag1 = GetActionType(cheatCode.fields[i], out action);
            bool checkFlag2;
            if (!checkFlag1)
            {
                return false;
            }
            if (action == EActionType.GETCOMPONENT || action == EActionType.GETCOMPONENTINCHILDREN)
            {
                if (tempResultObject.resultUnityObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                        targetComponents: new List<UnityEngine.Object>() { tempResultObject.resultUnityObject });
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else if (tempResultObject.resultUnityObjects != null && tempResultObject.resultUnityObjects.Count > 0)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                        targetComponents: tempResultObject.resultUnityObjects);
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Can't GetComponent in CSharpObject {cheatCode.fields[i - 1]}.{cheatCode.fields[i]}");
                    return false;
                }
            }
            else
            {
                if (tempResultObject.resultUnityObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: new List<object>() { tempResultObject.resultUnityObject });
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else if (tempResultObject.resultUnityObjects != null && tempResultObject.resultUnityObjects.Count > 0)
                {
                    List<object> tempInstances = new List<object>();
                    tempResultObject.resultUnityObjects.ForEach(unityObject =>
                    {
                        tempInstances.Add(unityObject);
                    });
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                           instances: tempInstances);
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else if (tempResultObject.resultCsharpObject != null)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: new List<object>() { tempResultObject.resultCsharpObject });
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else if (tempResultObject.resultScharpObjects != null && tempResultObject.resultScharpObjects.Count > 0)
                {
                    checkFlag2 = CheckTarget(cheatCode.fields[i], out resultComponent,
                            instances: tempResultObject.resultScharpObjects);
                    if (!checkFlag2)
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.LogError($"Something is Wrong");
                    return false;
                }
            }
        }

        string stringKey = "";
        string fieldName = "";
        bool checkActionType = GetActionType(cheatCode.fields[cheatCode.fields.Count - 1], out action);
        if (!checkActionType)
        {
            return false;
        }
        if (action == EActionType.GETCOMPONENT || action == EActionType.GETCOMPONENTINCHILDREN)
        {
            Debug.LogError("Wrong Cheat Code. Last Cheat Code must be a fields");
            return false;
        }
        string pattern = @"<(.*?)>";
        MatchCollection matchs = Regex.Matches(cheatCode.fields[cheatCode.fields.Count - 1], pattern, RegexOptions.Multiline);
        if (matchs.Count == 1)
        {
            stringKey = matchs[0].Groups[1].Value;
        }
        if(!string.IsNullOrEmpty(stringKey))
        {
            string getMethodPattern = @"(\w+)<[a-zA-Z0-9{}\[\]|\\;:'""""<>,\/!@#$%^&*()_?+=]+>";
            RegexOptions getMethodOptions = RegexOptions.Multiline;
            MatchCollection matchCollection = Regex.Matches(cheatCode.fields[cheatCode.fields.Count - 1], getMethodPattern, getMethodOptions);
            fieldName = matchCollection[0].Groups[1].Value;
        }

        if (resultComponent.resultUnityObject != null)
        {
            bool check;
            if (string.IsNullOrEmpty(stringKey))
            {
                check = resultComponent.resultUnityObject.SetFieldValue(cheatCode.fields[cheatCode.fields.Count - 1], cheatCode.value);
            }
            else
            {
                check = resultComponent.resultUnityObject.SetFieldValue(fieldName, cheatCode.value, stringKey);
            }
            return check;
        }
        else if (resultComponent.resultUnityObjects != null && resultComponent.resultUnityObjects.Count > 0)
        {
            bool check = true;
            if (string.IsNullOrEmpty(stringKey))
            {
                resultComponent.resultUnityObjects.ForEach(resultObject =>
                {
                    check &= resultObject.SetFieldValue(cheatCode.fields[cheatCode.fields.Count - 1], cheatCode.value);
                });
            }
            else
            {
                resultComponent.resultUnityObjects.ForEach(resultObject =>
                {
                    check &= resultObject.SetFieldValue(fieldName, cheatCode.value, stringKey);
                });
            }
            return check;
        }
        else if (resultComponent.resultCsharpObject != null)
        {
            // bool check = resultComponent.resultCsharpObject.SetValue(cheatCode.value);
            bool check;
            if(string.IsNullOrEmpty(stringKey))
            {
                check = resultComponent.resultCsharpObject.SetFieldValue(cheatCode.fields[cheatCode.fields.Count - 1], cheatCode.value);
            }
            else
            {
                check = resultComponent.resultCsharpObject.SetFieldValue(fieldName, cheatCode.value, stringKey);
            }
            return check;
        }
        else if (resultComponent.resultScharpObjects != null && resultComponent.resultScharpObjects.Count > 0)
        {
            bool check = true;
            if (string.IsNullOrEmpty(stringKey))
            {
                resultComponent.resultScharpObjects.ForEach(resultObject =>
                {
                    check &= resultObject.SetFieldValue(cheatCode.fields[cheatCode.fields.Count - 1], cheatCode.value);
                });
            }
            else
            {
                resultComponent.resultScharpObjects.ForEach(resultObject =>
                {
                    check &= resultObject.SetFieldValue(fieldName, cheatCode.value, stringKey);
                });
            }
            return check;
        }
        else
        {
            Debug.LogError($"Something is Wrong");
            return false;
        }
    }



    public static AnalyzedCheatCode AnalysisCheatCode(string cheatCode, AnalyzedCheatCode.ECommandType commandType, out bool check)
    {
        check = true;
        AnalyzedCheatCode result = new AnalyzedCheatCode();
        RegexOptions options = RegexOptions.Multiline;

        // Command:
        string analyzedPattern;
        MatchCollection analyzedMatches;
        // analyzedPattern = @"\[(.*?)\]";
        analyzedPattern = @"\G\[(.*?[^\[]*?)\]";
        analyzedMatches = Regex.Matches(cheatCode, analyzedPattern, options);
        if(analyzedMatches.Count != 1)
        {
            Debug.LogError("Wrong CheatCode");
            check = false;
            return result;
        }
        result.commandType = commandType;

        // GameObject, Component, Field:
        string listFieldString = analyzedMatches[0].Groups[1].Value;
        string fieldPattern;
        MatchCollection fieldMatches;
        fieldPattern = @"([^\.][a-zA-Z0-9{}\[\]|\\;:'""<>,\/!@#$%^&*()_?+=]+)";
        fieldMatches = Regex.Matches(listFieldString, fieldPattern, options);
        if(fieldMatches.Count < 2)
        {
            Debug.LogError("Wrong CheatCode");
            check = false;
            return result;
        }
        result.target = fieldMatches[0].Value;
        for (int i = 1; i < fieldMatches.Count; i++)
        {
            result.fields.Add(fieldMatches[i].Value);
        }

        // Method:
        string methodPattern;
        MatchCollection methodMatches;
        
        methodPattern = @"\.Get\(\)";
        
        methodMatches = Regex.Matches(cheatCode, methodPattern, options);
        if(methodMatches.Count <= 0)
        {
            if(methodMatches.Count > 1)
            {
                Debug.LogError("Wrong CheatCode");
                check = false;
                return result;
            }
            methodPattern = @"\.Set\((.*?)\)";
            methodMatches = Regex.Matches(cheatCode, methodPattern, options);
            if (methodMatches.Count != 1)
            {
                Debug.LogError("Wrong CheatCode");
                check = false;
                return result;
            }
        }
        
        result.SetMethod(methodMatches[0].Value);

        // Value:
        if(result.methodType == AnalyzedCheatCode.EMethodType.SETVALUE)
        {
            string valuePattern;
            MatchCollection valueMatches;

            valuePattern = @"\((.*?)\)";
            valueMatches = Regex.Matches(result.method, valuePattern, options);

            if (methodMatches.Count <= 0)
            {
                Debug.LogError("Wrong CheatCode");
                check = false;
                return result;
            }

            result.value = valueMatches[0].Groups[1].Value;
        }

        return result;
    }

    public static bool CheckTarget(string input, out ResultObject result, List<UnityEngine.Object> targetComponents = null, List<object> instances = null)
    {
        result = new ResultObject();
        EActionType action = EActionType.NONE;
        string pattern;
        RegexOptions options = RegexOptions.Multiline;
        MatchCollection matchs;

        bool checkFlag = GetActionType(input, out action);
        if(!checkFlag)
        {
            return false;
        }

        string value = "";
        pattern = @"<(.*?)>";
        matchs = Regex.Matches(input, pattern, options);
        if (matchs.Count != 1)
        {
            if (action == EActionType.GETCOMPONENT || action == EActionType.GETCOMPONENTINCHILDREN)
            {
                Debug.LogError("Wrong Cheat Code");
                return false;
            }
            goto GetResult;
        }
        value = matchs[0].Groups[1].Value;

    GetResult:
        if(action == EActionType.GETCOMPONENT)
        {
            if (targetComponents == null || targetComponents.Count <= 0)
            {
                Debug.LogError("Missing Target to GetComponent");
                return false;
            }
            result.resultType = Type.GetType(matchs[0].Groups[1].Value);
            if (targetComponents.Count == 1)
            {
                if (targetComponents[0] is Component)
                {
                    result.resultUnityObject = (targetComponents[0] as Component).GetComponent(result.resultType);
                }
                else if (targetComponents[0] is GameObject)
                {
                    result.resultUnityObject = (targetComponents[0] as GameObject).GetComponent(result.resultType);
                }
                else
                {
                    Debug.LogError("Wrong Type for target");
                    return false;
                }
                if (result.resultUnityObject == null)
                {
                    return false;
                }
                return true;
            }

            foreach (var unityObject in targetComponents)
            {
                if (unityObject is Component)
                {
                    result.resultUnityObjects.Add((unityObject as Component).GetComponent(result.resultType));
                }
                else if (unityObject is GameObject)
                {
                    result.resultUnityObjects.Add((unityObject as GameObject).GetComponent(result.resultType));
                }
                else
                {
                    Debug.LogError("Wrong Type for target");
                    return false;
                }
            }
            result.resultUnityObjects.RemoveAll(null);
            if (result.resultUnityObjects.Count <= 0)
            {
                return false;
            }
            return true;
        }

        if (action == EActionType.GETCOMPONENTINCHILDREN)
        {
            if (targetComponents == null || targetComponents.Count <= 0)
            {
                Debug.LogError("Missing Target to GetComponent");
                return false;
            }
            result.resultType = Type.GetType(matchs[0].Groups[1].Value);
            if(targetComponents.Count == 1)
            {
                if (targetComponents[0] is Component)
                {
                    result.resultUnityObject = (targetComponents[0] as Component).GetComponentInChildren(result.resultType);
                }
                else if (targetComponents[0] is GameObject)
                {
                    result.resultUnityObject = (targetComponents[0] as GameObject).GetComponentInChildren(result.resultType);
                }
                else
                {
                    Debug.LogError("Wrong Type for target");
                    return false;
                }
                if (result.resultUnityObject == null)
                {
                    return false;
                }
                return true;
            }

            foreach (var unityObject in targetComponents)
            {
                if (unityObject is Component)
                {
                    result.resultUnityObjects.Add((unityObject as Component).GetComponentInChildren(result.resultType));
                }
                else if (unityObject is GameObject)
                {
                    result.resultUnityObjects.Add((unityObject as GameObject).GetComponentInChildren(result.resultType));
                }
                else
                {
                    Debug.LogError("Wrong Type for target");
                    return false;
                }
            }
            result.resultUnityObjects.RemoveAll(null);
            if(result.resultUnityObjects.Count <= 0)
            {
                return false;
            }
            return true;
          
        }

        if(action == EActionType.NONE)
        {
            result.type = ResultObject.EResultType.NONCOMPONENT;
            if (instances == null || instances.Count <= 0)
            {
                Debug.LogError($"Missing Instance of");
                return false;
            }
            Type fieldType;
            object resultObject;
            if(!string.IsNullOrEmpty(value))
            {
                Type testType;
                string getMethodPattern = @"(\w+)<[a-zA-Z0-9{}\[\]|\\;:'""""<>,\/!@#$%^&*()_?+=]+>";
                RegexOptions getMethodOptions = RegexOptions.Multiline;
                MatchCollection matchCollection = Regex.Matches(input, getMethodPattern, getMethodOptions);
                object testObject = instances[0].GetFieldValue(matchCollection[0].Groups[1].Value, out testType);

                if(testObject == null)
                {
                    return false;
                }

                List<Type> types = testType.GetInterfaces().ToList();
                if (!types.Contains(typeof(ICollection)))
                {
                    Debug.LogError($"{instances[0].GetType()} isn't Collection");
                    return false;
                }
            }
            if(instances.Count == 1)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string getMethodPattern = @"(\w+)<[a-zA-Z0-9{}\[\]|\\;:'""""<>,\/!@#$%^&*()_?+=]+>";
                    RegexOptions getMethodOptions = RegexOptions.Multiline;
                    MatchCollection matchCollection = Regex.Matches(input, getMethodPattern, getMethodOptions);
                    resultObject = instances[0].GetFieldValue(matchCollection[0].Groups[1].Value, out fieldType);
                    List<Type> types = fieldType.GetInterfaces().ToList();
                    if(types.Contains(typeof(IDictionary)))
                    {
                        Type[] arguments = fieldType.GetGenericArguments();
                        Type keyType = arguments[0];
                        object dictKey = null;
                        try
                        {
                            dictKey = Convert.ChangeType(value, keyType);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            return false;
                        }
                        if(dictKey == null)
                        {
                            Debug.LogError($"Key is Null in {input}");
                            return false;
                        }
                        if(!(resultObject as IDictionary).Contains(dictKey))
                        {
                            Debug.LogError($"Not Contains {dictKey} in {input}");
                            return false;
                        }
                        result.resultCsharpObject = (resultObject as IDictionary)[dictKey];
                        result.resultType = result.resultCsharpObject.GetType();
                        return true;
                    }
                    int key;
                    try
                    {
                        key = Int32.Parse(value);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return false;
                    }
                    try
                    {
                        result.resultCsharpObject = (resultObject as IList)[(int)key];
                        result.resultType = result.resultCsharpObject.GetType();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return false;
                    }
                    return true;
                }
                resultObject = instances[0].GetFieldValue(input, out fieldType);
                result.resultCsharpObject = resultObject;
                result.resultType = fieldType;
                return true;
            }
            foreach (var instanceObject in instances)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string getMethodPattern = @"(\w+)<[a-zA-Z0-9{}\[\]|\\;:'""""<>,\/!@#$%^&*()_?+=]+>";
                    RegexOptions getMethodOptions = RegexOptions.Multiline;
                    MatchCollection matchCollection = Regex.Matches(input, getMethodPattern, getMethodOptions);
                    resultObject = instanceObject.GetFieldValue(matchCollection[0].Groups[1].Value, out fieldType);
                    List<Type> types = fieldType.GetInterfaces().ToList();
                    if (types.Contains(typeof(IDictionary)))
                    {
                        Type[] arguments = fieldType.GetGenericArguments();
                        Type keyType = arguments[0];
                        object dictKey = null;
                        try
                        {
                            dictKey = Convert.ChangeType(value, keyType);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            return false;
                        }
                        if (dictKey == null)
                        {
                            Debug.LogError($"Key is Null in {input}");
                            return false;
                        }
                        if (!(resultObject as IDictionary).Contains(dictKey))
                        {
                            Debug.LogError($"Not Contains {dictKey} in {input}");
                            return false;
                        }
                        result.resultScharpObjects.Add((resultObject as IDictionary)[dictKey]);
                        if (result.resultType == null)
                        {
                            result.resultType = result.resultScharpObjects[0].GetType();
                        }
                    }
                    else
                    {
                        int key;
                        try
                        {
                            key = Int32.Parse(value);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            return false;
                        }
                        try
                        {
                            result.resultScharpObjects.Add((resultObject as IList)[(int)key]);
                            if (result.resultType == null)
                            {
                                result.resultType = result.resultScharpObjects[0].GetType();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            return false;
                        }
                    }
                }
                else
                {
                    resultObject = instanceObject.GetFieldValue(input, out fieldType);
                    result.resultScharpObjects.Add(resultObject);
                    if (result.resultType == null)
                    {
                        result.resultType = fieldType;
                    }
                }
            }
            return true;
        }
        
        return true;
    }

    public static bool GetActionType(string field, out EActionType actionOFField)
    {
        actionOFField = EActionType.NONE;
        string pattern;
        RegexOptions options = RegexOptions.Multiline;
        MatchCollection matchs;
        pattern = @"(\bGet\b)";
        matchs = Regex.Matches(field, pattern, options);
        if (matchs.Count > 1)
        {
            Debug.LogError("Wrong CheatCode");
            return false;
        }
        else if (matchs.Count == 1)
        {
            actionOFField = EActionType.GETCOMPONENT;
        }
        else if (matchs.Count <= 0)
        {
            pattern = @"(\bGetChild\b)";
            matchs = Regex.Matches(field, pattern, options);
            if (matchs.Count > 1)
            {
                Debug.LogError("Wrong CheatCode");
                return false;
            }
            else if (matchs.Count == 1)
            {
                actionOFField = EActionType.GETCOMPONENTINCHILDREN;
            }
            else if (matchs.Count <= 0)
            {
                actionOFField = EActionType.NONE;
            }
        }
        return true;
    }
}

public enum EActionType
{
    NONE,
    GETCOMPONENT,
    GETCOMPONENTINCHILDREN
}

[System.Serializable]
public class ResultObject
{
    [Serializable]
    public enum EResultType
    {
        NONCOMPONENT,
        COMPONENT
    }
    public EResultType type;
    public object resultCsharpObject;
    public UnityEngine.Object resultUnityObject;
    public Type resultType;
    public List<object> resultScharpObjects;
    public List<UnityEngine.Object> resultUnityObjects;

    public override string ToString()
    {
        return $"{type.ToString()}\n{resultCsharpObject.ToJsonFormat()}\n{resultUnityObject.name}\n{resultType.ToString()}\n{resultScharpObjects.ToJsonFormat()}";
    }

    public ResultObject()
    {
        type = EResultType.NONCOMPONENT;
        resultCsharpObject = null;
        resultUnityObject = null;
        resultType = null;
        resultScharpObjects = new List<object>();
        resultUnityObjects = new List<UnityEngine.Object>();
    }

    public ResultObject(EResultType type, UnityEngine.Object outputUnityObject, object outputCsharpObject, Type outputType, List<object> outputCsharpResultObjects, List<UnityEngine.Object> outputUnityResultObjects)
    {
        this.type = type;
        resultCsharpObject = outputCsharpObject;
        resultUnityObject = outputUnityObject;
        resultType = outputType;
        resultScharpObjects = outputCsharpResultObjects;
        resultUnityObjects = outputUnityResultObjects;
    }

    public ResultObject(ResultObject clone)
    {
        type = clone.type;
        resultCsharpObject = clone.resultCsharpObject;
        resultUnityObject = clone.resultUnityObject;
        resultType = clone.resultType;
        resultScharpObjects = clone.resultScharpObjects;
        resultUnityObjects = clone.resultUnityObjects;
    }
}

[System.Serializable]
public class AnalyzedCheatCode
{
    public enum EMethodType
    {
        NONE,
        GETVALUE,
        SETVALUE
    }
    public enum ECommandType
    {
        NONE,
        GO,
        NONGO
    }

    public ECommandType commandType;
    public string command;
    public string target;
    public List<string> fields;
    public EMethodType methodType;
    public string method;
    public string value;

    public AnalyzedCheatCode()
    {
        commandType = ECommandType.NONE;
        command = "";
        target = "";
        fields = new List<string>();
        method = "";
        methodType = EMethodType.NONE;
        value = "";
    }
    
    public void SetMethod(string method)
    {
        this.method = method;
        string pattern = @"\.Set";
        RegexOptions options = RegexOptions.Multiline;
        MatchCollection matches = Regex.Matches(method, pattern, options);
        if (matches.Count > 0)
        {
            methodType = EMethodType.SETVALUE;
            return;
        }
        pattern = @"\.Get";
        matches = Regex.Matches(method, pattern, options);
        if (matches.Count > 0)
        {
            methodType = EMethodType.GETVALUE;
        }
    }

    public void SetCommand(string command)
    {
        this.command = command;
        if(string.Compare(this.command, "GO", false) == 0)
        {
            commandType = ECommandType.GO;
            return;
        }
        if (string.Compare(this.command, "NONGO", false) == 0)
        {
            commandType = ECommandType.NONGO;
        }
    }
}
