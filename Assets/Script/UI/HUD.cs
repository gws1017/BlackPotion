using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//게임 내내 항상 표시되는 UI를 위한 클래스
public class HUD : MonoBehaviour
{
    [SerializeField] protected Button _settingButton;
    [SerializeField] private Button _backMenuButton;

    [SerializeField] protected GameObject _settingMenuObject;
    [SerializeField] protected GameObject _maxScoreUIOjbect;
    //[SerializeField] protected GameObject _pauseMenuObject;

    protected GameManager _gm;

    public MaxScoreUI ScoreUI => _maxScoreUIOjbect.GetComponent<MaxScoreUI>();

    virtual protected void Start()
    {
        _gm = GameManager.GM;
        _backMenuButton.onClick.AddListener(HideMenu);
    }

    public void ShowSettingMenu()
    {
        _settingMenuObject.SetActive(true);
    }
    public void HideMenu()
    {
        SoundManager._Instance.PlayClickSound();
        _settingMenuObject.SetActive(false);
    }

    public void ShowEndingUI()
    {
        _maxScoreUIOjbect.SetActive(true);
    }

    public void HideEndingUI()
    {
        SoundManager._Instance.PlayClickSound();
        _maxScoreUIOjbect.SetActive(false);
        if(SceneManager.GetActiveScene().name == Constants.GAME_PLAY_SCENE)
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
    }

}
