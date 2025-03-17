using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class IngredientSlot : MonoBehaviour
    ,IPointerEnterHandler
    ,IPointerExitHandler
{
    public const int REFILL_GOLD = 10;
    public const int MAX_NUMBER = 10;
    public const int SUM_NUMBER = ((MAX_NUMBER - 1) * MAX_NUMBER) / 2;

    [Header("Component")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ParticleSystem _particle;

    [Header("UI")]
    [SerializeField] private Button _inputButton;
    [SerializeField] private Text _inputButtonText;
    [SerializeField] private Button _inputInfoButton;
    [SerializeField] private Image _ingredientImage;
    [SerializeField] private Image[] _inputInfoImages;
    [SerializeField] private GameObject _inputInfoUIInstance;
    [SerializeField] private GameObject _ingredientInfoUIPrefab;
    private GameObject _ingredientInfoUIInstance;

    [SerializeField] private int _ingredientAmount;
    private int _ingredientId;

    private PotionBrewer _brewer;
    private int _slotId;
    
    private Dictionary<int,int> _ingredientCountDict = new Dictionary<int,int>(); //현재까지 투입된 수량
    private List<bool> _isIngredientFullList = new List<bool>(14);//각 숫자가 최대 투입 가능 여부 (자연수)

    //Getter Setter
    public int IngredientAmount { get=>_ingredientAmount; set => _ingredientAmount = value; }
    public int SlotId { get => _slotId; set => _slotId = value; }
    
    void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _brewer = GameManager.GM.Brewer;

        _inputButton.onClick.AddListener(InputIngredient);
        _inputInfoButton.onClick.AddListener(ToggleInputInfoUI);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 offset = new Vector3(-2,1, -1);
        _ingredientInfoUIInstance = Instantiate<GameObject>(_ingredientInfoUIPrefab,
            _ingredientImage.transform.position + offset, _ingredientImage.transform.rotation);

        Text uiText = _ingredientInfoUIInstance.GetComponentInChildren<Text>();
        var names = _brewer?.CurrentQuest?.PInfo.ingredientName;

        if (uiText != null && names != null)
            uiText.text = names[SlotId];
        else
            Debug.LogError("Null Error");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(_ingredientInfoUIInstance != null)
            Destroy(_ingredientInfoUIInstance);
    }

    public void InitializeSlot()
    {
        //투입량 수정 필요
        IngredientAmount = SUM_NUMBER * _brewer.CurrentQuest.QuestGrade;

        //DB추가전 임시 사용
        _ingredientId = 4001;
        _ingredientImage.sprite = Resources.Load<Sprite>(ReadJson._dictMaterial[_ingredientId].materialImage);

        ParticleSystemRenderer renderer = _particle.GetComponent<ParticleSystemRenderer>();
        Material particleMaterial = new Material(Shader.Find("Unlit/Transparent"))
        {
            mainTexture = _ingredientImage.sprite.texture
        };
        renderer.material = particleMaterial;

        _ingredientCountDict.Clear();
        for (int i = 1; i <= MAX_NUMBER; ++i)
        {
            _ingredientCountDict[i] = 0;
            _inputInfoImages[i].color = Color.white;
        }
        _isIngredientFullList = Enumerable.Repeat(false, MAX_NUMBER+1).ToList();
    }

    private void InputIngredient()
    {
        int amount = GetRandomAmount();

        if(amount == 0)
        {
            Debug.LogWarning("투입 가능한 재료 수량이 없습니다.");
            return;
        }

        _brewer.InsertIngredient(SlotId, amount);
        _ingredientCountDict[amount]++;

        if (_ingredientCountDict[amount] >= _brewer.CurrentQuest.QuestGrade)
        {
            _isIngredientFullList[amount] = true;
            _inputInfoImages[amount].color = new Color32(120, 120, 120, 255);
        }

        //양조기강화 버프 체크
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        IngredientAmount -= amount;

        if (IngredientAmount <= 0)
        {
            _inputButtonText.text = "재료 수급";
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);
        }
        
        _particle.Play();
    }

    private int GetRandomAmount()
    {
        int amount = Random.Range(1, MAX_NUMBER);

        if (IsFullCount()) return 0;
        while (_ingredientCountDict[amount] >= _brewer.CurrentQuest.QuestGrade)
        {
            amount = Random.Range(1, MAX_NUMBER);
        }

        //홀짝버프 확인
        GameManager.GM.BM.CheckBuff(BuffType.EvenNumber,ref amount);
        GameManager.GM.BM.CheckBuff(BuffType.OddNumber,ref amount);

        return amount;
    }

    private bool IsFullCount()
    {
        bool ret = true;

        for (int i = 1; i <= MAX_NUMBER; ++i)
            if (!_isIngredientFullList[i]) ret = false;

        return ret;
    }

    private void IngredientSupply()
    {
        Debug.Log("재료를 수급합니다!");

        GameManager.GM.PlayInformation.ConsumeGold(REFILL_GOLD);
        IngredientAmount = SUM_NUMBER;

        _inputButtonText.text = "투입";

        _ingredientCountDict.Clear();
        _isIngredientFullList.Clear();

        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

    private void ToggleInputInfoUI()
    {
        if(_inputInfoUIInstance != null)
        _inputInfoUIInstance.SetActive(!_inputInfoUIInstance.activeSelf);
    }
    public void DisableInputButton() => _inputButton.enabled = false;
    public void EnableInputButton() => _inputButton.enabled = true;
}
