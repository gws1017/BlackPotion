using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{
    //public DaylightTime _playTime;
    //현재 일차
    public int _currentCraftDay = 0;
    //현재 소유하고 있는 골드량
    public int _currentMoney = 0;

    //누적 데이터
    public int _questSuccCnt = 0;
    public int _maxCraftDay = 0;

    void Start()
    {
        
    }

    public void IncrementCraftDay()
    {
        _currentCraftDay++;
        _maxCraftDay = Mathf.Max(_maxCraftDay, _currentMoney);
    }

}
