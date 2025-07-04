using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
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
    [SerializeField] private GameObject HidePanelObject;

    [Header("Setting Variable")]
    [SerializeField] private int[] _targetMoney;
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private float _typingSFXSpeed = 0.5f;
    private bool _isTargetAchieved;
    private bool _isRunningTypingSFXCorutine = false;
    private Sprite _targetSuccessSprite;
    private Sprite _targetFailSprite;
    public bool TargetSuccess => _isTargetAchieved;

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;
        _playInfo = GameManager.GM.PlayInformation;

        _targetSuccessSprite = Resources.Load<Sprite>(PathHelper.TARGET_SUCCESS_IMAGE);
        _targetFailSprite = Resources.Load<Sprite>(PathHelper.TARGET_FAIL_IMAGE);

        _nextButton.onClick.AddListener(TryNextDay);
    }

    public void ActiveHidePanel()
    {
        HidePanelObject.SetActive(true);
    }
    public void DeActiveHidePanel()
    {
        HidePanelObject.SetActive(false);
    }
    public IEnumerator UpdateReceiptCorutine()
    {
        ResetUIText();

        _resultImage.enabled = false;
        _nextButton.interactable = false;

        List<Quest> questList = _board.AcceptQuestList;
        Dictionary<Quest, bool> questResult = _board._questResultDict;
        int totalGold = 0;
        int uiCount = Mathf.Min(questList.Count, _potionNameText.Length);

        for (int i = 0; i < _potionNameText.Length; i++)
        {
            int gold = 0;
            if(i <uiCount)
            {
                Quest quest = questList[i];

                if (questResult.ContainsKey(quest) && questResult[quest])
                {
                    _potionNameText[i].color = Constants.POTION_SUCC_GREEN;
                    _moneyText[i].color = Constants.POTION_SUCC_GREEN;
                    gold = quest.SelectRewardMoney;
                }
                else
                {
                    _potionNameText[i].color = Color.red;
                    _moneyText[i].color = Color.red;
                    gold -= (int)(quest.QuestRewardMoney * Constants.QUEST_PENALTY_RATIO);
                }
                yield return StartCoroutine(TypingCorutine($"{quest.PotionName} Á¦Á¶", _potionNameText[i]));

                _potionNameText[i].enabled = true;
                _moneyText[i].enabled = true;
                totalGold += gold;
                yield return StartCoroutine(TypingCorutine($"{gold} G", _moneyText[i]));
            }
            else
            {
                _potionNameText[i].enabled = false;
                _moneyText[i].enabled = false;
            }

        }

        int currentTarget = _targetMoney[_playInfo.CurrentDay];
        yield return StartCoroutine(TypingCorutine($"{totalGold} / {_targetMoney[_playInfo.CurrentDay]}", _targetMoneyText));
        _resultImage.enabled = true;

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
        _nextButton.interactable = true;
    }

    public IEnumerator TypingCorutine(string str,Text text)
    {
        text.text = "";
        yield return new WaitForSeconds(1f);

        int typingLength = str.GetTypingLength();
        for (int i = 0; i <= typingLength; i++)
        {
            text.text = str.Typing(i);
            if(_isRunningTypingSFXCorutine == false)
                StartCoroutine(TypingSoundgCorutine());
            yield return new WaitForSeconds(_typingSpeed);
        }

    }

    public IEnumerator TypingSoundgCorutine()
    {
        _isRunningTypingSFXCorutine = true;
        SoundManager._Instance.PlaySFX2D(SFXType.Writing);
        yield return new WaitForSeconds(_typingSFXSpeed);
        _isRunningTypingSFXCorutine = false;
    }

    public void TryNextDay()
    {
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Click);
        GameManager.GM.TryNextDay();
    }

    private void ResetUIText()
    {
        int uiCount = Mathf.Min(_board.AcceptQuestList.Count, _potionNameText.Length);

        for (int i = 0; i < _potionNameText.Length; i++)
        {
            _potionNameText[i].text = "";
            _moneyText[i].text = "";
            _targetMoneyText.text = "";
        }
    }
}
