using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftResult : MonoBehaviour
{

    private const float MAX_QUALITY_VALUE = 100f;

    [Header("Component")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Animator _animatorLeft;
    [SerializeField] private Animator _animatorRight;
    [SerializeField] private GameObject _rewardUIInstance;
    [SerializeField] private GameObject _resultCheckUIInstance;
    private PotionBrewer _brewer;

    //UI
    [Header("Reward UI")]
    [SerializeField] private GameObject _rewardButtons;
    [SerializeField] private Text _questResultText;
    [SerializeField] private Text _questFailText;
    [SerializeField] private Text _moneyValueText;
    [SerializeField] private Text _recipeNameText;
    [SerializeField] private Text _selectText;
    [SerializeField] private Text _potionQualityRewardText;
    [SerializeField] private Text _resultText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _moneyButton;
    [SerializeField] private Button _recipeButton;
    [SerializeField] private Image _recipeImage;
    [SerializeField] private Outline _moneyOutline;
    [SerializeField] private Outline _recipeOutline;

    [Header("Result Check UI")]
    [SerializeField] private Image _potionImage;
    [SerializeField] private Text _potionName;
    [SerializeField] private Text _potionGrade;
    [SerializeField] private Text _potionQualityResultCheckText;
    [SerializeField] private Text _potionMinQuality;
    [SerializeField] private Text _potionMaxQuality;
    [SerializeField] private Text _retryGoldText;
    [SerializeField] private Slider _potionQualityProgressBar;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private GameObject _sliderObject;
    private string _potionCraftGrade;

    //내부 변수
    private int _selectReward;
    private int _rewardRecipeId;
    private float _rewardMultiplier = 1;
    private bool _isPotionCraftSuccessful;
    private bool _playResultSound = false;

    public int PotionQuality { set => _potionQualityRewardText.text = value.ToString(); }
    public bool IsPotionCraftSuccessful  {get => _isPotionCraftSuccessful; set => _isPotionCraftSuccessful = value;  }
    public PotionBrewer Brewer => _brewer ? _brewer : (_brewer = GameManager.GM.Brewer);

    void Start()
    {
        var gm = GameManager.GM;
        _canvas.worldCamera = gm.MainCamera;
        _brewer = gm.Brewer;
        _retryGoldText.text = $"{Constants.RETRY_GOLD} 골드";
        InitializeRewardUI();
    }

    private void InitializeRewardUI()
    {
        _selectButton.onClick.RemoveAllListeners();
        _moneyButton.onClick.RemoveAllListeners();
        _recipeButton.onClick.RemoveAllListeners();

        _selectButton.onClick.AddListener(SelectReward);
        _moneyButton.onClick.AddListener(SelectMoney);
        _recipeButton.onClick.AddListener(SelectRecipe);

    }

    private void InitializeResultCheckUI()
    {
        _restartButton.onClick.RemoveAllListeners();
        _nextButton.onClick.RemoveAllListeners();

        _nextButton.onClick.AddListener(ShowCraftResultUI);
        _restartButton.onClick.AddListener(RestartCraft);

        if (Brewer.CurrentQuest.IsRestart) _restartButton.interactable = false;
        _potionQualityProgressBar.maxValue = MAX_QUALITY_VALUE;
    }

    public void ShowCraftResultUI()
    {

        InitializeRewardUI();
        OffHighlight();

        SoundManager._Instance.PlaySFXAtObject(Brewer.gameObject, SFXType.Craft);
        _restartButton.interactable = true;

        _resultCheckUIInstance.SetActive(false);
        _rewardUIInstance.SetActive(true);

        Brewer.CurrentCraftState = PotionBrewer.CraftState.Success;
    }

    public void UpdateCraftResultUI()
    {
        _questResultText.text = "";
        _questFailText.enabled = false;

        _animatorLeft.SetTrigger("PlayOnce");
        _animatorRight.SetTrigger("PlayOnce");



        if (IsPotionCraftSuccessful)
        {
            ProcessSuccessResult();
        }
        else
        {
            ProcessFailureResult();
        }
    }

    

    private void ProcessSuccessResult()
    {
        var quest = Brewer.CurrentQuest;
        PlayInfo playInfo = GameManager.GM.PlayInformation;

        int rewardRecipeGrade = quest.QuestRewardRecipeGrade;

        //A랭크인경우 대성공 보너스
        if (_potionCraftGrade == Constants.PotionCraftGrade.RANK_A)
        {
            _rewardMultiplier = Constants.CRITICAL_SUCCESS;
            if (Constants.MAX_RECIPE_GRADE > rewardRecipeGrade)
                rewardRecipeGrade++;
        }
        else _rewardMultiplier = 1;

        List<int> availableRecipes = new List<int>();
        foreach (var RecipeData in ReadJson._dictPotion.Values)
        {
            if (RecipeData.potionGrade == rewardRecipeGrade
                && !playInfo.HasRecipe(RecipeData.potionId))
            {
                availableRecipes.Add(RecipeData.potionId);
            }
        }

        Constants.SetRecipeIcon(_recipeImage, rewardRecipeGrade);

        _rewardButtons.SetActive(true);
        _moneyValueText.text = $"{((int)(quest.QuestRewardMoney * _rewardMultiplier))} 골드";

        if (availableRecipes.Count == 0)
        {
            _recipeButton.enabled = false;
            _recipeNameText.text = "매진";
        }
        else
        {
            int rndId = UnityEngine.Random.Range(0, availableRecipes.Count);
            _rewardRecipeId = availableRecipes[rndId];

            var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
            _recipeNameText.text = RecipeData.potionName + " 레시피";
        }

        _selectText.text = "선택";
        playInfo.QuestSuccessCount++;
    }
    private void ProcessFailureResult()
    {
        var quest = Brewer.CurrentQuest;
        PlayInfo playInfo = GameManager.GM.PlayInformation;

        _rewardButtons.SetActive(false);
        _selectText.text = "다음";

        playInfo.ConsumeGold((int)(quest.QuestRewardMoney * Constants.QUEST_PENALTY_RATIO));
    }

    //제조버튼 클릭시 호출됨
    public void ShowResultCheckUI()
    {
        InitializeResultCheckUI();

        SoundManager._Instance.PlayClickSound();

        GameManager.GM.BM.DisableBuffInventory();
        gameObject.SetActive(true);
        _resultCheckUIInstance.SetActive(true);
        _rewardUIInstance.SetActive(false);

        var questInfo = Brewer.CurrentQuest;

        _potionImage.sprite = questInfo.PotionImage;
        _potionName.text = questInfo.PotionName;
        _potionQualityResultCheckText.text = Brewer.CurrentPotionQuality.ToString();
        _potionMinQuality.text = questInfo.QInfo.minCapacity.ToString();
        _potionMaxQuality.text = questInfo.QInfo.maxCapacity.ToString();

        if (Brewer._activePlusPowder)
        {
            string plusValue = $" + <color=red>{ReadJson._dictBuff[(int)BuffType.PlusPowder].buffState}</color>";
            
            _potionQualityResultCheckText.text += plusValue;
            _potionQualityRewardText.text = _potionQualityResultCheckText.text;
        }
        int currentQuality = Brewer.CurrentPotionQuality;
        currentQuality -= questInfo.QInfo.minCapacity;

        float qualityPercent = (float)(currentQuality) / (float)(questInfo.QInfo.maxCapacity - questInfo.QInfo.minCapacity) * 100.0f;
        _potionCraftGrade = Constants.CheckPotionCraftGrade(qualityPercent);
        _potionGrade.text = _potionCraftGrade;

        if (currentQuality < 0 || Brewer.IsCraftSuccessful() == false)
        {
            _potionGrade.color = Color.red;
            _sliderObject.SetActive(false);
            _potionCraftGrade = "F";
            _potionGrade.text = _potionCraftGrade;
        }
        else
        {
            _potionGrade.color = Color.black;
            _potionQualityProgressBar.value = qualityPercent;
            _sliderObject.SetActive(true);
        }
    }


    public void ShowResultText()
    {
        if (Brewer.CurrentCraftState == PotionBrewer.CraftState.None) 
            return;

        _potionQualityRewardText.text = Brewer.CurrentPotionQuality.ToString();
        _questResultText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 432);


        if (IsPotionCraftSuccessful)
        {
            _questResultText.text = (_potionCraftGrade == Constants.PotionCraftGrade.RANK_A) ?
            "의뢰 대성공" :"의뢰 성공";
            _resultText.text = "성공";
            _questFailText.enabled = false;
            if (_playResultSound == false)
                SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Succ);
        }
        else
        {
            //_questResultText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,184);
            _questResultText.text = "의뢰 실패";
            _questFailText.enabled = true;
            _questFailText.text = $"위약금 발생\n{Brewer.CurrentQuest.QuestRewardMoney * -0.1f}G";
            _resultText.text = "실패";
            if (_playResultSound == false)
                SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Fail);
        }
        _playResultSound = true;

    }

    public void SelectReward()
    {
        if (IsPotionCraftSuccessful)
        {
            var quest = Brewer.CurrentQuest;
            var playInfo = GameManager.GM.PlayInformation;

            if (_selectReward == 1)
            {
                playInfo.IncrementGold((int)(quest.QuestRewardMoney * _rewardMultiplier));
            }
            else if (_selectReward == 2)
            {
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                playInfo.AddRecipe(_rewardRecipeId);
                Debug.Log(RecipeData.potionName.ToString() + " 레시피 해금되었습니다.");
            }
            else 
                return;
        }
        _playResultSound = false;

        SoundManager._Instance.PlayClickSound();


        //버프 상점 오픈
        Brewer._activePlusPowder = false;
        Brewer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        GameManager.GM.BM.EnableBuffInventory();
        _questResultText.text = "의뢰 결과";
        _resultText.text = "결과";
        gameObject.SetActive(false);
    }

    public void RestartCraft()
    {
        if (Brewer.CurrentQuest.IsRestart) 
            return;
        if (GameManager.GM.PlayInformation.CurrentGold < Constants.RETRY_GOLD)
        {
            GameManager.GM.CreateInfoUI("골드가 부족합니다.",
                GameManager.GM.MainCamera.GetComponentInChildren<Canvas>().transform, new Vector3(0, -200, 0), Vector3.one * 128);
            return;
        }
        SoundManager._Instance.PlayClickSound();

        Brewer.CurrentCraftState = PotionBrewer.CraftState.Retry;
        GameManager.GM.BM.EnableBuffInventory();
        gameObject.SetActive(false);
    }

    public void OffHighlight()
    {
        _selectReward = 0;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectMoney()
    {
        _selectReward = 1;
        
        _moneyOutline.effectColor = Constants.REWARD_SELECT_HILIGHT;
        _recipeOutline.effectColor = Color.white;

        SoundManager._Instance.PlaySFX2D(SFXType.Coin);
    }

    public void SelectRecipe()
    {
        _selectReward = 2;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Constants.REWARD_SELECT_HILIGHT;
        SoundManager._Instance.PlaySFX2D(SFXType.Recipe1);

    }
}
