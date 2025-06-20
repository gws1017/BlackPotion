using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{
    [SerializeField] private Text _infoText;
    [SerializeField] private Button _confirmButton;
    public void InitializeUI(string str)
    {
        _infoText.text = str;
        GameManager.GM.BM.DisableBuffInventory();

        _confirmButton.onClick.AddListener(() => {
            SoundManager._Instance.PlayClickSound();
            GameManager.GM.BM.EnableBuffInventory();
            Destroy(gameObject); 
        });
    }
}
