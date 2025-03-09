using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionBrewer : MonoBehaviour
{
    //Component
    [SerializeField]
    private IngredientSlot[] _slots;
    [SerializeField]
    private Store _storeUI;

    //UI
    [SerializeField]
    private Canvas _canvas;
    //재료 투입 현황 텍스트
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //투입 결과 오브젝트
    [SerializeField]
    private GameObject[] _capacityObject;
    //제조버튼
    [SerializeField]
    private Button _craftButton;

    //보상UI 오브젝트
    [SerializeField]
    private CraftResult _craftResult;

    //현재 제조 포션 정보
    [Header("Current Quest Info")]
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private Text _potionNameText;
    [SerializeField]
    private Text _questText;
    [SerializeField]
    private Text _reqQulaityValueText;
    //현재 표시하는 Quest 정보
    [SerializeField]
    private int _currentQuestIndex;
    public Quest _currentQuest;

    [Header("Current Recipe Info")]
    [SerializeField]
    private Text _recipeNameText;
    [SerializeField]
    private Text[] _IngridientNameText;

    //솥 표기 정보
    //재료 종류
    private int _ingreCnt;

    //현재 투입한 수량
    public int[] _currentMount;
    //최대 투입량
    private int[] _maxMount;

    private int _currentPotionQuality;

    //Getter Setter
    public int CurrentPotionQuality
    {
        get { return _currentPotionQuality; }
    }

    public Store StoreUI
    {
        get { return _storeUI; }
    }

    public QuestBoard Board
    {
        get { return GameManager.GM.Board; }
    }

    void Start()
    {
        InitializeBrewer();
    }

    public void InitializeBrewer()
    {
        //레퍼런스 초기화
        _canvas.worldCamera = GameManager.GM.MainCamera;

        _craftResult.gameObject.SetActive(false);

        //슬롯의id 설정
        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotId = i;
        }
        _craftButton.onClick.RemoveAllListeners();
        _craftButton.onClick.AddListener(PotionCraft);
    }

    //포션 제조에 성공했는지 확인하는 함수
    private bool IsSuccCraft()
    {
        bool ret = true;

        //증가 가루 버프 체크
        GameManager.GM.BM.CheckBuff(BuffType.PlusPowder,ref _currentPotionQuality);
        
        //요구 품질보다 낮을 때
        if (_currentQuest.RequirePotionQuality > _currentPotionQuality)
            return false;

        //재료를 오버해서 넣었을때
        foreach (var slot in _slots)
        {
            if (slot.enabled == true)
            {
                int sid = slot.SlotId;
                if (_currentMount[sid] > _maxMount[sid]) return false;
            }
        }
        return ret;
    }

    //포션 제조 함수
    public void PotionCraft()
    {
        //제조 결과 UI 띄우기
        if (IsSuccCraft())
        {
            _craftResult.ShowCraftResultUI(true);
            Board.SetQuestResult(_currentQuest, true);
        }
        else
        {
            _craftResult.ShowCraftResultUI(false);
            Board.SetQuestResult(_currentQuest, false);
        }
        _craftResult.PotionQuality = _currentPotionQuality;
        _reqQulaityValueText.text = _currentQuest.RequirePotionQuality.ToString();

    }
    //다음 제조 혹은 정산 단계로 이동하는 함수
    public void GetNextCraft()
    {
        _currentQuestIndex++;
        if (Board.CurrrentAcceptQuestCnt > _currentQuestIndex)
        {
            GameManager.GM.SM.SaveQuestOrder(_currentQuestIndex);
            UpdateQuestInfo(_currentQuestIndex);
        }
        else
        {
            _currentQuestIndex = 0;
            GameManager.GM.Receipt.UpdateReceipt();
            GameManager.GM.ShowCraftReceipt();
        }
    }

    //재료 투입 함수
    public void InsertIngredient(int slotId, int mount)
    {
        int prevValue = _currentMount[slotId];
        _currentMount[slotId] += mount;

        if (prevValue == _currentMount[slotId]) return;

        //다 차면 빨간색상
        if (_maxMount[slotId] <= _currentMount[slotId])
        {
            _ingredientInputAmountText[slotId].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

        //수량 텍스트 업데이트
        //추후 비율 DB 업데이트후 수정예정
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //현재 품질 업데이트
        _currentPotionQuality = 0;
        foreach (var val in _currentMount)
        {
            _currentPotionQuality += val;
        }
    }

    //지금 제조하는 의뢰에 맞게 양조기 정보를 업데이트한다.
    public void UpdateQuestInfo(int questIndex = 0)
    {
        //if (_currentQuest != null) return;
        _currentQuestIndex = questIndex;
        _currentQuest = Board.GetCurrentQuest(_currentQuestIndex);

        _currentMount = new int[3];
        _maxMount = new int[3];

        _currentPotionQuality = 0;

        var potionInfo = _currentQuest.PInfo;
        _ingreCnt = potionInfo.ingredientCount;

        _recipeNameText.text = potionInfo.potionName;
        int index = 1;
        for (int i = 0; i < 3; ++i)
        {
            _capacityObject[i].SetActive(true);
            _slots[i].gameObject.SetActive(true);
            _slots[i].InitializeSlot();
            _slots[i].EnableInputButton();

            if (potionInfo.ingredientName[i] == "x")
                _IngridientNameText[i].text = "";
            else
                _IngridientNameText[i].text = (index++).ToString() + ". "+ potionInfo.ingredientName[i];

            _ingredientInputAmountText[i].color = Color.black;
            _maxMount[i] = potionInfo.maxMount[i] * _currentQuest.QuestGrade;
            _ingredientInputAmountText[i].text = _currentMount[i].ToString() + " / " + _maxMount[i].ToString();
        }

        //의뢰 별 투입되는 재료의 종류(투입구의 수)에 따라 UI가 바뀐다.
        if (_ingreCnt == 1) //2번째만
        {
            for (int i = 0; i < 3; ++i)
            {
                if (i == 1) continue;
                _capacityObject[i].SetActive(false);
                _slots[i].gameObject.SetActive(false);
            }
        }
        else if (_ingreCnt == 2) //1 3번째만
        {
            _capacityObject[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);

        }
        else if (_ingreCnt == 3) ;

        UpdateQuestUIInfo();

    }

    //현재 의뢰 정보 UI를 업데이트하는 함수
    private void UpdateQuestUIInfo()
    {
        _questText.text = _currentQuest.QuestText;
        _potionImage.sprite = _currentQuest.PotionImage;
        _potionNameText.text = _currentQuest.PotionName;
        _reqQulaityValueText.text = _currentQuest.PotionQualityValue;
        _craftButton.enabled = true;
    }

}
