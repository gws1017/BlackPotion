using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftResult : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;
    private PotionBrewer _brewer;
    [SerializeField]
    private Animator _animatorLeft;
    [SerializeField]
    private Animator _animatorRight;
    [SerializeField]
    private GameObject _rewardUIInstance;
    [SerializeField]
    private GameObject _resultCheckUIInstance;

    //UI
    [Header("Reward UI")]
    [SerializeField]
    private GameObject _rewardButtons;
    [SerializeField]
    private Text _questResultText;
    [SerializeField]
    private Text _moneyValueText;
    [SerializeField]
    private Text _recipeNameText;
    [SerializeField]
    private Text _selectText;
    [SerializeField]
    private Text _potionQualityValueText;
    [SerializeField]
    private Text _resultText;
    [SerializeField]
    private Button _selectButton;
    [SerializeField]
    private Button _moneyButton;
    [SerializeField]
    private Button _recipeButton;
    [SerializeField]
    private Outline _moneyOutline;
    [SerializeField]
    private Outline _recipeOutline;

    [Header("Result Check UI")]
    [SerializeField]
    private Image _potionImage;
    [SerializeField]
    private Text _potionName;
    [SerializeField]
    private Text _potionGrade;
    [SerializeField]
    private Text _potionQuality;
    [SerializeField]
    private Text _potionMinQuality;
    [SerializeField]
    private Text _potionMaxQuality;
    [SerializeField]
    private Slider _potionQualityProgressBar;
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _nextButton;



    //���� ���� ����
    private int _selectReward;
    //�Ƿ� ���� ���θ� �����ϴ� ����
    private bool _result;
    //���� �������� ID
    private int _rewardRecipeId;
    private float _rewardMultiplier = 1;

    public int PotionQuality
    {
        set { _potionQualityValueText.text = value.ToString(); }
    }

    public bool PotionCraftResult  {get => _result; set => _result = value;  }
    public PotionBrewer Brewer
    {
        get
        {
            if(_brewer == null)
            {
                _brewer = GameManager.GM.Brewer;
            }
            return _brewer;
        }
    }

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _brewer = GameManager.GM.Brewer;
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

        if (Brewer._currentQuest._isRestart) _restartButton.interactable = false;
        _potionQualityProgressBar.maxValue = 100;
    }

    //���� ���� ����� ǥ���ϴ� �Լ�
    public void ShowCraftResultUI()
    {
        InitializeRewardUI();
        //�ܰ��� ȿ�� ����
        OffHighlight();
        _restartButton.interactable = true;

        _resultCheckUIInstance.SetActive(false);
        _rewardUIInstance.SetActive(true);
        Brewer._craftState = PotionBrewer.CraftState.Next;

    }

    public void UpdateCraftResultUI()
    {
        var quest = Brewer._currentQuest;
        PlayInfo pinfo = GameManager.GM.PlayInformation;
        //�ϼ� �ִϸ��̼�
        _animatorLeft.SetTrigger("PlayOnce");
        _animatorRight.SetTrigger("PlayOnce");
        //_succImage.enabled = true;

        if (_result)
        {
            //Ư�� �����ǰ� �ƴ϶� ������ ����� �������� �����´�
            int RewardRecipeGrade = quest.QuestRewardRecipeGrade;

            //�ְ� ��� �޼��� ���� ����
            if (PlayInfo.PotionCraftGrade.RANK_1 == Brewer._potionCraftGrade)
            {
                _rewardMultiplier = PlayInfo.CRITICAL_SUCCESS;
                if(PlayInfo.MAX_RECIPE_GRADE> RewardRecipeGrade) RewardRecipeGrade++;
            }
            else _rewardMultiplier = 1;

            //ȹ�氡���� ������ ��������
            List<int> recipes = new List<int>();
            foreach (var RecipeData in ReadJson._dictPotion.Values)
            {
                if (RecipeData.potionGrade == RewardRecipeGrade
                    && pinfo.HasRecipe(RecipeData.potionId) == false)
                {
                    recipes.Add(RecipeData.potionId);
                }
            }

            _rewardButtons.SetActive(true);
            _moneyValueText.text = ((int)(quest.QuestRewardMoney * _rewardMultiplier)).ToString() + " ���";

            //ȹ�� ������ ������ Ȯ��
            if (recipes.Count == 0)
            {
                _recipeButton.enabled = false;
                _recipeNameText.text = "����";
            }
            else
            {
                int rndId = UnityEngine.Random.Range(0, recipes.Count);
                _rewardRecipeId = recipes[rndId];

                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                _recipeNameText.text = RecipeData.potionName.ToString() + " ������";
            }
            _selectText.text = "����";

            pinfo.QuestSuccessCount++;
        }
        else
        {
            _rewardButtons.SetActive(false);
            _selectText.text = "����";

            pinfo.ConsumeGold((int)(quest.QuestRewardMoney * 0.1));
        }
    }

    public void ShowResultCheckUI()
    {
        InitializeResultCheckUI();

        gameObject.SetActive(true);
        _resultCheckUIInstance.SetActive(true);
        _rewardUIInstance.SetActive(false);

        var questInfo = Brewer._currentQuest;

        _potionImage.sprite = questInfo.PotionImage;
        _potionName.text = questInfo.PotionName;
        _potionQuality.text = Brewer.CurrentPotionQuality.ToString();
        _potionMinQuality.text = questInfo.QInfo.minQuality.ToString();
        _potionMaxQuality.text = questInfo.QInfo.maxQuality.ToString();

        int currentQuality = Brewer.CurrentPotionQuality;


        if (currentQuality < questInfo.QInfo.minQuality) currentQuality = 0;

        float qualityPercent = (float)(currentQuality) / (float)(questInfo.QInfo.maxQuality) * 100.0f;

        
        _potionGrade.text = CheckPotionCraftGrade(qualityPercent);
        _potionQualityProgressBar.value = qualityPercent;
    }

    public string CheckPotionCraftGrade(float qualityPercent)
    {
        string potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_5;

        if (qualityPercent <= PlayInfo.PotionCraftGrade.BORDER_5)
        {
            potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_5;
        }
        else if (qualityPercent <= PlayInfo.PotionCraftGrade.BORDER_4 && qualityPercent > PlayInfo.PotionCraftGrade.BORDER_5 + 1)
        {
            potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_4;
        }
        else if (qualityPercent <= PlayInfo.PotionCraftGrade.BORDER_3 && qualityPercent > PlayInfo.PotionCraftGrade.BORDER_4 + 1)
        {
            potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_3;
        }
        else if (qualityPercent <= PlayInfo.PotionCraftGrade.BORDER_2 && qualityPercent > PlayInfo.PotionCraftGrade.BORDER_3 + 1)
        {
            potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_2;
        }
        else if (qualityPercent <= PlayInfo.PotionCraftGrade.BORDER_1 && qualityPercent > PlayInfo.PotionCraftGrade.BORDER_2 + 1)
        {
            potionCraftGrade = PlayInfo.PotionCraftGrade.RANK_1;
        }

        Brewer._potionCraftGrade = potionCraftGrade;

        return potionCraftGrade;
    }

    public void ShowResultText()
    {
        if (Brewer._craftState == PotionBrewer.CraftState.None) return;
        if (_result)
        {
            if(Brewer._potionCraftGrade == PlayInfo.PotionCraftGrade.RANK_1)
            _questResultText.text = "�Ƿ� �뼺��";
            else
            _questResultText.text = "�Ƿ� ����";
            _resultText.text = "����";
        }
        else
        {
            _questResultText.text = "�Ƿ� ����\n����� �߻�";
            _resultText.text = "����";
        }
    }

    //������ ������ PlayInfo�� ������Ʈ�ϴ� �Լ�
    public void SelectReward()
    {
        if (_result)
        {
            var quest = Brewer._currentQuest;

            if (_selectReward == 1)
            {
                //��� ȹ��
                GameManager.GM.PlayInformation.IncrementGold((int)(quest.QuestRewardMoney * _rewardMultiplier));
            }
            else if (_selectReward == 2)
            {
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                GameManager.GM.PlayInformation.AddRecipe(_rewardRecipeId);
                //UI�� ǥ���� ���ΰ�?
                Debug.Log(RecipeData.potionName.ToString() + " ������ �رݵǾ����ϴ�.");
            }
            else return;
        }

        //���� ���� ����
        Brewer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        _questResultText.text = "�Ƿ� ���";
        _resultText.text = "���";
        //_succImage.enabled = false;
        gameObject.SetActive(false);
    }

    public void RestartCraft()
    {
        if (Brewer._currentQuest._isRestart) return;
        Brewer._craftState = PotionBrewer.CraftState.Restart;
        gameObject.SetActive(false);
    }

    //���� ���̶���Ʈ ���� �Լ�
    public void OffHighlight()
    {
        _selectReward = 0;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectMoney()
    {
        // ���̶���Ʈ ȿ��
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
