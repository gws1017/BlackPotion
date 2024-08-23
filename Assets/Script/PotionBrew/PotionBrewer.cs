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
    //��� ���� ��Ȳ �ؽ�Ʈ
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //���� ǰ�� ��ġ �ؽ�Ʈ
    [SerializeField]
    private Text _currentQualityText;
    //��� �̹���
    [SerializeField]
    private Image[] _ingredientImage;


    //���� ǥ���ϴ� Quest ����
    [SerializeField]
    private int _currentQuestIndex;
    public Quest _currentQuest;

    //�� ǥ�� ����
    //��� ����
    private int _ingreCnt;

    //���� ������ ����
    public int[] _currentMount;
    //�ִ� ���Է�
    private int[] _maxMount;
  

    void Start()
    {
        //���۷��� �ʱ�ȭ
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
                //slot ��Ȱ��ȭ
            }
        }

        for (int i = 0; i < _ingreCnt; ++i)
        {
            _maxMount[i] = potionInfo.maxMount[i];
            _ingredientInputAmountText[i].text = _currentMount[i].ToString() + " / " + _maxMount[i].ToString();
        }
    }

    //��� ���� �Լ�
    public void InsertIngredient(int slotId, int mount)
    {
        if (_maxMount[slotId] >= _currentMount[slotId] + mount)
        {
            _currentMount[slotId] += mount;
            //�� ���� ������������
            if (_maxMount[slotId] == _currentMount[slotId])
                _ingredientInputAmountText[slotId].color = Color.red;
        }
        //���� �ؽ�Ʈ ������Ʈ
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //���� ǰ�� ������Ʈ
        int sum = 0;
        foreach(var val in _currentMount)
        {
            sum += val;
        }
        _currentQualityText.text = sum.ToString();


    }

}
