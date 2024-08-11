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
    private Text _currentConfirmQuest;
    [SerializeField]
    private Image _questGradeMark;
    [SerializeField]
    private Image _rewardRecipe;

    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private Button _receiveButton;


    //�θ� ����Ʈ
    private Quest _parentQuest;

    public Quest ParentQuest
    {
        get { return _parentQuest; }
        set { _parentQuest = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCameraInCanvas();

        //�Ƿ� ID�� �θ� ����Ʈ ��ü����
        //�ڽ��� Detail ����Ʈ �������� ���� �ÿ� �����ϰ� �ʱ�ȭ �Ѵ�.

        //���� ������ �Ƿ� ���� ����Ʈ ���忡�� �����´�?
        int currentConfirmQuestCnt = 0;
        _currentConfirmQuest.text = "( " + currentConfirmQuestCnt.ToString() + " / 5 )";

        //��ư �ʱ�ȭ
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _receiveButton.onClick.AddListener(ReceiveQuest);

    }
    protected override void InitilizeData()
    {
        InitializeQuestInfo(_parentQuest);
        base.InitilizeData();

        _potionName.text = _potionInfo.potionName;
        _rewardMoney.text = _questInfo.money.ToString();
        CheckRewardRecipe();
        CheckQuestGradeMark();
    }

    private void CheckRewardRecipe()
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

    private void CheckQuestGradeMark()
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

    public void CloseDetailQuest()
    {
        GameObject.Find("QuestBoard").GetComponent<QuestBoard>().EnableOpenButtons();
        Debug.Log("���Ƿ� �ı�");

        Destroy(gameObject);
    }

    public void ReceiveQuest()
    {
        Debug.Log("�Ƿ� �����߽��ϴ�.");
    }
}
