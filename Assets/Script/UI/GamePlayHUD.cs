using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayHUD : HUD
{
    [SerializeField] private Text _currentGoldUIText;
    [SerializeField] private Text _currentDayUIText;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _restartYesButton;
    [SerializeField] private Button _restartNoButton;
    [SerializeField] private GameObject _restartUIObject;

    [Header("Debug Option")]
    [SerializeField] private Button _debugButton;
    [SerializeField] private Text _requireQualityText;
    [SerializeField] private Button _goldButton;
    [SerializeField] private GameObject _debugPannel;



    override protected void Start() 
    {
        base.Start();

        _menuButton.onClick.AddListener(TogglePauseMenu);
        _restartNoButton.onClick.AddListener(ReturnToMainMenu);
        _restartYesButton.onClick.AddListener(GameRestart);

        bool isReleaseBuild = !Application.isEditor && !Debug.isDebugBuild;
        if(isReleaseBuild == false)
        {
            _debugButton.enabled = true;
            _debugButton.onClick.AddListener(ToggleDebugPannel);
            _goldButton.onClick.AddListener(CheatGold);
        }
        else
        {
            _debugButton.enabled = false;
            _debugPannel.SetActive(false);
        }

        SoundManager._Instance.CurrentBGM = BGMType.InGame;
        SoundManager._Instance.PlayBGM();
    }
    private void LateUpdate()
    {
        _currentGoldUIText.text = _gm.PlayInformation.CurrentGold.ToString();
        _currentDayUIText.text = (_gm.PlayInformation.CurrentDay + 1).ToString();
    }

    public void TogglePauseMenu()
    {
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Click);
        _settingMenuObject.SetActive(!_settingMenuObject.activeSelf);
        //_settingMenuObject.SetActive(false);
        _gm.Board._CanActiveSelectEffect = !_settingMenuObject.activeSelf;
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
    }

    public void ToggleDebugPannel()
    {
        _debugPannel.SetActive(!_debugPannel.activeSelf);
        if (GameManager.GM.Brewer.CurrentQuest == null) return;
        _requireQualityText.text = GameManager.GM.Brewer.CurrentQuest.RequirePotionCapacity.ToString();
    }

    public void ShowRestartUI()
    {
        _restartUIObject.SetActive(true);
    }

    public void GameRestart()
    {
        GameManager.GM.CheckRecipt();
        _restartUIObject.SetActive(false);
    }

    public void CheatGold()
    {
        if(GameManager.GM.PlayInformation.CurrentGold <1000)
            GameManager.GM.PlayInformation.IncrementGold(9000);
    }
}
