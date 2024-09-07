using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionBrewer : MonoBehaviour
{
    //Component
    [SerializeField]
    private QuestBoard _board;
    [SerializeField]
    private Slot[] _slots;

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

    [SerializeField]
    private Button _craftButton;

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

    void Start()
    {
        //레퍼런스 초기화
        _canvas.worldCamera = GameManager.MainCamera;
        _board = GameManager.Board;

        //슬롯의 양조기 접근을 위한 레퍼런스 할당
        for(int i = 0; i< _slots.Length; ++i)
        {
            _slots[i].Brewer = this;
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

        var potionInfo = _currentQuest.PInfo;
        _ingreCnt = potionInfo.ingredientCount;
        if (_ingreCnt > 0)
        {
            for (int i = _ingreCnt; i <= 2; ++i)
            {
                _ingredientInputAmountText[i].enabled = false;
                _ingredientImage[i].enabled = false;
                _slots[i].gameObject.SetActive(false);
                _slots[i].InitializeIngredient();
            }
        }

        for (int i = 0; i < _ingreCnt; ++i)
        {
            _maxMount[i] = potionInfo.maxMount[i];
            _ingredientInputAmountText[i].text = _currentMount[i].ToString() + " / " + _maxMount[i].ToString();
        }
        UpdateQuestUIInfo();
    }

    //현재 의뢰 정보 UI를 업데이트하는 함수
    private void UpdateQuestUIInfo()
    {
        _questText.text = _currentQuest.QuestText;
        _potionImage.sprite = _currentQuest.PotionImage;
        _potionNameText.text = _currentQuest.PotionName;
        _reqQulaityValueText.text = _currentQuest.PotionQualityValue;
    }

    public void PotionCraft()
    {
        if(_currentQuest.RequirePotionQuality <= _currentPotionQuality)
        {
            Debug.Log("포션제조 성공");
        }
       else
        {
            Debug.Log("포션제조 실패");
        }
        //제조 결과 UI 띄우기
        //다음의뢰 가져오기
        if (_board._accpetQuestList.Count-1 > _currentQuestIndex)
        {
            _currentQuestIndex++;
            _currentQuest = _board._accpetQuestList[_currentQuestIndex];
            UpdateQuestInfo(_currentQuestIndex);
        }
        else
        {
            GameManager.CheckQuest();
        }
    }
    //재료 투입 함수
    public void InsertIngredient(int slotId, int mount)
    {
        int prevValue = _currentMount[slotId];
        if (_maxMount[slotId] >= _currentMount[slotId] + mount)
            _currentMount[slotId] += mount;
        else
            _currentMount[slotId] = _maxMount[slotId];

        if (prevValue == _currentMount[slotId]) return;

        //다 차면 빨간색상으로
        if (_maxMount[slotId] == _currentMount[slotId])
            _ingredientInputAmountText[slotId].color = Color.red;

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
