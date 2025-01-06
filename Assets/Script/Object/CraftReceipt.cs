using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    //레퍼런스
    private PlayInfo _playInfo;
    private QuestBoard _board;
    [SerializeField]
    private Store _storeUI;

    //UI
    [SerializeField]
    private Button _nextButton;
    [SerializeField]
    private Text[] _potionNameText;
    [SerializeField]
    private Text[] _moneyText;
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private Text _targetMoneyText;

    //목표 금액
    [SerializeField]
    private int[] _targetMoney;
    //목표 달성 여부
    private bool _targetSucc;

    public bool TargetSuccess
    {
        get { return _targetSucc; }
    }

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;
        _playInfo = GameManager.GM.PlayInfomation;
        _nextButton.onClick.AddListener(ShowRecipeStore);
    }

    //정산 내용을 업데이트한다
    public void UpdateReceipt()
    {
        var questList = _board.AcceptQuestList;
        var questResult = _board._questResultDict;
        int totalGold = 0;

        //정산은 현재 보유 골드가 아니라 의뢰 성공여부에 따른 수익만 체크한다
        //상점에서 사용한 돈은 고려하지 않는다.
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].text = questList[i].PotionName + " 제조";
            int gold = 0;

            //의뢰별 결과 UI 업데이트
            if (questResult[questList[i]])
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
            _moneyText[i].text = gold.ToString() + "G";
        }

        //수락한 의뢰 수만큼 만 보여준다
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].enabled = true;
            _moneyText[i].enabled = true;
        }
        for (int i = questList.Count; i < _potionNameText.Length; i++)
        {
            _potionNameText[i].enabled = false;
            _moneyText[i].enabled = false;
        }

        if(totalGold >= _targetMoney[_playInfo.CurrentDay])
        {
            //이미지 경로를 저장하는 클래스를 따로만들까?
            //아니면 #define?
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetSucc");
            _targetSucc = true;
        }
        else
        {
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetFail");
            _targetSucc = false;
        }
        _targetMoneyText.text = totalGold.ToString() + " / " + _targetMoney[_playInfo.CurrentDay].ToString();
    }

    //레시피상점 오픈
    public void ShowRecipeStore()
    {
        _storeUI.OpenStoreUI(Store.StoreType.Recipe);
    }
}
