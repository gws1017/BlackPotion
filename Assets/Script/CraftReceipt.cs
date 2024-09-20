using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    //UI
    [SerializeField]
    private Text[] _potionNameText;
    [SerializeField]
    private Text[] _moneyText;
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private Text _targetMoneyText;

    // Start is called before the first frame update
    void Start()
    {
        _canvas.worldCamera = GameManager.MainCamera;
    }


}
