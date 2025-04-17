using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleHUD : HUD
{
    public enum MainMenuButton
    {
        GameStart,
        Continue,
        Setting,
        Quit,
        DefaultMax
    };
    [SerializeField] private Image[] _stampImages;
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Animator[] _stampAnimators;

    private MainMenuButton _currentButtonType;
    private Action[] _readyActions;
    override protected void Start()
    {
        base.Start();

        _settingButton.onClick.RemoveAllListeners();

        int buttonCount = (int)MainMenuButton.DefaultMax;
        _readyActions = new Action[buttonCount];

        if(PlayerPrefs.HasKey("Save") == false)
        {
            _continueButton.interactable = false;
        }

        _gameStartButton.onClick.AddListener(()=>TriggerAction(MainMenuButton.GameStart));
        _continueButton.onClick.AddListener(() => TriggerAction(MainMenuButton.Continue));
        _settingButton.onClick.AddListener(()=>TriggerAction(MainMenuButton.Setting));
        _quitButton.onClick.AddListener(() => TriggerAction(MainMenuButton.Quit));
    }

    private void TriggerAction(MainMenuButton type)
    {
        _currentButtonType = type;
        int id = (int)type;
        switch(type)
        {
            case MainMenuButton.GameStart:
                _readyActions[id] = () => StartGame();
                break;
            case MainMenuButton.Continue:
                _readyActions[id] = () => ContinueGame();
                break;
            case MainMenuButton.Setting:
                _readyActions[id] = () => ShowSettingMenu();
                break;
            case MainMenuButton.Quit:
                _readyActions[id] = () => QuitGame();
                break;
        }
        _stampAnimators[id].SetTrigger("PlayFill");
    }

    public void OnAnimationComplete()
    {
        int index = (int)_currentButtonType;
        if (_readyActions[index] != null)
        {
            _readyActions[index].Invoke();
            _readyActions[index] = null;
        }

    }

    public void StartGame()
    {
        SceneSharedData.loadSaveData = false;
        SceneManager.LoadScene(Constants.GAME_PLAY_SCENE);
    }
    public void ContinueGame()
    {
        SceneSharedData.loadSaveData = true;
        SceneManager.LoadScene(Constants.GAME_PLAY_SCENE);
    }

    public void QuitGame()
    {
        _quitButton.onClick.RemoveAllListeners();
        _settingButton.onClick.RemoveAllListeners();
        _gameStartButton.onClick.RemoveAllListeners();
        _continueButton.onClick.RemoveAllListeners();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
