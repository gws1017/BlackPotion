using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionBrewer : MonoBehaviour
{
    //Component
    [SerializeField]
    private QuestBoard _board;

    //UI
    //재료 투입 현황 텍스트
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //현재 품질 수치 텍스트
    [SerializeField]
    private Text _currentQualityText;
    //재료 이미지
    [SerializeField]
    private Image[] _ingredientImage;


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
  

    void Start()
    {
        //레퍼런스 초기화
        GetComponentInChildren<Canvas>().worldCamera = GameManager.MainCamera;
        _board = GameManager.Board;

    }
    public void UpdateQuestInfo(int questId = 0)
    {
        _currentQuestIndex = questId;
        _currentQuest = _board._accpetQuestList[_currentQuestIndex];

        _currentMount = new int[3];
        _maxMount = new int[3];

        var potionInfo = _currentQuest.PInfo;
        _ingreCnt = potionInfo.ingredientCount;
        if (_ingreCnt > 0)
        {
            for (int i = _ingreCnt; i <= 2; ++i)
            {
                _ingredientInputAmountText[i].enabled = false;
                _ingredientImage[i].enabled = false;
                //slot 비활성화
            }
        }

        for (int i = 0; i < _ingreCnt; ++i)
        {
            _maxMount[i] = potionInfo.maxMount[i];
            _ingredientInputAmountText[i].text = _currentMount[i].ToString() + " / " + _maxMount[i].ToString();
        }
    }

    //재료 투입 함수
    public void InsertIngredient(int slotId, int mount)
    {
        if (_maxMount[slotId] >= _currentMount[slotId] + mount)
        {
            _currentMount[slotId] += mount;
            //다 차면 빨간색상으로
            if (_maxMount[slotId] == _currentMount[slotId])
                _ingredientInputAmountText[slotId].color = Color.red;
        }
        //수량 텍스트 업데이트
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //현재 품질 업데이트
        int sum = 0;
        foreach(var val in _currentMount)
        {
            sum += val;
        }
        _currentQualityText.text = sum.ToString();


    }

}
