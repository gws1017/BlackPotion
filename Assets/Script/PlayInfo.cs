using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{
    //public DaylightTime _playTime;
    //���� ����
    public int _currentCraftDay = 0;
    //���� �����ϰ� �ִ� ��差
    public int _currentMoney = 0;

    //���� ������
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
