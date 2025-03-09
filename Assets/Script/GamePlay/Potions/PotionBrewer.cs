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
    //��� ���� ��Ȳ �ؽ�Ʈ
    [SerializeField]
    private Text[] _ingredientInputAmountText;
    //���� ��� ������Ʈ
    [SerializeField]
    private GameObject[] _capacityObject;
    //������ư
    [SerializeField]
    private Button _craftButton;

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

    [Header("Current Recipe Info")]
    [SerializeField]
    private Text _recipeNameText;
    [SerializeField]
    private Text[] _IngridientNameText;

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
        //���۷��� �ʱ�ȭ
        _canvas.worldCamera = GameManager.GM.MainCamera;

        _craftResult.gameObject.SetActive(false);

        //������id ����
        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotId = i;
        }
        _craftButton.onClick.RemoveAllListeners();
        _craftButton.onClick.AddListener(PotionCraft);
    }

    //���� ������ �����ߴ��� Ȯ���ϴ� �Լ�
    private bool IsSuccCraft()
    {
        bool ret = true;

        //���� ���� ���� üũ
        GameManager.GM.BM.CheckBuff(BuffType.PlusPowder,ref _currentPotionQuality);
        
        //�䱸 ǰ������ ���� ��
        if (_currentQuest.RequirePotionQuality > _currentPotionQuality)
            return false;

        //��Ḧ �����ؼ� �־�����
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

    //���� ���� �Լ�
    public void PotionCraft()
    {
        //���� ��� UI ����
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
    //���� ���� Ȥ�� ���� �ܰ�� �̵��ϴ� �Լ�
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

    //��� ���� �Լ�
    public void InsertIngredient(int slotId, int mount)
    {
        int prevValue = _currentMount[slotId];
        _currentMount[slotId] += mount;

        if (prevValue == _currentMount[slotId]) return;

        //�� ���� ��������
        if (_maxMount[slotId] <= _currentMount[slotId])
        {
            _ingredientInputAmountText[slotId].color = Color.red;
            _slots[slotId].DisableInputButton();
        }

        //���� �ؽ�Ʈ ������Ʈ
        //���� ���� DB ������Ʈ�� ��������
        _ingredientInputAmountText[slotId].text = _currentMount[slotId].ToString() + " / " + _maxMount[slotId].ToString();

        //���� ǰ�� ������Ʈ
        _currentPotionQuality = 0;
        foreach (var val in _currentMount)
        {
            _currentPotionQuality += val;
        }
    }

    //���� �����ϴ� �Ƿڿ� �°� ������ ������ ������Ʈ�Ѵ�.
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

        //�Ƿ� �� ���ԵǴ� ����� ����(���Ա��� ��)�� ���� UI�� �ٲ��.
        if (_ingreCnt == 1) //2��°��
        {
            for (int i = 0; i < 3; ++i)
            {
                if (i == 1) continue;
                _capacityObject[i].SetActive(false);
                _slots[i].gameObject.SetActive(false);
            }
        }
        else if (_ingreCnt == 2) //1 3��°��
        {
            _capacityObject[1].SetActive(false);
            _slots[1].gameObject.SetActive(false);

        }
        else if (_ingreCnt == 3) ;

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

}
