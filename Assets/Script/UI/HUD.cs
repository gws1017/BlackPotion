using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임 내내 항상 표시되는 UI를 위한 클래스
public class HUD : MonoBehaviour
{
    [SerializeField] private Text _currentGoldUIText;

    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _backMenuButton;

    [SerializeField] private GameObject _pauseMenuObject;
    [SerializeField] private GameObject _settingMenuObject;

    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.GM;
        _menuButton.onClick.AddListener(TogglePauseMenu);
        _settingButton.onClick.AddListener(ShowSettingMenu);
        _backMenuButton.onClick.AddListener(HideSettingMenu);
    }
    private void LateUpdate()
    {
        _currentGoldUIText.text = _gm.PlayInformation.CurrentGold.ToString();
    }
    public void ShowSettingMenu()
    {
        _settingMenuObject.SetActive(true);
    }
    public void HideSettingMenu()
    {
        _settingMenuObject.SetActive(false);
    }
    public void TogglePauseMenu()
    {
        _pauseMenuObject.SetActive(!_pauseMenuObject.activeSelf);
        _settingMenuObject.SetActive(false);
    }
}
