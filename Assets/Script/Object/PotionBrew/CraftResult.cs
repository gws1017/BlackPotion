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

    //보상 선택 정보
    private int _selectReward;
    //의뢰 성공 여부를 저장하는 변수
    private bool _result;
    //보상 레시피의 ID
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

    //포션 제조 결과를 표시하는 함수
    public void ShowCraftResultUI(bool result)
    {
        InitializeResultUI();
        //외곽선 효과 오프
        OffHighlight();
        _result = result;
        gameObject.SetActive(true);

        var quest = Brwer._currentQuest;
        PlayInfo pinfo = GameManager.GM.PlayInfomation;
        //완성 애니메이션
        _animatorLeft.SetTrigger("PlayOnce");
        _animatorRight.SetTrigger("PlayOnce");
        //_succImage.enabled = true;

        if (_result)
        {
            //특정 레시피가 아니라 레시피 등급중 무작위로 가져온다
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
            
            pinfo._questSuccCnt++;
        }
        else
        {
            _rewardButtons.SetActive(false);
            _selectText.text = "다음";
            
            pinfo.ConsumeGold((int)(quest.QuestRewardMoney * 0.1));
        }
    }

    public void ShowResultText()
    {
        if (_result)
        {
            _questResultText.text = "의뢰 성공";
            _resultText.text = "성공";
        }
        else
        {
            _questResultText.text = "의뢰 실패";
            _resultText.text = "실패";
        }
    }

    //선택한 보상을 PlayInfo에 업데이트하는 함수
    public void SelectReward()
    {
        if (_result)
        {
            var quest = Brwer._currentQuest;

            if (_selectReward == 1)
            {
                //골드 획득
                GameManager.GM.PlayInfomation.IncreamentGold(quest.QuestRewardMoney);
            }
            else if (_selectReward == 2)
            {
                var RecipeData = ReadJson._dictPotion[_rewardRecipeId];
                GameManager.GM.PlayInfomation.AddRecipe(_rewardRecipeId);
                //UI로 표기할 것인가?
                Debug.Log(RecipeData.potionName.ToString() + " 레시피 해금되었습니다.");
            }
            else return;
        }

        //버프 상점 오픈
        Brwer.StoreUI.OpenStoreUI(Store.StoreType.Buff);
        _questResultText.text = "의뢰 결과";
        _resultText.text = "결과";
        //_succImage.enabled = false;
        gameObject.SetActive(false);
    }

    //보상 하이라이트 관련 함수
    public void OffHighlight()
    {
        _selectReward = 0;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectMoney()
    {
        // 하이라이트 효과
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
