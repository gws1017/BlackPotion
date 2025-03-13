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
    //UI
    //���� ��ư
    [SerializeField]
    private Button _inputButton;
    [SerializeField] 
    private Text _inputButtonText;
    [SerializeField]
    private Button _inputInfoButton;
    [SerializeField]
    private GameObject _inputInfoUIInstance;

    //���� ������ ��
    [SerializeField]
    private int _ingredientAmount;

    [SerializeField]
    private Image _ingredientImage;
    [SerializeField]
    private Image[] _inputInfoImages;
    private int _ingredientId;

    [SerializeField]
    private GameObject _ingridientInfoUIPrefab;
    private GameObject _ingridientInfoUIInstance;
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private ParticleSystem _particle;

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
        _inputInfoButton.onClick.AddListener(ToggleInputInfoUI);

        _ingredientCountCheckDict = new Dictionary<int, int>();
        _ingredientCountFullList = new List<bool> { false };
    }

    //���콺 ������ �Ƿڸ� �����ϱ����� �߰���
    
    public void OnPointerEnter(PointerEventData eventData)
    {

        Vector3 offset = new Vector3(-2,1, -1);
        _ingridientInfoUIInstance = Instantiate<GameObject>(_ingridientInfoUIPrefab, _ingredientImage.transform.position + offset, _ingredientImage.transform.rotation);

        _ingridientInfoUIInstance.GetComponentInChildren<Text>().text
            = _brewer._currentQuest.PInfo.ingredientName[SlotId];
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_ingridientInfoUIInstance);
        Debug.Log("mouse exit");

    }

    //���� �����ϴ� ���� �Ƿڿ� �°� �ʱ�ȭ�Ѵ�.
    public void InitializeSlot()
    {
        IngredientAmount = SUM_NUMBER * _brewer._currentQuest.QuestGrade;

        //_ingredientId = _brewer._currentQuest.PInfo.materialIdList[SlotId];
        _ingredientImage.sprite = Resources.Load<Sprite>(ReadJson._dictMaterial[4001].materialImage);

        ParticleSystemRenderer renderer = _particle.GetComponent<ParticleSystemRenderer>();
        Material particleMaterial = new Material(Shader.Find("Unlit/Transparent"));
        particleMaterial.mainTexture = _ingredientImage.sprite.texture;
        renderer.material = particleMaterial;

        for (int i = 1; i <= MAX_NUMBER; ++i)
        {
            _ingredientCountCheckDict[i] = 0;
            _inputInfoImages[i].color = Color.white;
        }
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
        {
            _ingredientCountFullList[amount] = true;
            Color usedColor = new Color32(120, 120, 120,255);
            _inputInfoImages[amount].color = usedColor;
        }

        //�����Ⱝȭ ���� üũ
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        IngredientAmount -= amount;

        //��� �� ������� ���
        if (IngredientAmount <= 0)
        {
            _inputButtonText.text = "��� ����";
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);

        }
        
        _particle.Play();
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
        GameManager.GM.BM.CheckBuff(BuffType.EvenNumber,ref amount);
        GameManager.GM.BM.CheckBuff(BuffType.OddNumber,ref amount);
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
        GameManager.GM.PlayInformation.ConsumeGold(REFILL_GOLD);
        IngredientAmount = SUM_NUMBER;

        _inputButtonText.text = "����";

        _ingredientCountCheckDict.Clear();
        _ingredientCountFullList.Clear();

        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
    }

    private void ToggleInputInfoUI()
    {
        _inputInfoUIInstance.SetActive(!_inputInfoUIInstance.activeSelf);
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
