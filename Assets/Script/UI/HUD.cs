using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임 내내 항상 표시되는 UI를 위한 클래스
public class HUD : MonoBehaviour
{
    public const string GAME_PLAY_SCENE = "GamePlayScene";
    public const string MAIN_MENU_SCENE = "MainMenuScene";

    [SerializeField] protected Button _settingButton;
    [SerializeField] private Button _backMenuButton;

    [SerializeField] protected GameObject _settingMenuObject;

    protected GameManager _gm;

    virtual protected void Start()
    {
        _gm = GameManager.GM;
        _settingButton.onClick.AddListener(ShowSettingMenu);
        _backMenuButton.onClick.AddListener(HideSettingMenu);
    }

    public void ShowSettingMenu()
    {
        _settingMenuObject.SetActive(true);
    }
    public void HideSettingMenu()
    {
        _settingMenuObject.SetActive(false);
    }
    
}
