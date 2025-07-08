using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MaxScoreUI : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private HUD _hud;

    [Header("UI")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Text _maxScoreValueText;

    [SerializeField] private GameObject _currentScoreTabObject;
    [SerializeField] private Text _currentScoreValueText;

    public string CurrentScoreText {set => _currentScoreValueText.text = value; }
    void Start()
    {
        UpdateMaxScoreUI();
    }
    void UpdateMaxScoreUI()
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => { _hud.HideEndingUI(); });
        if (_saveManager == null)
        {
            _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        }

        _maxScoreValueText.text = $"{_saveManager.MaxGold}°ñµå";

        _currentScoreValueText.text = $"{_saveManager.RecentGold}°ñµå";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
