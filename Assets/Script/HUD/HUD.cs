using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Text _currentGoldUIText;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void LateUpdate()
    {
        _currentGoldUIText.text = GameManager.GM._playInfo.CurrentGold.ToString();
    }
}
