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
        MaxScore,
        DefaultMax
    };
    [SerializeField] private Image[] _stampImages;
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _maxScoreButton;
    [SerializeField] private Animator[] _stampAnimators;

    private MainMenuButton _currentButtonType;
    private Action[] _readyActions;

    bool _isPlayStampAnim = false;

    public void DisableContinueButton()
    {
        if(_continueButton != null)
            _continueButton.interactable = false;
    }
    public void EnableContinueButton()
    {
        if (_continueButton != null)
            _continueButton.interactable = true;
    }
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
        _maxScoreButton.onClick.AddListener(() => TriggerAction(MainMenuButton.MaxScore));
        SoundManager._Instance.CurrentBGM = BGMType.Title;
        SoundManager._Instance.PlayBGM();
    }

    private void TriggerAction(MainMenuButton type)
    {
        if (_isPlayStampAnim == false)
            _isPlayStampAnim = true;
        else
            return;

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
            case MainMenuButton.MaxScore:
                _readyActions[id] = () => ShowEndingUI();
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

    public void OnAnimationPlayStamp()
    {
        SoundManager._Instance.PlaySFX2D(SFXType.Stamp);
    }
    public void OnAnimationComplete()
    {
        int index = (int)_currentButtonType;
        if (_readyActions[index] != null)
        {
            _readyActions[index].Invoke();
            _readyActions[index] = null;
        }
        _isPlayStampAnim = false;

    }

    public void StartGame()
    {
        SceneSharedData.loadSaveData = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(Constants.GAME_PLAY_SCENE);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.GM.PlayInformation.ResetInfo(false);
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
