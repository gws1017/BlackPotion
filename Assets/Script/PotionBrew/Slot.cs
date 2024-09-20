using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private PotionBrewer _potionBrwer;

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

    private Dictionary<int,bool> _ingredientCountCheckDict = new Dictionary<int,bool>();
    public PotionBrewer Brewer
    {
        get
        {
            return _potionBrwer;
        }
        set
        {
            _potionBrwer = value;
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
        GetComponentInChildren<Canvas>().worldCamera = GameManager.MainCamera;
        _inputButton.onClick.AddListener(InputIngredient);
        _ingredientAmountText.text = _ingredientAmount.ToString();
    }

    public void InitializeIngredient()
    {
        _ingredientAmount = 78*_potionBrwer._currentQuest.QuestGrade;
        _ingredientAmountText.text = _ingredientAmount.ToString();
        _ingredientAmountText.color = Color.black;
    }

    private int GetRandomAmount()
    {
        int amount = Random.Range(1,13);
        if (_ingredientCountCheckDict.Count >= 12) return 0;
        while (_ingredientCountCheckDict.ContainsKey(amount) == true)
        {
            amount = Random.Range(1, 13);
        }

        return amount;
    }
    private void InputIngredient()
    {
        //아직 나오지 않은 수를 뽑고
        int amount = GetRandomAmount();
        _potionBrwer.InsertIngredient(SlotId, amount);
        //투입했으므로 나온 수로 체크
        _ingredientCountCheckDict.Add(amount, true);
        IngredientAmount -= amount;

        //재료 다 사용했을 경우
        if (IngredientAmount <= 0)
        {
            _ingredientAmountText.color = Color.red;
            _inputButtonText.text = "재료 수급";
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);

        }
    }
    
    private void IngredientSupply()
    {
        Debug.Log("재료를 수급합니다!");
        //골드를 소모함
        IngredientAmount = 78;
        _inputButtonText.text = "투입";
        _ingredientAmountText.color = Color.white;
        _ingredientCountCheckDict.Clear();
        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

}
