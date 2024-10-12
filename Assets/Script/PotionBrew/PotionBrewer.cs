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
    //��� ���� ��Ȳ �ؽ�Ʈ
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //���� ǰ�� ��ġ �ؽ�Ʈ
    [SerializeField]
    private Text _currentQualityText;
    //��� �̹���
    [SerializeField]
    private Image[] _ingredientImage;
    //���� ��� ������Ʈ
    [SerializeField]
    private GameObject[] _capacityObject;
    //������ư
    [SerializeField]
    private Button _craftButton;
    //+ �̹��� ������Ʈ 0 -> ���Ա� �ϳ��϶� 1 -> ���Ա�2
    [SerializeField]
    private GameObject[] _plusImageObjects;

    //����UI ������Ʈ
    [SerializeField]
    private CraftResult _craftResult;

    //���� ���� ���� ����
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
        //���۷��� �ʱ�ȭ
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;

        //������ ������ ������ ���� ���۷��� �Ҵ�
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

        if (_ingreCnt == 1) //2��°��
        {
            for (int i = 0; i < 3; ++i)
            {
                if (i == 1) continue;
                _capacityObject[i].SetActive(false);
                _slots[i].gameObject.SetActive(false);
            }
        }
        else if(_ingreCnt == 2) //1 3��°��
        {
            _plusImageObjects[_ingreCnt - 2].SetActive(true);
            _capacityObject[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);

        }
        else if(_ingreCnt == 3)
            _plusImageObjects[_ingreCnt - 2].SetActive(true);


        UpdateQuestUIInfo();
    }

    //���� �Ƿ� ���� UI�� ������Ʈ�ϴ� �Լ�
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

        //�䱸 ǰ������ ���� ��
        if (_currentQuest.RequirePotionQuality > _currentPotionQuality)
            return false;

        //��Ḧ �����ؼ� �־�����
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
        //���� ��� UI ����
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

    //��� ���� �Լ�
    public void InsertIngredient(int slotId, int mount)
    {
        //�ִ�뷮�̻����� ���԰����ϰ� �����ؾ���
        //�ִ� �뷮 �Ѿ�� �Ƿ� ������
        int prevValue = _currentMount[slotId];
        
        _currentMount[slotId] += mount;
        
        
        if (prevValue == _currentMount[slotId]) return;

        //�� ���� ������������
        if (_maxMount[slotId] <= _currentMount[slotId])
        {
            _ingredientInputAmountText[slotId].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

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
