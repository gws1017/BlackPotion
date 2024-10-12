using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionBrewer : MonoBehaviour
{
    //Component
    private QuestBoard _board;
    [SerializeField]
    private Slot[] _slots;
    [SerializeField]
    private Store _storeUI;

    //UI
    [SerializeField]
    private Canvas _canvas;
    //재료 투입 현황 텍스트
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //현재 품질 수치 텍스트
    [SerializeField]
    private Text _currentQualityText;
    //재료 이미지
    [SerializeField]
    private Image[] _ingredientImage;
    //투입 결과 오브젝트
    [SerializeField]
    private GameObject[] _capacityObject;
    //제조버튼
    [SerializeField]
    private Button _craftButton;
    //+ 이미지 오브젝트 0 -> 투입구 하나일때 1 -> 투입구2
    [SerializeField]
    private GameObject[] _plusImageObjects;

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

    void Start()
    {
        //레퍼런스 초기화
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;

        //슬롯의 양조기 접근을 위한 레퍼런스 할당
        for(int i = 0; i< _slots.Length; ++i)
        {
            _slots[i].SlotId = i;
        }
        _craftButton.onClick.AddListener(PotionCraft);
    }
    public void UpdateQuestInfo(int questIndex = 0)
    {
        _currentQuestIndex = questIndex;
        _currentQuest = _board._accpetQuestList[_currentQuestIndex];

        _currentMount = new int[3];
        _maxMount = new int[3];

        _currentPotionQuality = 0;
        _currentQualityText.text = _currentPotionQuality.ToString();

        var potionInfo = _currentQuest.PInfo;
        _ingreCnt = potionInfo.ingredientCount;

        for (int i = 0; i < 3; ++i)
        {
            if(i!=2)_plusImageObjects[i].SetActive(false);
            _capacityObject[i].SetActive(true);
            _slots[i].gameObject.SetActive(true);
            _slots[i].InitializeIngredient();
            _slots[i].EnableInputButton();
            _ingredientInputAmountText[i].color = Color.black;
            _maxMount[i] = potionInfo.maxMount[i] * _currentQuest.QuestGrade;
            _ingredientInputAmountText[i].text = _currentMount[i].ToString() + " / " + _maxMount[i].ToString();
        }

        if (_ingreCnt == 1) //2번째만
        {
            for (int i = 0; i < 3; ++i)
            {
                if (i == 1) continue;
                _capacityObject[i].SetActive(false);
                _slots[i].gameObject.SetActive(false);
            }
        }
        else if(_ingreCnt == 2) //1 3번째만
        {
            _plusImageObjects[_ingreCnt - 2].SetActive(true);
            _capacityObject[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);

        }
        else if(_ingreCnt == 3)
            _plusImageObjects[_ingreCnt - 2].SetActive(true);


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

    private bool IsSuccCraft()
    {
        bool ret = true;

        //요구 품질보다 낮을 때
        if (_currentQuest.RequirePotionQuality > _currentPotionQuality)
            return false;

        //재료를 오버해서 넣었을때
        foreach(var slot in _slots)
        {
            if (slot.enabled == true)
            {
                int sid = slot.SlotId;
                if (_currentMount[sid] > _maxMount[sid]) return false;
            }
        }

        return ret;
    }

    public void PotionCraft()
    {
        //제조 결과 UI 띄우기
        _craftResult.PotionQuality = _currentPotionQuality;
        if (IsSuccCraft())
        {
            _craftResult.ShowCraftResult(true);
            _board.SetQuestResult(_currentQuest, true);
        }
       else
        {
            _craftResult.ShowCraftResult(false);
            _board.SetQuestResult(_currentQuest, false);
        }
        _reqQulaityValueText.text = _currentQuest.RequirePotionQuality.ToString();

    }
    public void GetNextCraft()
    {
        _currentQuestIndex++;
        Debug.Log(_currentQuestIndex);
        if (_board._accpetQuestList.Count > _currentQuestIndex)
        {
            _currentQuest = _board._accpetQuestList[_currentQuestIndex];
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
        //최대용량이상으로 투입가능하게 수정해야함
        //최대 용량 넘어가면 의뢰 실패임
        int prevValue = _currentMount[slotId];
        
        _currentMount[slotId] += mount;
        
        
        if (prevValue == _currentMount[slotId]) return;

        //다 차면 빨간색상으로
        if (_maxMount[slotId] <= _currentMount[slotId])
        {
            _ingredientInputAmountText[slotId].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

        //수량 텍스트 업데이트
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //현재 품질 업데이트
        _currentPotionQuality = 0;
        foreach (var val in _currentMount)
        {
            _currentPotionQuality += val;
        }
        _currentQualityText.text = _currentPotionQuality.ToString();


    }

}
