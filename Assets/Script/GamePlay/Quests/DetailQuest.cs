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

    //������ ���� ��޿� ���� ������ ǥ��
    private void ShowRewardRecipeGrade()
    {
        Constants.SetRecipeIcon(_rewardRecipe, QuestRewardRecipeGrade);
        _rewardRecipeText.text = Constants.RecipeGradeToString((Constants.RecipeGrade)QuestRewardRecipeGrade);

    }

    //�Ƿ� ��޿� �´� �̹����� ������Ʈ��
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
