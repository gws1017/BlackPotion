using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillAmountStamp : MonoBehaviour
{
    [SerializeField] private TitleHUD _TitleHUD;

    void Start()
    {
        
    }

    public void OnAnimationComplete()
    {
        _TitleHUD.OnAnimationComplete();
    }
    public void OnAnimationPlayStamp()
    {
        _TitleHUD.OnAnimationPlayStamp();
    }
}
