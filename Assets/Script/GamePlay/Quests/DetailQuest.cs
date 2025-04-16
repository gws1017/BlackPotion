using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class DetailQuest : Quest
{
    [Header("UI")]
    [SerializeField] private Text _potionName;
    [SerializeField] private Text _rewardMoney;
    [SerializeField] private Text _currentAcceptQuest;
    [SerializeField] private Image _questGradeMark;
    [SerializeField] private Image _rewardRecipe;
    [SerializeField] private Text _rewardRecipeText;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _acceptButton;
    
    private bool _hasRecipe;

    //부모 퀘스트
    private Quest _parentQuest;
    public Quest ParentQuest { get =>_parentQuest; set => _parentQuest = value; }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        //버튼 초기화
        _quitButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.RemoveAllListeners();
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _acceptButton.onClick.AddListener(AcceptQuest);
    }

    protected override void InitializeData()
    {
        //부모 의뢰정보로 상세 의뢰 객체를 업데이트한다
        CopyQuestInfo(_parentQuest);
        base.InitializeData();

        //상세의뢰에만 있는 추가적인 데이터를 업데이트
        _potionName.text = PotionName;
        _rewardMoney.text = QuestRewardMoney.ToString();

        ShowRewardRecipeGrade();
        UpdateQuestGradeMark();

        //미보유 레시피 비활성화
        _hasRecipe = GameManager.GM.PlayInformation.HasRecipe(PInfo.potionId);
        if(_hasRecipe == false)
        {
            _acceptButton.interactable = false;
            Text[] textComponents = _acceptButton.GetComponentsInChildren<Text>(); 
            textComponents[0].text = "수락 불가";
            textComponents[1].text = "(보유하지 않은 레시피)";
        }
        else
        {
            SetAcceptQuestText(Board.CurrentAcceptQuestCount);
        }
    }

    public void CloseDetailQuest()
    {
        Board.EnableOpenButtons();
        Destroy(gameObject);
    }

    public void AcceptQuest()
    {
        Board.AcceptQuest(ParentQuest);
        SetAcceptQuestText(Board.CurrentAcceptQuestCount);
        CloseDetailQuest();
    }

    private void SetAcceptQuestText(int count)
    {
        _currentAcceptQuest.text = $"( {count} / {PlayInfo.MAX_ACCEPT_QUEST_COUNT} )";
    }

    //레시피 보상 등급에 따라 색으로 표기
    private void ShowRewardRecipeGrade()
    {
        PlayInfo.RecipeGrade grade = (PlayInfo.RecipeGrade)QuestRewardRecipeGrade;
        switch (grade)
        {
            case PlayInfo.RecipeGrade.Normal:
                _rewardRecipeText.color = Color.gray;
                break;
            case PlayInfo.RecipeGrade.Common:
                _rewardRecipeText.color = Color.white;
                break;
            case PlayInfo.RecipeGrade.Rare:
                _rewardRecipeText.color = Color.blue;
                break;
            case PlayInfo.RecipeGrade.Uncommon:
                _rewardRecipeText.color = Color.green;
                break;
            case PlayInfo.RecipeGrade.Legendary:
                _rewardRecipeText.color = Color.yellow;
                break;
        }
        _rewardRecipeText.text = PlayInfo.RecipeGradeToString((PlayInfo.RecipeGrade)QuestRewardRecipeGrade);

    }

    //의뢰 등급에 맞는 이미지를 업데이트함
    private void UpdateQuestGradeMark()
    {
        switch (_questGrade)
        {
            case PlayInfo.QuestGrade.Small:
                _gradeColor.color = Color.black;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/stamp_small");
                break;
            case PlayInfo.QuestGrade.Middle:
                _gradeColor.color = Color.magenta;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/stamp_medium");
                break;
            case PlayInfo.QuestGrade.Large:
                _gradeColor.color = Color.yellow;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/stamp_large");
                break;
        }
    }
}
