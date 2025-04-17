using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� ���� �׻� ǥ�õǴ� UI�� ���� Ŭ����
public class HUD : MonoBehaviour
{
    [SerializeField] protected Button _settingButton;
    [SerializeField] private Button _backMenuButton;

    [SerializeField] protected GameObject _settingMenuObject;
    //[SerializeField] protected GameObject _pauseMenuObject;

    protected GameManager _gm;

    virtual protected void Start()
    {
        _gm = GameManager.GM;
        _settingButton.onClick.AddListener(ShowSettingMenu);
        _backMenuButton.onClick.AddListener(HideMenu);
    }

    public void ShowSettingMenu()
    {
        _settingMenuObject.SetActive(true);
    }
    public void HideMenu()
    {
        _settingMenuObject.SetActive(false);
    }
    
}
