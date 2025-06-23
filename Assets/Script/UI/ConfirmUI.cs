using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{

    [SerializeField] private Text _infoText; 
    [SerializeField] private Text _info2Text;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private List<UIInfoEntry> _buttonPresetList = new List<UIInfoEntry>();
    [SerializeField] private Dictionary<UIInfoType, GameObject> _buttonPresetDict = new Dictionary<UIInfoType, GameObject>();

    public enum UIInfoType
    {
        Confirm,
        YesAndNo
    }

    [System.Serializable]
    public struct UIInfoEntry
    {
        public UIInfoType type;
        public GameObject buttonPresetObject;
    }
    private void Awake()
    {
        foreach(var entry in _buttonPresetList)
        {
            _buttonPresetDict.Add(entry.type, entry.buttonPresetObject);
        }
    }
    public void InitializeUI( string str, UIInfoType type = UIInfoType.Confirm, Action yesFunc = null)
    {
        _infoText.text = str;
        GameManager.GM.BM.DisableBuffInventory();

        Action DestructUI = () => {
            SoundManager._Instance.PlayClickSound();
            GameManager.GM.BM.EnableBuffInventory();
            Destroy(gameObject);
        };

        foreach (var Object in _buttonPresetDict.Values)
        {
            Object.SetActive(false);
        }

        switch(type)
        {
            case UIInfoType.Confirm:
                if(_buttonPresetDict.ContainsKey(UIInfoType.Confirm) == true)
                {
                    _buttonPresetDict[UIInfoType.Confirm].SetActive(true);

                    _confirmButton.onClick.AddListener(() => {
                        DestructUI();
                    });
                }
                
                break;
            case UIInfoType.YesAndNo:
                if (_buttonPresetDict.ContainsKey(UIInfoType.YesAndNo) == true)
                {
                    _buttonPresetDict[UIInfoType.YesAndNo].SetActive(true);
                    _info2Text.text = $"{Constants.INGRIDIENT_REFILL_GOLD}G";
                    _yesButton.onClick.AddListener(() => {
                        if (yesFunc != null)
                            yesFunc();
                        DestructUI();
                    });
                    _noButton.onClick.AddListener(() => {
                        DestructUI();
                    });
                }
                 
                break;

        }
        
    }
}
