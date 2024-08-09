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
    private Button _confirmButton;


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
        //�Ƿ� ID�� �θ� ����Ʈ ��ü����
        //�ڽ��� Detail ����Ʈ �������� ���� �ÿ� �����ϰ� �ʱ�ȭ �Ѵ�.

        //���� ������ �Ƿ� ���� ����Ʈ ���忡�� �����´�?
        int currentConfirmQuestCnt = 0;
        _currentConfirmQuest.text = "( " + currentConfirmQuestCnt.ToString() + " / 5 )";
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
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/CommonGrade");
        }
        else if (recipeGrade == 2)
        {
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/UncommonGrade");
        }
        else if (recipeGrade == 3)
        {
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/RareGrade");

        }
        else if (recipeGrade == 4)
        {
            _questGradeMark.sprite = Resources.Load<Sprite>("Images/LegendaryGrade");
        }
    }

    private void CheckQuestGradeMark()
    {
        if (_questGrade == 1)
        {
            _gradeColor.color = Color.black;
        }
        else if (_questGrade == 2)
        {
            _gradeColor.color = Color.magenta;
        }
        else if (_questGrade == 3)
        {
            _gradeColor.color = Color.yellow;

        }
        else if (_questGrade == 4)
        {
            _gradeColor.color = Color.red;

        }
    }

}
