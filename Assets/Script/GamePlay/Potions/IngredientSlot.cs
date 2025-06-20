using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class IngredientSlot : MonoBehaviour
{


    [Header("Component")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ParticleSystem _particle;

    [Header("UI")]
    [SerializeField] private Button _inputButton;
    [SerializeField] private Button _inputInfoButton;
    [SerializeField] private Text _inputButtonText;
    [SerializeField] private Text _refillGoldText;
    [SerializeField] private Image _ingredientImage;
    [SerializeField] private Image[] _inputInfoImages;
    [SerializeField] private GameObject _inputInfoUIInstance;
    [SerializeField] private GameObject _ingredientInfoUIPrefab;
    [SerializeField] private GameObject _refillGoldObject;

    private GameObject _ingredientInfoUIInstance;

    [SerializeField] private int _ingredientAmount;
    private int _ingredientId;

    private PotionBrewer _brewer;
    private int _slotId;
    private int _questGrade;

    private Dictionary<int, bool> _ingredientCountDict = new Dictionary<int, bool>(); //현재까지 투입된 수량

    //Getter Setter
    public int IngredientAmount { get => _ingredientAmount; set => _ingredientAmount = value; }
    public int SlotId { get => _slotId; set => _slotId = value; }

    void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _brewer = GameManager.GM.Brewer;

        _inputButton.onClick.AddListener(InputIngredient);
        _refillGoldText.text = $"{Constants.INGRIDIENT_REFILL_GOLD} 골드";
    }

    public void ShowIngredientImage()
    {
        Vector3 offset = new Vector3(-2, 1, -1);
        _ingredientInfoUIInstance = Instantiate<GameObject>(_ingredientInfoUIPrefab,
            _ingredientImage.transform.position + offset, _ingredientImage.transform.rotation);

        Text uiText = _ingredientInfoUIInstance.GetComponentInChildren<Text>();


        if (uiText != null && _ingredientId != 0)
        {
            uiText.text = ReadJson._dictMaterial[_ingredientId].materialName;
        }
        else
            Debug.LogError("Null Error");
    }
    public void HideIngredientImage()
    {
        if (_ingredientInfoUIInstance != null)
            Destroy(_ingredientInfoUIInstance);
    }

    public void InitializeSlot()
    {
        _questGrade = (int)_brewer.CurrentQuest.QuestGrade + 1;
        IngredientAmount = Constants.INGRIDIENT_SUM_NUMBER;

        int? ingredientId = _brewer?.CurrentQuest?.PInfo.ingredientIdList[SlotId];
        if (ingredientId != null && ingredientId != 0)
        {
            _ingredientId = ingredientId.Value;
            _ingredientImage.sprite = Resources.Load<Sprite>(ReadJson._dictMaterial[_ingredientId].materialImage);


            ParticleSystemRenderer renderer = _particle.GetComponent<ParticleSystemRenderer>();
            if (renderer.material != null)
                renderer.material.mainTexture = _ingredientImage.sprite.texture;
            else
                Debug.Log("Shader Not FOUND!!!");


        }

        _refillGoldObject.SetActive(false);

        ResetIngredientUsage();

    }

    private void ResetIngredientUsage()
    {
        _inputButtonText.text = "투입";
        _ingredientCountDict.Clear();
        for (int i = 1; i <= Constants.INGRIDIENT_MAX_NUMBER; ++i)
        {
            _ingredientCountDict[i] = false;
            _inputInfoImages[i].color = Color.white;
        }
        IngredientAmount = Constants.INGRIDIENT_SUM_NUMBER;
    }

    private void InputIngredient()
    {
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Add);
        int amount = GetRandomAmount();

        if (amount == 0)
        {
            if (_questGrade > 1)
            {
                _questGrade--;
                ResetIngredientUsage();
                amount = GetRandomAmount();
            }
            else
            {
                Debug.LogWarning("투입 가능한 재료 수량이 없습니다.");
                return;
            }
        }
        Debug.Log(amount);
        _brewer.InsertIngredient(SlotId, amount);
        _ingredientCountDict[amount] = true;
        _inputInfoImages[amount].color = new Color32(120, 120, 120, 255);


        //양조기강화 버프 체크
        GameManager.GM.BM.CheckBuff(BuffType.UpgradeBrew, ref amount);

        IngredientAmount -= amount;

        if (IngredientAmount <= 0 && _questGrade <= 1)
        {
            _inputButtonText.text = "재료 수급";
            _refillGoldObject.SetActive(true);
            _inputButton.onClick.RemoveAllListeners();
            _inputButton.onClick.AddListener(IngredientSupply);
        }

        _particle.Emit(amount);
    }


    private int GetRandomAmount()
    {
        if (IsFullCount()) return 0;

        int amount = Random.Range(1, Constants.INGRIDIENT_MAX_NUMBER + 1);
        while (_ingredientCountDict[amount])
        {
            amount = Random.Range(1, Constants.INGRIDIENT_MAX_NUMBER + 1);
        }

        //홀짝버프 확인
        GameManager.GM.BM.CheckBuff(BuffType.EvenNumber, ref amount);
        GameManager.GM.BM.CheckBuff(BuffType.OddNumber, ref amount);

        return amount;
    }

    private bool IsFullCount()
    {
        bool ret = true;

        foreach (bool used in _ingredientCountDict.Values)
        {
            if (!used)
                return false;
        }

        return ret;
    }

    private void IngredientSupply()
    {
        Debug.Log("재료를 수급합니다!");

        if(GameManager.GM.PlayInformation.CurrentGold < Constants.INGRIDIENT_REFILL_GOLD)
        {
            GameManager.GM.CreateInfoUI("골드가 부족합니다.", GameManager.GM.MainCamera.GetComponentInChildren<Canvas>().transform,
   new Vector3(0, -200, 0), Vector3.one * 128);
            return;
        }

        GameManager.GM.PlayInformation.ConsumeGold(Constants.INGRIDIENT_REFILL_GOLD);
        IngredientAmount = Constants.INGRIDIENT_SUM_NUMBER;

        _inputButtonText.text = "투입";

        _ingredientCountDict.Clear();

        _inputButton.onClick.RemoveAllListeners();
        _inputButton.onClick.AddListener(InputIngredient);
        ResetIngredientUsage();
    }

    public void ToggleInputInfoUI()
    {
        if (_inputInfoUIInstance != null)
            _inputInfoUIInstance.SetActive(!_inputInfoUIInstance.activeSelf);
    }
    public void DisableInputButton() => _inputButton.enabled = false;
    public void EnableInputButton() => _inputButton.enabled = true;
}
