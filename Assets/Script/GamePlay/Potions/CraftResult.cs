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
    [SerializeField] private Text _moneyValueText;
    [SerializeField] private Text _recipeNameText;
    [SerializeField] private Text _selectText;
    [SerializeField] private Text _potionQualityValueText;
    [SerializeField] private Text _resultText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _moneyButton;
    [SerializeField] private Button _recipeButton;
    [SerializeField] private Outline _moneyOutline;
    [SerializeField] private Outline _recipeOutline;

    [Header("Result Check UI")]
    [SerializeField] private Image _potionImage;
    [SerializeField] private Text _potionName;
    [SerializeField] private Text _potionGrade;
    [SerializeField] private Text _potionQuality;
    [SerializeField] private Text _potionMinQuality;
    [SerializeField] private Text _potionMaxQuality;
    [SerializeField] private Slider _potionQualityProgressBar;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextButton;
    private string _potionCraftGrade;

    //내부 변수
    private int _selectReward;
    private int _rewardRecipeId;
    private float _rewardMultiplier = 1;
    private bool _isPotionCraftSuccessful;

    public int PotionQuality { set => _potionQualityValueText.text = value.ToString(); }
    public bool IsPotionCraftSuccessful  {get => _isPotionCraftSuccessful; set => _isPotionCraftSuccessful = value;  }
    public PotionBrewer Brewer => _brewer ? _brewer : (_brewer = GameManager.GM.Brewer);

    void Start()
    {
        var gm = GameManager.GM;
        _canvas.worldCamera = gm.MainCamera;
        _brewer = gm.Brewer;
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

        _restartButton.interactable = true;

        _resultCheckUIInstance.SetActive(false);
        _rewardUIInstance.SetActive(true);

        Brewer.CurrentCraftState = PotionBrewer.CraftState.Success;
    }

    public void UpdateCraftResultUI()
    {
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

        if (_potionCraftGrade == PlayInfo.PotionCraftGrade.RANK_1)
        {
            _rewardMultiplier = PlayInfo.CRITICAL_SUCCESS;
            if (PlayInfo.MAX_RECIPE_GRADE > rewardRecipeGrade)
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

        playInfo.ConsumeGold((int)(quest.QuestRewardMoney * PlayInfo.QUEST_PENALTY_RATIO));
    }

    public void ShowResultCheckUI()
    {
        InitializeResultCheckUI();

        gameObject.SetActive(true);
        _resultCheckUIInstance.SetActive(true);
        _rewardUIInstance.SetActive(false);

        var questInfo = Brewer.CurrentQuest;

        _potionImage.sprite = questInfo.PotionImage;
        _potionName.text = questInfo.PotionName;
        _potionQuality.text = Brewer.CurrentPotionQuality.ToString();
        _potionMinQuality.text = questInfo.QInfo.minCapacity.ToString();
        _potionMaxQuality.text = questInfo.QInfo.maxCapacity.ToString();

        if (Brewer._activePlusPowder)
        {
            string plusValue = $" + <color=red>{ReadJson._dictBuff[(int)BuffType.PlusPowder].buffState}</color>";
            
            _potionQuality.text += plusValue;
            _potionQualityValueText.text = _potionQuality.text;
        }
        int currentQuality = Brewer.CurrentPotionQuality;
        if (currentQuality < questInfo.QInfo.minCapacity) 
            currentQuality = 0;

        float qualityPercent = (float)(currentQuality) / (float)(questInfo.QInfo.maxCapacity) * 100.0f;
        _potionCraftGrade = PlayInfo.CheckPotionCraftGrade(qualityPercent);
        _potionGrade.text = _potionCraftGrade;
        _potionQualityProgressBar.value = qualityPercent;
    }


    public void ShowResultText()
    {
        if (Brewer.CurrentCraftState == PotionBrewer.CraftState.None) 
            return;

        if (IsPotionCraftSuccessful)
        {
            _questResultText.text = (_potionCraftGrade == PlayInfo.PotionCraftGrade.RANK_1) ?
            "의뢰 대성공" :"의뢰 성공";
            _resultText.text = "성공";
        }
        else
        {
            _questResultText.text = "의뢰 실패\n위약금 발생";
            _resultText.text = "실패";
        }
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

        //버프 상점 오픈
        Brewer._activePlusPowder = false;
        Brewer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        _questResultText.text = "의뢰 결과";
        _resultText.text = "결과";
        gameObject.SetActive(false);
    }

    public void RestartCraft()
    {
        if (Brewer.CurrentQuest.IsRestart) 
            return;

        Brewer.CurrentCraftState = PotionBrewer.CraftState.Retry;
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
        _moneyOutline.effectColor = Color.black;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectRecipe()
    {
        _selectReward = 2;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.black;
    }
}
