using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleHUD : HUD
{

    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _quitButton;

    override protected void Start()
    {
        base.Start();
        _gameStartButton.onClick.AddListener(StartGame);
        _continueButton.onClick.AddListener(ContinueGame);
        _quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        SceneSharedData.loadSaveData = false;
        SceneManager.LoadScene(GAME_PLAY_SCENE);
    }
    public void ContinueGame()
    {
        SceneSharedData.loadSaveData = true;
        SceneManager.LoadScene(GAME_PLAY_SCENE);
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
