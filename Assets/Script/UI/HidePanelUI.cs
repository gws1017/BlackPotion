using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePanelUI : MonoBehaviour
{

    private BuffManager _bm;

    private BuffManager BM
    {
        get
        {
            if (_bm == null)
                _bm = GameManager.GM.BM;
            return _bm;
        }
    }
    private void Start()
    {
        
    }

    private bool IsValidManager()
    {
        if (GameManager.GM == null)
            return false;
        if (GameManager.GM.BM == null)
            return false;
        if (GameManager.GM.CurrentStage != GameStage.Brewing)
            return false;

        return true;
    }

    private void OnEnable()
    {
        if (IsValidManager())
            BM.DisableBuffListButton();
    }
    private void OnDisable()
    {
        if (IsValidManager())
            BM.EnableBuffListButton();
    }

}
