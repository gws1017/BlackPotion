using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
using static SaveManager;

public class PotionBrewer : MonoBehaviour
{
    private const int INGREDIENT_SLOT_COUNT = 3;

    //Component
    [Header("Main Component")]
    [SerializeField] private IngredientSlot[] _slots;
    [SerializeField] private Store _storeUI;
    [SerializeField] private GameObject _storeUIPrefab;
    private GameObject _storeObject;
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
    [SerializeField] private Image _questGradeImage;
    [SerializeField] private Text _potionNameText;
    [SerializeField] private Outline _potionNameOutline;
    [SerializeField] private Text _questText;
    [SerializeField] private Text _questTitleText;
    [SerializeField] private Text _reqCapacityValueText;
    [SerializeField] private Text _questProgressText;
    [SerializeField] private int _currentQuestIndex;
    private Quest _currentQuest;

    [Header("Current Recipe Info")]
    [SerializeField] private Text _recipeNameText;
    [SerializeField] private Text[] _ingredientNameText = new Text[INGREDIENT_SLOT_COUNT];
    [SerializeField] private Text[] _inputIngredientNameText = new Text[INGREDIENT_SLOT_COUNT];

    //내부 변수
    private int _ingredientCount;
    private int[] _currentAmount = new int[INGREDIENT_SLOT_COUNT];
    private int[] _maxAmount = new int[INGREDIENT_SLOT_COUNT];
    private int _currentPotionCapacity;
    public bool _activePlusPowder;

    public IngredientSlot[] Slots => _slots;
    public enum CraftState
    {
        None,
        Retry,
        Complete
    }

    private CraftState _craftState;
    public CraftState CurrentCraftState { get => _craftState; set => _craftState = value; }

    //Getter Setter
    public int CurrentPotionCapacity => _currentPotionCapacity;
    public int CurrentQuestIndex => _currentQuestIndex;
    public int[] MaxAmount => _maxAmount;
    public int[] CurrentAmount => _currentAmount;

    public Text ReqCapacityValueText => _reqCapacityValueText;

    public Store StoreUI 
    {
        get
        {
            if(_storeUI == null)
            {
                if(_storeObject == null)
                {
                    _storeObject = Instantiate(_storeUIPrefab);
                }
                _storeUI = _storeObject.GetComponent<Store>();
            }
            return _storeUI;
        }
    }
    public QuestBoard Board => GameManager.GM.Board;
    public Quest CurrentQuest => _currentQuest;

    void Start()
    {
        InitializeBrewer();
    }

