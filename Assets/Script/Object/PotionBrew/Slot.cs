using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    //UI
    //���� ��ư
    [SerializeField]
    private Button _inputButton;
    [SerializeField] 
    private Text _inputButtonText;
    //���� ������ ��
    [SerializeField]
    private Text _ingredientAmountText;
    [SerializeField]
    private int _ingredientAmount;

    public const int REFILL_GOLD = 10;

    public const int MAX_NUMBER = 10;

    public const int SUM_NUMBER = ((MAX_NUMBER - 1) * MAX_NUMBER) / 2;

    //PotionBrwer ���۷���
    private PotionBrewer _brewer;
    //���� ������ ID
    private int _slotId;

    //���� ���� ���Ե� ������ üũ�ϴ� Dictionary
    private Dictionary<int,int> _ingredientCountCheckDict = new Dictionary<int,int>();
    //1~13������ ���� �� �� ���� �� �ִ� ������ Ȯ���ϱ� ���� List
    private List<bool> _ingredientCountFullList = new List<bool>(14);

    //Getter Setter
    public int IngredientAmount
    {
        get
        {
            return _ingredientAmount;
        }
        set
        {
            _ingredientAmount = value;
            _ingredientAmountText.text = _ingredientAmount.ToString();
        }
    }

    public int SlotId
    {
        get { return _slotId; }
        set { _slotId = value; }
    }
    
    void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = GameManager.GM.MainCamera;
        _brewer = GameManager.GM.Brewer;

        _inputButton.onClick.AddListener(InputIngredient);

        _ingredientAmountText.text = _ingredientAmount.ToString();

        _ingredientCountCheckDict = new Dictionary<int, int>();
        _ingredientCountFullList = new List<bool> { false };
    }

    //���� �����ϴ� ���� �Ƿڿ� �°� �ʱ�ȭ�Ѵ�.
    public void InitializeSlot()
    {
        IngredientAmount = SUM_NUMBER * _brewer._currentQuest.QuestGrade;

        _ingredientAmountText.color = Color.black;

        for (int i = 1; i <= MAX_NUMBER; ++i) _ingredientCountCheckDict[i] = 0;
        _ingredientCountFullList = Enumerable.Repeat(false, MAX_NUMBER+1).ToList();
    }

    //��� ���� �Լ�
    private void InputIngredient()
    {
        //���� ������ ���� ���� �̰�
        int amount = GetRandomAmount();
        _brewer.InsertIngredient(SlotId, amount);
        //���������Ƿ� ���� ���� üũ
        _ingredientCountCheckDict[amount]++;

        if (_ingredientCountCheckDict[amount] >= _brewer._currentQuest.QuestGrade)
            _ingredientCountFullList[amount] = true;

        //�����Ⱝȭ ���� üũ
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        IngredientAmount -= amount;

        //��� �� ������� ���
        if (IngredientAmount <= 0)
        {
            _ingredientAmountText.color = Color.red;
            _inputButtonText.text = "��� ����";
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);

        }
    }

    //���Ե� ������ ������ �̴� �Լ�
    private int GetRandomAmount()
    {
        int amount = Random.Range(1, MAX_NUMBER);

        if (IsFullCount()) return 0;
        //1~13�� ���ڴ� ���� ���ڸ� �Ƿ� ��޸�ŭ ���� �� �ִ�.
        while (_ingredientCountCheckDict[amount] >= _brewer._currentQuest.QuestGrade)
        {
            amount = Random.Range(1, MAX_NUMBER);
        }
        //Ȧ¦���� Ȯ��
        GameManager.GM.BM.CheckBuff(BuffType.EvenOddNumber,ref amount);
        return amount;
    }

    //���̻� ���� ���� �� ���� ������ Ȯ���ϴ� �Լ�
    private bool IsFullCount()
    {
        bool ret = true;

        for (int i = 1; i <= MAX_NUMBER; ++i)
            if (_ingredientCountFullList[i] == false) ret = false;

        return ret;
    }

    //��� ���� �Լ�
    private void IngredientSupply()
    {
        Debug.Log("��Ḧ �����մϴ�!");
        //��带 �Ҹ���
        GameManager.GM.PlayInfomation.ConsumeGold(REFILL_GOLD);
        IngredientAmount = SUM_NUMBER;

        _inputButtonText.text = "����";
        _ingredientAmountText.color = Color.white;

        _ingredientCountCheckDict.Clear();
        _ingredientCountFullList.Clear();

        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

    //���� ��� ON / OFF
    public void DisableInputButton()
    {
        _inputButton.enabled = false;
    }
    public void EnableInputButton()
    {
        _inputButton.enabled = true;
    }
}
