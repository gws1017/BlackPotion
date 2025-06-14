using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayHUD : HUD
{
    [SerializeField] private Text _currentGoldUIText;
    [SerializeField] private Button _menuButton;


    override protected void Start() 
    {
        base.Start();
        _menuButton.onClick.AddListener(TogglePauseMenu);
        SoundManager._Instance.CurrentBGM = BGMType.InGame;
        SoundManager._Instance.PlayBGM();
    }
    private void LateUpdate()
    {
        _currentGoldUIText.text = _gm.PlayInformation.CurrentGold.ToString();
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
}
