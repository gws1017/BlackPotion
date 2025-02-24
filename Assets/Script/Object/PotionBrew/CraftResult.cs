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

    //UI
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

    //���� ���� ����
    private int _selectReward;
    //�Ƿ� ���� ���θ� �����ϴ� ����
    private bool _result;
    //���� �������� ID
    private int _rewardRecipeId;

    public int PotionQuality
    {
        set { _potionQualityValueText.text = value.ToString(); }
    }

    public PotionBrewer Brwer
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
        InitializeResultUI();
    }

    void InitializeResultUI()
    {
        _selectButton.onClick.RemoveAllListeners();
        _moneyButton.onClick.RemoveAllListeners();
        _recipeButton.onClick.RemoveAllListeners();

        _selectButton.onClick.AddListener(SelectReward);
        _moneyButton.onClick.AddListener(SelectMoney);
        _recipeButton.onClick.AddListener(SelectRecipe);
    }

    //���� ���� ����� ǥ���ϴ� �Լ�
    public void ShowCraftResultUI(bool result)
    {
        InitializeResultUI();
        //�ܰ��� ȿ�� ����
        OffHighlight();
        _result = result;
        gameObject.SetActive(true);

        var quest = Brwer._currentQuest;
        PlayInfo pinfo = GameManager.GM.PlayInfomation;
        //�ϼ� �ִϸ��̼�
        _animatorLeft.SetTrigger("PlayOnce");
        _animatorRight.SetTrigger("PlayOnce");
        //_succImage.enabled = true;

        if (_result)
        {
            //Ư�� �����ǰ� �ƴ϶� ������ ����� �������� �����´�
            int RewardRecipeGrade = quest.QuestRewardRecipeGrade;

            //ȹ�氡���� ������ ��������
            List<int> recipes = new List<int>();
            foreach(var RecipeData in ReadJson._dictPotion.Values)
            {
                if(RecipeData.potionGrade == RewardRecipeGrade 
                    && pinfo.HasRecipe(RecipeData.potionId) == false)
                {
                    recipes.Add(RecipeData.potionId);
                }
            }

            _rewardButtons.SetActive(true);
            _moneyValueText.text = quest.QuestRewardMoney.ToString() + " ���";

            //ȹ�� ������ ������ Ȯ��
            if (recipes.Count == 0)
            {
                _recipeButton.enabled = false;
                _recipeNameText.text = "����";
            }
            else
            {
                int rndId = Random.Range(0, recipes.Count);
                _rewardRecipeId = recipes[rndId];
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];

                _recipeNameText.text = RecipeData.potionName.ToString() + " ������";
            }
            _selectText.text = "����";
            
            pinfo._questSuccCnt++;
        }
        else
        {
            _rewardButtons.SetActive(false);
            _selectText.text = "����";
            
            pinfo.ConsumeGold((int)(quest.QuestRewardMoney * 0.1));
        }
    }

    public void ShowResultText()
    {
        if (_result)
        {
            _questResultText.text = "�Ƿ� ����";
            _resultText.text = "����";
        }
        else
        {
            _questResultText.text = "�Ƿ� ����";
            _resultText.text = "����";
        }
    }

    //������ ������ PlayInfo�� ������Ʈ�ϴ� �Լ�
    public void SelectReward()
    {
        if (_result)
        {
            var quest = Brwer._currentQuest;

            if (_selectReward == 1)
            {
                //��� ȹ��
                GameManager.GM.PlayInfomation.IncreamentGold(quest.QuestRewardMoney);
            }
            else if (_selectReward == 2)
            {
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                GameManager.GM.PlayInfomation.AddRecipe(_rewardRecipeId);
                //UI�� ǥ���� ���ΰ�?
                Debug.Log(RecipeData.potionName.ToString() + " ������ �رݵǾ����ϴ�.");
            }
            else return;
        }

        //���� ���� ����
        Brwer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        _questResultText.text = "�Ƿ� ���";
        _resultText.text = "���";
        //_succImage.enabled = false;
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
