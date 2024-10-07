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

    [SerializeField]
    private PotionBrewer _brewer;

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

    private int _selectId;
    private bool _result;
    private int _rewardRecipeId;

    public int PotionQuality
    {
        set { _potionQualityValueText.text = value.ToString(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;

        _selectButton.onClick.AddListener(RewardSelect);
        _moneyButton.onClick.AddListener(SelectMoney);
        _recipeButton.onClick.AddListener(SelectRecipe);
    }

    public void ShowCraftResult(bool result)
    {
        _result = result;
        gameObject.SetActive(true);

        var quest = _brewer._currentQuest;
        PlayInfo pinfo = GameManager.GM._playInfo;

        if (result)
        {
            int RewardRecipeGrade = quest.QuestRewardRecipeGrade;

            //획득가능한 레시피 가져오기
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
            _moneyValueText.text = quest.QuestRewardMoney.ToString() + " 골드";

            //획득 가능한 레시피 확인
            if (recipes.Count == 0)
            {
                _recipeButton.enabled = false;
                _recipeNameText.text = "매진";
            }
            else
            {
                int rndId = Random.Range(0, recipes.Count);
                _rewardRecipeId = recipes[rndId];
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];

                _recipeNameText.text = RecipeData.potionName.ToString() + " 레시피";
            }
            _selectText.text = "선택";
            _questResultText.text = "의뢰 성공";
            _resultText.text = "성공";
            pinfo._questSuccCnt++;
        }
        else
        {
            _rewardButtons.SetActive(false);
            _selectText.text = "다음";
            _questResultText.text = "의뢰 실패";
            _resultText.text = "실패";
            //마이너스 골드를 적용할 것인가?
            pinfo.ConsumeGold((int)(quest.QuestRewardMoney * 0.1));
        }
    }

    public void RewardSelect()
    {
        if (_result)
        {
            var quest = _brewer._currentQuest;

            if (_selectId == 1)
            {
                //골드 획득
                GameManager.GM._playInfo.IncreamentGold(quest.QuestRewardMoney);
                Debug.Log(quest.QuestRewardMoney.ToString() + " 획득했습니다.");
            }
            else if (_selectId == 2)
            {
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                GameManager.GM._playInfo.AddRecipe(_rewardRecipeId);
                Debug.Log(RecipeData.potionName.ToString() + " 레시피 해금되었습니다.");
            }
            else return;
        }

        _brewer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        gameObject.SetActive(false);
    }

    public void SelectMoney()
    {
        // 하이라이트 효과
        _selectId = 1;
        _moneyOutline.effectColor = Color.black;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectRecipe()
    {
        _selectId = 2;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.black;
    }


}
