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

    //�θ� ����Ʈ
    private Quest _parentQuest;
    public Quest ParentQuest { get =>_parentQuest; set => _parentQuest = value; }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        //��ư �ʱ�ȭ
        _quitButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.RemoveAllListeners();
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _acceptButton.onClick.AddListener(AcceptQuest);
    }

    protected override void InitializeData()
    {
        //�θ� �Ƿ������� �� �Ƿ� ��ü�� ������Ʈ�Ѵ�
        CopyQuestInfo(_parentQuest);
        base.InitializeData();

        //���Ƿڿ��� �ִ� �߰����� �����͸� ������Ʈ
        _potionName.text = PotionName;
        _rewardMoney.text = QuestRewardMoney.ToString();

        ShowRewardRecipeGrade();
        UpdateQuestGradeMark();

        //�̺��� ������ ��Ȱ��ȭ
        _hasRecipe = GameManager.GM.PlayInformation.HasRecipe(PInfo.potionId);
        if(_hasRecipe == false)
        {
            _acceptButton.interactable = false;
            Text[] textComponents = _acceptButton.GetComponentsInChildren<Text>(); 
            textComponents[0].text = "���� �Ұ�";
            textComponents[1].text = "(�������� ���� ������)";
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

    //������ ���� ��޿� ���� ������ ǥ��
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

    //�Ƿ� ��޿� �´� �̹����� ������Ʈ��
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
