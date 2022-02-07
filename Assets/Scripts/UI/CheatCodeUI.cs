using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeUI : MonoBehaviour
{
    [SerializeField] private InputField _input;
    [SerializeField] private Button _submitBtn;
    [SerializeField] private Scrollbar _verticleScrollBar;
    [SerializeField] private Text _output;

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
    }
    private void RemoveAllButtonEvent()
    {
        _submitBtn.onClick.RemoveListener(OnSubmitButtonClicked);
    }
    private void OnSubmitButtonClicked()
    {
        if(string.IsNullOrEmpty(_input.text)) return;
        string cheatCode = _input.text;
        _input.text = "";
        string[] splitCheatCodes = cheatCode.Split(new char[] { '.', ' ' }, 3);
        if(splitCheatCodes.Length <= 2)
        {
            Debug.Log("GetValue");
            Type fieldType;
            List<object> values = CheatCode.GetFieldValueByCheatCode(cheatCode, out fieldType);
            values.ForEach(value =>
            {
                _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + $"Index {values.IndexOf(value)} - {fieldType}: {value}";
            });
            _verticleScrollBar.value = 0;
            return;
        }
        if (splitCheatCodes.Length == 3)
        {
            Debug.Log("SetValue");
            if (CheatCode.SetFieldValueByCheatCode(cheatCode))
            {
                _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + $"Set All Value of {splitCheatCodes[1]} to {splitCheatCodes[2]} for All Instance of {splitCheatCodes[0]}";
            }
            _verticleScrollBar.value = 0;
        }
    }
}
