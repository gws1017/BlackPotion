using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private PotionBrewer _potionBrewer;

    //UI
    [SerializeField]
    private Button _inputButton;
    [SerializeField] 
    private Text _inputButtonText;
    [SerializeField]
    private Text _ingredientAmountText;

    [SerializeField]
    private int _ingredientAmount;
    private int _slotId;

    private Dictionary<int,int> _ingredientCountCheckDict = new Dictionary<int,int>();
    private List<bool> _ingredientCountFullList = new List<bool>(14);
    public PotionBrewer Brewer
    {
        get
        {
            return _potionBrewer;
        }
        set
        {
            _potionBrewer = value;
        }
    }

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
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = GameManager.GM.MainCamera;
        _inputButton.onClick.AddListener(InputIngredient);
        _ingredientAmountText.text = _ingredientAmount.ToString();
        _ingredientCountCheckDict = new Dictionary<int, int>();
        _ingredientCountFullList = new List<bool> { false };
    }

    public void InitializeIngredient()
    {
        _ingredientAmount = 78*_potionBrewer._currentQuest.QuestGrade;
        _ingredientAmountText.text = _ingredientAmount.ToString();
        _ingredientAmountText.color = Color.black;
        for (int i = 1; i <= 13; ++i) _ingredientCountCheckDict[i] = 0;
        _ingredientCountFullList = Enumerable.Repeat(false, 14).ToList();

    }

    private int GetRandomAmount()
    {
        int amount = Random.Range(1,13);

        if (IsFullCount()) return 0;
        while (_ingredientCountCheckDict[amount] >= _potionBrewer._currentQuest.QuestGrade)
        {
            amount = Random.Range(1, 13);
        }

        return amount;
    }

    private bool IsFullCount()
    {
        bool ret = true;
        for(int i = 1; i<= 13; ++i)
        { 
            
            if (_ingredientCountFullList[i] == false) ret = false;
        }
        return ret;
    }

    private void InputIngredient()
    {
        //���� ������ ���� ���� �̰�
        int amount = GetRandomAmount();
        _potionBrewer.InsertIngredient(SlotId, amount);
        //���������Ƿ� ���� ���� üũ
        _ingredientCountCheckDict[amount]++;

        if (_ingredientCountCheckDict[amount] >= _potionBrewer._currentQuest.QuestGrade)
            _ingredientCountFullList[amount] = true;

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
    
    private void IngredientSupply()
    {
        Debug.Log("��Ḧ �����մϴ�!");
        //��带 �Ҹ���
        IngredientAmount = 78;
        _inputButtonText.text = "����";
        _ingredientAmountText.color = Color.white;
        _ingredientCountCheckDict.Clear();
        _ingredientCountFullList.Clear();
        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

}
