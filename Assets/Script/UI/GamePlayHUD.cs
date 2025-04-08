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
    }
    private void LateUpdate()
    {
        _currentGoldUIText.text = _gm.PlayInformation.CurrentGold.ToString();
    }

    public void TogglePauseMenu()
    {
        _pauseMenuObject.SetActive(!_pauseMenuObject.activeSelf);
        _settingMenuObject.SetActive(false);
        _gm.Board._CanActiveSelectEffect = !_pauseMenuObject.activeSelf;
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
