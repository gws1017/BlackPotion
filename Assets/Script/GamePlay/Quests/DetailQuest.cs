using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class DetailQuest : Quest
{
    [Header("UI")]
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
        SoundManager._Instance.PlayClickSound();
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
        if (count >= Constants.MAX_ACCEPT_QUEST_COUNT)
            _acceptButton.interactable = false;
        else
            _acceptButton.interactable = true;

        _currentAcceptQuest.text = $"( {count} / {Constants.MAX_ACCEPT_QUEST_COUNT} )";
    }

    //레시피 보상 등급에 따라 색으로 표기
    private void ShowRewardRecipeGrade()
    {
        Constants.SetRecipeIcon(_rewardRecipe, QuestRewardRecipeGrade);
        _rewardRecipeText.text = Constants.RecipeGradeToString((Constants.RecipeGrade)QuestRewardRecipeGrade);

    }

    //의뢰 등급에 맞는 이미지를 업데이트함
    private void UpdateQuestGradeMark()
    {
        switch (_questGrade)
        {
            case Constants.QuestGrade.Small:
                _questGradeMark.sprite = Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_SMALL);
                break;
            case Constants.QuestGrade.Middle:
                _questGradeMark.sprite = Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_MEDIUM);
                break;
            case Constants.QuestGrade.Large:
                _questGradeMark.sprite = Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_LARGE);
                break;
        }
    }
}
