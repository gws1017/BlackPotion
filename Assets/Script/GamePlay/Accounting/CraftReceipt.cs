using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [Header("Component")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Store _storeUI;
    private PlayInfo _playInfo;
    private QuestBoard _board;

    //UI
    [Header("UI")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Text[] _potionNameText;
    [SerializeField] private Text[] _moneyText;
    [SerializeField] private Image _resultImage;
    [SerializeField] private Text _targetMoneyText;

    [Header("Setting Variable")]
    [SerializeField] private int[] _targetMoney;
    private bool _isTargetAchieved;
    private Sprite _targetSuccessSprite;
    private Sprite _targetFailSprite;
    public bool TargetSuccess => _isTargetAchieved;

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;
        _playInfo = GameManager.GM.PlayInformation;

        _targetSuccessSprite = Resources.Load<Sprite>(PlayInfo.TARGET_SUCCESS_IMAGE);
        _targetFailSprite = Resources.Load<Sprite>(PlayInfo.TARGET_FAIL_IMAGE);

        _nextButton.onClick.AddListener(ShowRecipeStore);
    }

    public void UpdateReceipt()
    {
        List<Quest> questList = _board.AcceptQuestList;
        Dictionary<Quest, bool> questResult = _board._questResultDict;
        int totalGold = 0;
        int uiCount = Mathf.Min(questList.Count, _potionNameText.Length);

        for (int i = 0; i < _potionNameText.Length; i++)
        {
                int gold = 0;
            if(i <uiCount)
            {
                _potionNameText[i].text = $"{questList[i].PotionName} Á¦Á¶";
                Quest quest = questList[i];

                if (questResult.ContainsKey(quest) && questResult[quest])
                {
                    _potionNameText[i].color = Color.green;
                    _moneyText[i].color = Color.green;
                    gold = questList[i].QuestRewardMoney;
                }
                else
                {
                    _potionNameText[i].color = Color.red;
                    _moneyText[i].color = Color.red;
                    gold -= (int)(questList[i].QuestRewardMoney * PlayInfo.QUEST_PENALTY_RATIO);
                }
                _potionNameText[i].enabled = true;
                _moneyText[i].enabled = true;
            }
            else
            {
                _potionNameText[i].enabled = false;
                _moneyText[i].enabled = false;
            }
            totalGold += gold;
            _moneyText[i].text = $"{gold} G";

        }

        int currentTarget = _targetMoney[_playInfo.CurrentDay];
        if (totalGold >= currentTarget)
        {
            _resultImage.sprite = _targetSuccessSprite;
            _isTargetAchieved = true;
        }
        else
        {
            _resultImage.sprite = _targetFailSprite;
            _isTargetAchieved = false;
        }
        _targetMoneyText.text = $"{totalGold} / {_targetMoney[_playInfo.CurrentDay]}";
    }

    public void ShowRecipeStore()
    {
        _storeUI.OpenStoreUI(Store.StoreType.Recipe);
    }
}
