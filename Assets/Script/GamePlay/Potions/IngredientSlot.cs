using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
    ,IPointerEnterHandler
    ,IPointerExitHandler
{
    //UI
    //투입 버튼
    [SerializeField]
    private Button _inputButton;
    [SerializeField] 
    private Text _inputButtonText;

    //투입 가능한 량
    [SerializeField]
    private int _ingredientAmount;

    [SerializeField]
    private Image _ingredientImage;
    private int _ingredientId;

    [SerializeField]
    private GameObject _infoUIPrefab;
    private GameObject _infoUIInstance;
    [SerializeField]
    private Canvas _canvas;

    public const int REFILL_GOLD = 10;

    public const int MAX_NUMBER = 10;

    public const int SUM_NUMBER = ((MAX_NUMBER - 1) * MAX_NUMBER) / 2;

    //PotionBrwer 레퍼런스
    private PotionBrewer _brewer;
    //현재 슬롯의 ID
    private int _slotId;

    //현재 까지 투입된 수량을 체크하는 Dictionary
    private Dictionary<int,int> _ingredientCountCheckDict = new Dictionary<int,int>();
    //1~13까지의 숫자 중 더 뽑을 수 있는 수인지 확인하기 위한 List
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
        }
    }

    public int SlotId
    {
        get { return _slotId; }
        set { _slotId = value; }
    }
    
    void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        GetComponentInChildren<Canvas>().worldCamera = GameManager.GM.MainCamera;
        _brewer = GameManager.GM.Brewer;

        _inputButton.onClick.AddListener(InputIngredient);

        _ingredientCountCheckDict = new Dictionary<int, int>();
        _ingredientCountFullList = new List<bool> { false };
    }

    //마우스 오버된 의뢰를 강조하기위해 추가함
    
    public void OnPointerEnter(PointerEventData eventData)
    {

        Vector3 offset = new Vector3(-2,1, -1);
        _infoUIInstance = Instantiate<GameObject>(_infoUIPrefab, _ingredientImage.transform.position + offset, _ingredientImage.transform.rotation);

        _infoUIInstance.GetComponentInChildren<Text>().text
            = _brewer._currentQuest.PInfo.ingredientName[SlotId];
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_infoUIInstance);
        Debug.Log("mouse exit");

    }

    //현재 제조하는 포션 의뢰에 맞게 초기화한다.
    public void InitializeSlot()
    {
        IngredientAmount = SUM_NUMBER * _brewer._currentQuest.QuestGrade;

        //_ingredientId = _brewer._currentQuest.PInfo.materialIdList[SlotId];
        _ingredientImage.sprite = Resources.Load<Sprite>(ReadJson._dictMaterial[4001].materialImage);

        for (int i = 1; i <= MAX_NUMBER; ++i) _ingredientCountCheckDict[i] = 0;
        _ingredientCountFullList = Enumerable.Repeat(false, MAX_NUMBER+1).ToList();
    }

    //재료 투임 함수
    private void InputIngredient()
    {
        //아직 나오지 않은 수를 뽑고
        int amount = GetRandomAmount();
        _brewer.InsertIngredient(SlotId, amount);
        //투입했으므로 나온 수로 체크
        _ingredientCountCheckDict[amount]++;

        if (_ingredientCountCheckDict[amount] >= _brewer._currentQuest.QuestGrade)
            _ingredientCountFullList[amount] = true;

        //양조기강화 버프 체크
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        IngredientAmount -= amount;

        //재료 다 사용했을 경우
        if (IngredientAmount <= 0)
        {
            _inputButtonText.text = "재료 수급";
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);

        }
    }

    //투입될 무작위 수량을 뽑는 함수
    private int GetRandomAmount()
    {
        int amount = Random.Range(1, MAX_NUMBER);

        if (IsFullCount()) return 0;
        //1~13의 숫자는 같은 숫자를 의뢰 등급만큼 뽑을 수 있다.
        while (_ingredientCountCheckDict[amount] >= _brewer._currentQuest.QuestGrade)
        {
            amount = Random.Range(1, MAX_NUMBER);
        }
        //홀짝버프 확인
        GameManager.GM.BM.CheckBuff(BuffType.EvenOddNumber,ref amount);
        return amount;
    }

    //더이상 뽑을 수있 는 수가 없는지 확인하는 함수
    private bool IsFullCount()
    {
        bool ret = true;

        for (int i = 1; i <= MAX_NUMBER; ++i)
            if (_ingredientCountFullList[i] == false) ret = false;

        return ret;
    }

    //재료 리필 함수
    private void IngredientSupply()
    {
        Debug.Log("재료를 수급합니다!");
        //골드를 소모함
        GameManager.GM.PlayInformation.ConsumeGold(REFILL_GOLD);
        IngredientAmount = SUM_NUMBER;

        _inputButtonText.text = "투입";

        _ingredientCountCheckDict.Clear();
        _ingredientCountFullList.Clear();

        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

    //투입 기능 ON / OFF
    public void DisableInputButton()
    {
        _inputButton.enabled = false;
    }
    public void EnableInputButton()
    {
        _inputButton.enabled = true;
    }
}
