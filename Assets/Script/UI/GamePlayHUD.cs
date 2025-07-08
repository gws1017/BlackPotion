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
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private GameObject _restartUIObject;
    [SerializeField] private GameObject _tutorialUIObject;

    [Header("Debug Option")]
    [SerializeField] private Button _debugButton;
    [SerializeField] private Text _requireQualityText;
    [SerializeField] private Button _goldButton;
    [SerializeField] private Button _successButton;
    [SerializeField] private GameObject _debugPannel;



    override protected void Start() 
    {
        base.Start();

        _menuButton.onClick.AddListener(TogglePauseMenu);
        _restartNoButton.onClick.AddListener(ReturnToMainMenu);
        _restartYesButton.onClick.AddListener(GameRestart);
        _tutorialButton.onClick.AddListener(ToggleTutorialUI);

        bool isReleaseBuild = !Application.isEditor && !Debug.isDebugBuild;
        if(isReleaseBuild == false)
        {
            _debugButton.gameObject.SetActive(true);
            _debugButton.onClick.AddListener(ToggleDebugPannel);
            _goldButton.onClick.AddListener(CheatGold);
            _successButton.onClick.AddListener(CheatSuccess);
        }
        else
        {
            _debugButton.gameObject.SetActive(false);
            _debugPannel.SetActive(false);
        }

        SoundManager._Instance.CurrentBGM = BGMType.InGame;
        SoundManager._Instance.PlayBGM();
    }
    private void LateUpdate()
    {
        _currentGoldUIText.text = _gm.PlayInformation.CurrentGold.ToString();
        _currentDayUIText.text = $"{(_gm.PlayInformation.CurrentDay + 1)}ÀÏÂ÷";
    }

    public void ToggleTutorialUI()
    {
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Click);
        _tutorialUIObject.SetActive(!_tutorialUIObject.activeSelf);
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
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Click);
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
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Click);
        GameManager.GM.PlayInformation.ResetInfo();
        GameManager.GM.ShowQuestBoard();
        _restartUIObject.SetActive(false);
    }

    public void CheatGold()
    {
        if(GameManager.GM.PlayInformation.CurrentGold <1000)
            GameManager.GM.PlayInformation.IncrementGold(9000);
    }
    public void CheatSuccess()
    {
        _gm.Receipt.SetTargetSuccess(true);
    }
}
