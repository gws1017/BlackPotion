using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� ���� �׻� ǥ�õǴ� UI�� ���� Ŭ����
public class HUD : MonoBehaviour
{
    [SerializeField]
    private Text _currentGoldUIText;

    private void LateUpdate()
    {
        _currentGoldUIText.text = GameManager.GM.PlayInformation.CurrentGold.ToString();
    }
}
