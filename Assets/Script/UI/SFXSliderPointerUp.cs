using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXSliderPointerUp : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
       SoundManager._Instance.PlaySFX2D(SFXType.Stamp);
    }
}
