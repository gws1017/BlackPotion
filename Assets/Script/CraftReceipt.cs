using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    private QuestBoard _board;

    //UI
    [SerializeField]
    private Text[] _potionNameText;
    [SerializeField]
    private Text[] _moneyText;
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private Text _targetMoneyText;

    [SerializeField]
    private int[] _targetMoney;

    // Start is called before the first frame update
    void Start()
    {
        _canvas.worldCamera = GameManager.MainCamera;
        _board = GameManager.Board;
    }

    public void UpdateReceipt()
    {
        var questList = _board._accpetQuestList;
        var info = _board._questResultDict;
        int totalGold = 0;
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].text = questList[i].PotionName + " 제조";
            int gold = 0;
            if (info[questList[i]])
            {
                _potionNameText[i].color = Color.green;
                _moneyText[i].color = Color.green;
                gold = questList[i].QuestRewardMoney;
            }
            else
            {
                _potionNameText[i].color = Color.red;
                _moneyText[i].color = Color.red;
                //의뢰 실패시 보상금의 10퍼센트 위약금 적용
                gold -= (int)(questList[i].QuestRewardMoney * 0.1);
            }
            totalGold += gold;
            _moneyText[i].text = questList[i].QuestRewardMoney.ToString() + "G";
        }

        for (int i = questList.Count; i < _potionNameText.Length; i++)
        {
            _potionNameText[i].enabled = false;
            _moneyText[i].enabled = false;
        }

        if(totalGold >= _targetMoney[GameManager.GM._playInfo._currentCraftDay])
        {
            //이미지 경로를 저장하는 클래스를 따로만들까?
            //아니면 #define?
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetSucc");
        }
        else
        {
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetFail");
        }
        _targetMoneyText.text = totalGold.ToString() + " / " + _targetMoney[0].ToString();
        GameManager.GM._playInfo.IncrementCraftDay();
    }

}
