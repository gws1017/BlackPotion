using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXSliderPointerUp : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private SoundType _type;
    [SerializeField] private Slider _soundSlider;
    private SaveManager _saveManager;

    private void Start()
    {
        if (_saveManager == null)
        {
            _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
       if(_type == SoundType.SFX)
            SoundManager._Instance.PlaySFX2D(SFXType.Stamp);
        _saveManager.SaveVolume(_type,_soundSlider.value);
    }
}
