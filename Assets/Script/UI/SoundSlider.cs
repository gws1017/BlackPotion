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

    void Start()
    {
        _bgmSlider.value = SoundManager._Instance.BGMVolume;
        _sfxSlider.value = SoundManager._Instance.SFXVolume;
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
