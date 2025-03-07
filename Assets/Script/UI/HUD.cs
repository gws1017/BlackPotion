using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임 내내 항상 표시되는 UI를 위한 클래스
public class HUD : MonoBehaviour
{
    [SerializeField]
    private Text _currentGoldUIText;

    private void LateUpdate()
    {
        _currentGoldUIText.text = GameManager.GM.PlayInformation.CurrentGold.ToString();
    }
}
