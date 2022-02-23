using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeUI : MonoBehaviour
{
    [SerializeField] private Dropdown _dropdown;
    [SerializeField] private InputField _input;
    [SerializeField] private Button _submitBtn;
    [SerializeField] private Button _tutBtn;
    [SerializeField] private Scrollbar _verticleScrollBar;
    [SerializeField] private Text _output;
    [SerializeField] private GameObject _tutObject;

    private List<AnalyzedCheatCode.ECommandType> _commandsList;
    private AnalyzedCheatCode.ECommandType _currentCommand;

    private void Awake() 
    {
        _dropdown.options = new List<Dropdown.OptionData>();
        _commandsList = new List<AnalyzedCheatCode.ECommandType>();
        _commandsList.Add(AnalyzedCheatCode.ECommandType.GO);
        _commandsList.Add(AnalyzedCheatCode.ECommandType.NONGO);

        _commandsList.ForEach(command =>
        {
            _dropdown.options.Add(new Dropdown.OptionData(command.ToString()));
        });
    }

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
        _dropdown.onValueChanged.AddListener(OnDropdownItemSelected);
        _dropdown.value = 0;
        _currentCommand = _commandsList[_dropdown.value];
    }
    private void RemoveAllButtonEvent()
    {
        _submitBtn.onClick.RemoveListener(OnSubmitButtonClicked);
        _tutBtn.onClick.RemoveListener(OnTutButtonClicked);
        _dropdown.onValueChanged.RemoveListener(OnDropdownItemSelected);
    }
    private void OnSubmitButtonClicked()
    {
        if(string.IsNullOrEmpty(_input.text)) return;
        string cheatCode = _input.text;
        _output.text += (string.IsNullOrEmpty(_output.text) ? "" : "\n") + cheatCode;
        _input.text = "";
        bool checkFlag;
        AnalyzedCheatCode analyzedCheatCode = CheatCode.AnalysisCheatCode(cheatCode, _currentCommand, out checkFlag);
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

    private void OnDropdownItemSelected(int index)
    {
        _currentCommand = _commandsList[index];
    }
}
