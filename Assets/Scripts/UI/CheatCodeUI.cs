using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeUI : MonoBehaviour
{
    [SerializeField] private InputField _input;
    [SerializeField] private Button _submitBtn;
    [SerializeField] private Button _tutBtn;
    [SerializeField] private Scrollbar _verticleScrollBar;
    [SerializeField] private Text _output;
    [SerializeField] private GameObject _tutObject;

    private void OnEnable() 
    {
        AddAllButtonEvent();
    }
    private void OnDisable() 
    {
        RemoveAllButtonEvent();
    }

    private void AddAllButtonEvent()
    {
        _submitBtn.onClick.AddListener(OnSubmitButtonClicked);
        _tutBtn.onClick.AddListener(OnTutButtonClicked);
    }
    private void RemoveAllButtonEvent()
    {
        _submitBtn.onClick.RemoveListener(OnSubmitButtonClicked);
        _tutBtn.onClick.RemoveListener(OnTutButtonClicked);
    }
    private void OnSubmitButtonClicked()
    {
        if(string.IsNullOrEmpty(_input.text)) return;
        string cheatCode = _input.text;
        _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + cheatCode;
        _input.text = "";
        bool checkFlag;
        AnalyzedCheatCode analyzedCheatCode = CheatCode.AnalysisCheatCode(cheatCode, out checkFlag);
        if (!checkFlag)
        {
            return;
        }
        if(analyzedCheatCode.methodType==AnalyzedCheatCode.EMethodType.GETVALUE)
        {
            Debug.Log("GetValue");
            Type fieldType;
            List<object> values = CheatCode.GetFieldValueByCheatCode(analyzedCheatCode, out fieldType);
            if(values == null) return;
            Debug.Log(values.ToJsonFormat());
            for (int i = 0; i < values.Count; i++)
            {
                _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + $"Index {i} - {fieldType}: {values[i].ToJsonFormat()}";
            }
            _verticleScrollBar.value = 0;
            return;
        }

        if (analyzedCheatCode.methodType == AnalyzedCheatCode.EMethodType.SETVALUE)
        {
            Debug.Log("SetValue");
            if (CheatCode.SetFieldValueByCheatCode(analyzedCheatCode))
            {
                _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + $"Set All Value DONE";
            }
            else
            {
                _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + $"Set All Value FAILE";
            }
        }
    }

    private void OnTutButtonClicked()
    {
        _tutObject.SetActive(true);
    }
}