    public void InitializeBrewer()
    {
        _gameManager = GameManager.GM;
        _canvas.worldCamera = _gameManager.MainCamera;
        _craftResult.gameObject.SetActive(false);

        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotId = i;
        }
        _craftButton.onClick.RemoveAllListeners();
        _craftButton.onClick.AddListener(CraftPotion);
    }

    public bool IsFullSlot()
    {
        foreach (var slot in _slots)
        {
            int sid = slot.SlotId;
            if (slot.enabled && _currentAmount[sid] > _maxAmount[sid])
                return false;
        }

        return true;
    }

    public bool IsEmptySlot()
    {
        for (int i = 0; i < CurrentAmount.Length; ++i)
        {
            if (MaxAmount[i] > 0 && CurrentAmount[i] == 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCraftSuccessful()
    {
        _gameManager.BM.CheckBuff(BuffType.PlusPowder, ref _currentPotionCapacity);
        bool ret = (_currentQuest.RequirePotionCapacity <= _currentPotionCapacity);

        ret &= IsFullSlot();

        ret &= IsEmptySlot();

        return ret;
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

        if (_craftState == CraftState.Complete)
        {
            ProcessCraftResult();
        }
        else if (_craftState == CraftState.Retry)
        {
            ProcessCraftRetry();
        }
    }

    public void ProcessCraftResult()
    {
        bool isSuccess = IsCraftSuccessful();
        _craftResult.IsPotionCraftSuccessful = isSuccess;
        Board.SetQuestResult(_currentQuest, isSuccess);

        _gameManager.SM.SaveQuestResult(CurrentQuestIndex, isSuccess);

        _craftButton.gameObject.SetActive(false);

        _gameManager.SM.SaveVisibleCraftResult(true);
        _craftResult.UpdateCraftResultUI();
        _reqCapacityValueText.text = _currentQuest.RequirePotionCapacity.ToString();
    }

    private void ProcessCraftRetry()
    {
        _gameManager.PlayInformation.ConsumeGold(Constants.RETRY_GOLD);
        _currentQuest.IsRestart = true;
        UpdateQuestInfo(_currentQuestIndex);
    }

    public void ShowCraftResultUI()
    {
        _craftResult.ShowCraftResultUI();
        ProcessCraftResult();
    }

    public void GetNextCraft()
    {
        _currentQuestIndex++;

        _gameManager.SM.SaveVisibleCraftResult(false);
        _gameManager.SM.SaveFailPenalty(false);
        _gameManager.SM.SaveQuestOrder(_currentQuestIndex);
        if (Board.CurrentAcceptQuestCount > _currentQuestIndex)
        {
            UpdateQuestInfo(_currentQuestIndex);
        }
        else
        {
            _currentQuestIndex = 0;
            _gameManager.ShowCraftReceipt();
            StartCoroutine(_gameManager.Receipt.UpdateReceiptCorutine());
        }
        _craftButton.gameObject.SetActive(true);
    }

    public void InsertIngredient(int slotId,int ingridientIndex, int amount,IngridientEvent oev = null)
    {
        int prevValue = _currentAmount[slotId];

        //양조기강화 버프 체크
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        _currentAmount[slotId] += amount;

        //GameManager.GM.SM.SaveInputAmount(slotId, amount);

        IngridientEvent ev = new IngridientEvent
        {
            slotId = slotId,
            type = InputEventType.FreeInput,
            value = amount
        };

        if (oev !=null)
            ev = oev;
         GameManager.GM.SM.SaveInputEvent(ev, _currentQuestIndex);

        if (prevValue == _currentAmount[slotId]) return;
        if (_maxAmount[slotId] <= _currentAmount[slotId])
        {
            if(_maxAmount[slotId] < _currentAmount[slotId])
                _ingredientInputAmountText[ingridientIndex].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

        _ingredientInputAmountText[ingridientIndex].text =
            $"{_currentAmount[slotId]} / {_maxAmount[slotId]}";

        _currentPotionCapacity = 0;
        _currentPotionCapacity = _currentAmount.Sum();
    }

    public void SetCurrentAmount(int slotId, int ingridientIndex,int value)
    {
        if (_maxAmount[slotId] <= 0) return;

        _currentAmount[slotId] = value;
        _ingredientInputAmountText[ingridientIndex].text =
            $"{_currentAmount[slotId]} / {_maxAmount[slotId]}";

        if(_currentAmount[slotId] / _maxAmount[slotId] < 1)
        {
            _ingredientInputAmountText[ingridientIndex].color = Color.black;
            _slots[slotId].EnableInputButton();
        }
    }

    public void UpdateQuestInfo(int questIndex = 0)
    {
        _currentQuestIndex = questIndex;
        _currentQuest = Board.GetCurrentQuest(_currentQuestIndex);

        _gameManager.SM.SaveQuestRestart(CurrentQuestIndex, _currentQuest.IsRestart);

        var potionInfo = _currentQuest.PInfo;

        int amountMultiply = 0;
        foreach (var ratio in potionInfo.materialRatioList)
            amountMultiply += ratio;
        amountMultiply = _currentQuest.QInfo.maxCapacity / amountMultiply;

        _currentAmount = new int[INGREDIENT_SLOT_COUNT];
        _maxAmount = new int[INGREDIENT_SLOT_COUNT];
        _currentPotionCapacity = 0;
        
        _recipeNameText.text = potionInfo.potionName;

        int uiIndex = 1;
        int ingridientCount = 0;
        for (int i = 0; i < INGREDIENT_SLOT_COUNT; ++i)
        {
            _capacityObjects[ingridientCount].SetActive(true);
            _slots[i].gameObject.SetActive(true);
            _slots[i].InitializeSlot();
            _slots[i].EnableInputButton();
            _slots[i].IngridientIndex = ingridientCount;

            GameManager.GM.SM.SaveSlotInfo(i, _slots[i].IngridientIndex);

            int ingredientId = potionInfo.ingredientIdList[i];

            _ingredientInputAmountText[i].color = Color.black;
            _maxAmount[i] = (potionInfo.materialRatioList[i] * amountMultiply);
            _ingredientInputAmountText[ingridientCount].text = $"{_currentAmount[i]} / {_maxAmount[i]}";

            if (ingredientId != 0)
            {
                _inputIngredientNameText[ingridientCount].text = ReadJson._dictMaterial[ingredientId].materialName;
                _ingredientNameText[ingridientCount].text = $"{uiIndex++}. {_inputIngredientNameText[ingridientCount].text}";
                ingridientCount++;
            }
        }

        for(int i = ingridientCount; i< INGREDIENT_SLOT_COUNT; ++i)
        {
            _capacityObjects[i].SetActive(false);
            _ingredientNameText[i].text = string.Empty;
        }
        
        _ingredientCount = ingridientCount;
        // 재료 투입 슬롯 조정
        if (_ingredientCount == 1)
        {
            for (int i = 0; i < INGREDIENT_SLOT_COUNT; ++i)
            {
                if (i != 1)//가운데만
                {
                    //_capacityObjects[i].SetActive(false);
                    _slots[i].gameObject.SetActive(false);
                }
            }
        }
        else if (_ingredientCount == 2)
        {
            //_capacityObjects[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);
        }

        UpdateQuestUIInfo();
    }

    private void UpdateQuestUIInfo()
    {
        _questText.text = _currentQuest.QuestText;
        _questTitleText.text = _currentQuest.QuestName;
        _questProgressText.text = $"의뢰 {_currentQuestIndex + 1} / {Board.CurrentAcceptQuestCount}";
        _potionImage.sprite = _currentQuest.PotionImage;
        _questGradeImage.sprite = Constants.GetQuestGradeMark(_currentQuest.QuestGrade);
        _potionNameText.text = _currentQuest.PotionName;
        _potionNameOutline.effectColor = Constants.RecipeGradeToColor((RecipeGrade)_currentQuest.PInfo.potionGrade);
        _reqCapacityValueText.text = _currentQuest.PotionCapacityValue;
        _craftButton.enabled = true;
    }

}
