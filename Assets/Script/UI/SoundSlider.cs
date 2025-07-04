using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    private SaveManager _saveManager;


    void Start()
    {
        if(_saveManager == null)
        {
            _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        }
        float sfxValue = _saveManager.SFXVolume;
        float bgmValue = _saveManager.BGMVolume;

        _sfxSlider.value = sfxValue;
        _bgmSlider.value = bgmValue;

        SoundManager._Instance.SFXVolume = sfxValue;
        SoundManager._Instance.BGMVolume = bgmValue;
    }


    public void SFXVolumeSlide()
    {
        SoundManager._Instance.SFXVolume = _sfxSlider.value;
    }

    public void BGMVolumeSlide()
    {
        SoundManager._Instance.BGMVolume = _bgmSlider.value;
    }


}
