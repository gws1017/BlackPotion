using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PotionBrewer : MonoBehaviour
{
    private const int INGREDIENT_SLOT_COUNT = 3;

    //Component
    [Header("Main Component")]
    [SerializeField] private IngredientSlot[] _slots;
    [SerializeField] private Store _storeUI;
    private GameManager _gameManager;

    //UI
    [Header("UI")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Text[] _ingredientInputAmountText;
    [SerializeField] private GameObject[] _capacityObjects = new GameObject[INGREDIENT_SLOT_COUNT];
    [SerializeField] private Button _craftButton;
    [SerializeField] private CraftResult _craftResult;

    //현재 제조 포션 정보
    [Header("Current Quest Info")]
    [SerializeField] private Image _potionImage;
    [SerializeField] private Text _potionNameText;
    [SerializeField] private Text _questText;
    [SerializeField] private Text _reqQualityValueText;
    [SerializeField] private int _currentQuestIndex;
    private Quest _currentQuest;

    [Header("Current Recipe Info")]
    [SerializeField] private Text _recipeNameText;
    [SerializeField] private Text[] _ingredientNameText = new Text[INGREDIENT_SLOT_COUNT];

    //내부 변수
    private int _ingredientCount;
    private int[] _currentAmount;
    private int[] _maxAmount;
    private int _currentPotionQuality;

    public enum CraftState
    {
        None,
        Retry,
        Success
    }

    private CraftState _craftState;
    public CraftState CurrentCraftState { get => _craftState; set => _craftState = value; }

    //Getter Setter
    public int CurrentPotionQuality => _currentPotionQuality;
    public Store StoreUI => _storeUI;
    public QuestBoard Board => GameManager.GM.Board;
    public Quest CurrentQuest => _currentQuest;

    void Start()
    {
        _gameManager = GameManager.GM;
        InitializeBrewer();
    }

    public void InitializeBrewer()
    {
        _canvas.worldCamera = _gameManager.MainCamera;
        _craftResult.gameObject.SetActive(false);

        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotId = i;
        }
        _craftButton.onClick.RemoveAllListeners();
        _craftButton.onClick.AddListener(CraftPotion);
    }

    private bool IsCraftSuccessful()
    {
        _gameManager.BM.CheckBuff(BuffType.PlusPowder, ref _currentPotionQuality);

        if (_currentQuest.RequirePotionQuality > _currentPotionQuality)
            return false;

        foreach (var slot in _slots)
        {
            int sid = slot.SlotId;
            if (slot.enabled && _currentAmount[sid] > _maxAmount[sid])
                return false;
        }
        return true;
    }

    public void CraftPotion()
    {
        StartCoroutine(CraftPotionCoroutine());
    }

    public IEnumerator CraftPotionCoroutine()
    {
        _craftResult.ShowResultCheckUI();
        _craftState = CraftState.None;

        yield return new WaitUntil(() => _craftState != CraftState.None);

        if (_craftState == CraftState.Success)
        {
            ProcessCraftResult();
        }
        else if (_craftState == CraftState.Retry)
        {
            ProcessCraftRetry();
        }
    }

    private void ProcessCraftResult()
    {
        bool isSuccess = IsCraftSuccessful();
        _craftResult.IsPotionCraftSuccessful = isSuccess;
        Board.SetQuestResult(_currentQuest, isSuccess);

        _craftResult.UpdateCraftResultUI();
        _craftResult.PotionQuality = _currentPotionQuality;
        _reqQualityValueText.text = _currentQuest.RequirePotionQuality.ToString();
    }

    private void ProcessCraftRetry()
    {
        _gameManager.PlayInformation.ConsumeGold(PlayInfo.RESTART_GOLD);
        _currentQuest.IsRestart = true;
        UpdateQuestInfo(_currentQuestIndex);
        _gameManager.SM.SaveQuest();
    }

    public void GetNextCraft()
    {
        _currentQuestIndex++;
        if (Board.CurrentAcceptQuestCount > _currentQuestIndex)
        {
            _gameManager.SM.SaveQuestOrder(_currentQuestIndex);
            UpdateQuestInfo(_currentQuestIndex);
        }
        else
        {
            _currentQuestIndex = 0;
            _gameManager.Receipt.UpdateReceipt();
            _gameManager.ShowCraftReceipt();
        }
    }

    public void InsertIngredient(int slotId, int amount)
    {
        int prevValue = _currentAmount[slotId];
        _currentAmount[slotId] += amount;
        if (prevValue == _currentAmount[slotId]) return;

        if (_maxAmount[slotId] <= _currentAmount[slotId])
        {
            _ingredientInputAmountText[slotId].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

        //수량 텍스트 업데이트
        //추후 비율 DB 업데이트후 수정예정
        _ingredientInputAmountText[slotId].text =
            $"{_currentAmount[slotId]} / {_maxAmount[slotId]}";

        _currentPotionQuality = 0;
        _currentPotionQuality = _currentAmount.Sum();
    }

    public void UpdateQuestInfo(int questIndex = 0)
    {
        _currentQuestIndex = questIndex;
        _currentQuest = Board.GetCurrentQuest(_currentQuestIndex);

        _currentAmount = new int[INGREDIENT_SLOT_COUNT];
        _maxAmount = new int[INGREDIENT_SLOT_COUNT];
        _currentPotionQuality = 0;

        var potionInfo = _currentQuest.PInfo;
        _ingredientCount = potionInfo.ingredientCount;
        _recipeNameText.text = potionInfo.potionName;

        int uiIndex = 1;
        for (int i = 0; i < INGREDIENT_SLOT_COUNT; ++i)
        {
            _capacityObjects[i].SetActive(true);
            _slots[i].gameObject.SetActive(true);
            _slots[i].InitializeSlot();
            _slots[i].EnableInputButton();

            _ingredientNameText[i].text = (potionInfo.ingredientName[i] == "x")
                ? string.Empty
                : $"{uiIndex++}. {potionInfo.ingredientName[i]}";

            _ingredientInputAmountText[i].color = Color.black;
            _maxAmount[i] = potionInfo.maxMount[i] * _currentQuest.QuestGrade;
            _ingredientInputAmountText[i].text = $"{_currentAmount[i]} / {_maxAmount[i]}";
        }

        // 재료 투입 슬롯 조정
        if (_ingredientCount == 1)
        {
            for (int i = 0; i < INGREDIENT_SLOT_COUNT; ++i)
            {
                if (i != 1)//가운데만
                {
                    _capacityObjects[i].SetActive(false);
                    _slots[i].gameObject.SetActive(false);
                }
            }
        }
        else if (_ingredientCount == 2)
        {
            _capacityObjects[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);
        }

        UpdateQuestUIInfo();
    }

    private void UpdateQuestUIInfo()
    {
        _questText.text = _currentQuest.QuestText;
        _potionImage.sprite = _currentQuest.PotionImage;
        _potionNameText.text = _currentQuest.PotionName;
        _reqQualityValueText.text = _currentQuest.PotionQualityValue;
        _craftButton.enabled = true;
    }

}
