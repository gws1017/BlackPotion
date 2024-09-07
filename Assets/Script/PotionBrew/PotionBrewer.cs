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
    //��� ���� ��Ȳ �ؽ�Ʈ
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //���� ǰ�� ��ġ �ؽ�Ʈ
    [SerializeField]
    private Text _currentQualityText;
    //��� �̹���
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

    private int _currentPotionQuality;

    void Start()
    {
        //���۷��� �ʱ�ȭ
        _canvas.worldCamera = GameManager.MainCamera;
        _board = GameManager.Board;

        //������ ������ ������ ���� ���۷��� �Ҵ�
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

    //���� �Ƿ� ���� UI�� ������Ʈ�ϴ� �Լ�
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
            Debug.Log("�������� ����");
        }
       else
        {
            Debug.Log("�������� ����");
        }
        //���� ��� UI ����
        //�����Ƿ� ��������
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
    //��� ���� �Լ�
    public void InsertIngredient(int slotId, int mount)
    {
        int prevValue = _currentMount[slotId];
        if (_maxMount[slotId] >= _currentMount[slotId] + mount)
            _currentMount[slotId] += mount;
        else
            _currentMount[slotId] = _maxMount[slotId];

        if (prevValue == _currentMount[slotId]) return;

        //�� ���� ������������
        if (_maxMount[slotId] == _currentMount[slotId])
            _ingredientInputAmountText[slotId].color = Color.red;

        //���� �ؽ�Ʈ ������Ʈ
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //���� ǰ�� ������Ʈ
        _currentPotionQuality = 0;
        foreach (var val in _currentMount)
        {
            _currentPotionQuality += val;
        }
        _currentQualityText.text = _currentPotionQuality.ToString();


    }

}
