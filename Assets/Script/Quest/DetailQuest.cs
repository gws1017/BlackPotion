using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class DetailQuest : Quest
{
    //UI
    [SerializeField]
    private Text _potionName;
    [SerializeField]
    private Text _rewardMoney;
    [SerializeField]
    private Text _currentAcceptQuest;
    [SerializeField]
    private Image _questGradeMark;
    [SerializeField]
    private Image _rewardRecipe;

    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private Button _acceptButton;


    //�θ� ����Ʈ
    private Quest _parentQuest;

    public Quest ParentQuest
    {
        get { return _parentQuest; }
        set { _parentQuest = value; }
    }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        //�Ƿ� ID�� �θ� ����Ʈ ��ü����
        //�ڽ��� Detail ����Ʈ �������� ���� �ÿ� �����ϰ� �ʱ�ȭ �Ѵ�.

        SetAcceptQuestText(Board._accpetQuestList.Count);

        //��ư �ʱ�ȭ
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _acceptButton.onClick.AddListener(AcceptQuest);

    }

   
    protected override void InitilizeData()
    {
        //�θ� �Ƿ������� �� �Ƿ� ��ü�� ������Ʈ�Ѵ�
        InitializeQuestInfo(_parentQuest);
        base.InitilizeData();

        //���Ƿڿ��� �ִ� �߰����� �����͸� ������Ʈ
        _potionName.text = _potionInfo.potionName;
        _rewardMoney.text = _questInfo.money.ToString();
        ShowRewardRecipeGrade();
        UpdateQuestGradeMark();
    }

    //���Ƿ� Object �ݴ� �Լ�
    public void CloseDetailQuest()
    {
        Board.EnableOpenButtons();
        Debug.Log("���Ƿ� �ı�");

        Destroy(gameObject);
    }

    //�Ƿ� ���� �Լ�
    public void AcceptQuest()
    {
        Debug.Log("�Ƿ� �����߽��ϴ�.");
        Board.AcceptQuest(ParentQuest);
        SetAcceptQuestText(Board._accpetQuestList.Count);
        CloseDetailQuest();
    }

    //���� ��Ȳ UI ������Ʈ
    private void SetAcceptQuestText(int count)
    {
        _currentAcceptQuest.text = "( " + count + " / " + Board.MaxAcceptQuestCount + " )";
    }

    //������ ���� ��޿� ���� ������ ǥ��
    private void ShowRewardRecipeGrade()
    {
        int recipeGrade = _questInfo.recipeGrade;

        if (recipeGrade == 1)
        {
            _rewardRecipe.color = Color.black;
        }
        else if (recipeGrade == 2)
        {
            _rewardRecipe.color = Color.magenta;
        }
        else if (recipeGrade == 3)
        {
            _rewardRecipe.color = Color.yellow;

        }
        else if (recipeGrade == 4)
        {
            _rewardRecipe.color = Color.red;
        }
    }

    //�Ƿ� ��޿� �´� �̹����� ������Ʈ��
    private void UpdateQuestGradeMark()
    {
        if (_questGrade == 1)
        {
            _gradeColor.color = Color.black;
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkSmall");
        }
        else if (_questGrade == 2)
        {
            _gradeColor.color = Color.magenta;
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkMid");
        }
        else if (_questGrade == 3)
        {
            _gradeColor.color = Color.yellow;
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkLarge");

        }
        else if (_questGrade == 4)
        {
            _gradeColor.color = Color.red;
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/MarkXLarge");
        }
    }
}
