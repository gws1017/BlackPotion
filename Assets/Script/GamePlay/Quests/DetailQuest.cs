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
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _acceptButton;

    //부모 퀘스트
    private Quest _parentQuest;
    public Quest ParentQuest { get =>_parentQuest; set => _parentQuest = value; }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;
        SetAcceptQuestText(Board.CurrentAcceptQuestCount);

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
        _potionName.text = _potionInfo.potionName;
        _rewardMoney.text = _questInfo.money.ToString();

        ShowRewardRecipeGrade();
        UpdateQuestGradeMark();

        //미보유 레시피 비활성화
        _acceptButton.interactable = GameManager.GM.PlayInformation.HasRecipe(PInfo.potionId);
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
        switch (_questInfo.recipeGrade)
        {
            case 1:
                _rewardRecipe.color = Color.black;
                break;
            case 2:
                _rewardRecipe.color = Color.magenta;
                break;
            case 3:
                _rewardRecipe.color = Color.yellow;
                break;
            case 4:
                _rewardRecipe.color = Color.red;
                break;
        }

    }

    //의뢰 등급에 맞는 이미지를 업데이트함
    private void UpdateQuestGradeMark()
    {
        switch (_questGrade)
        {
            case 1:
                _gradeColor.color = Color.black;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkSmall");
                break;
            case 2:
                _gradeColor.color = Color.magenta;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkMid");
                break;
            case 3:
                _gradeColor.color = Color.yellow;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkLarge");

                break;
            case 4:
                _gradeColor.color = Color.red;
                _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkXLarge");
                break;
        }
    }
}
