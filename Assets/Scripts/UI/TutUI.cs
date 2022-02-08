using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    private void OnEnable() 
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    private void OnDisable() 
    {
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    public void OnCloseButtonClicked()
    {
        this.gameObject.SetActive(false);
    }
}
