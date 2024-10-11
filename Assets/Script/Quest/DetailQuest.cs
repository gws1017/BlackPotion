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


    //부모 퀘스트
    private Quest _parentQuest;

    public Quest ParentQuest
    {
        get { return _parentQuest; }
        set { _parentQuest = value; }
    }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        //의뢰 ID는 부모 퀘스트 객체에서
        //자식인 Detail 퀘스트 프리팹을 생성 시에 전달하고 초기화 한다.

        //현재 수락한 의뢰 수를 퀘스트 보드에서 가져온다?
        SetAcceptQuestText(Board._accpetQuestList.Count);

        //버튼 초기화
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _acceptButton.onClick.AddListener(AcceptQuest);

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
        Board.EnableOpenButtons();
        Debug.Log("상세의뢰 파괴");

        Destroy(gameObject);
    }

    public void AcceptQuest()
    {
        Debug.Log("의뢰 수락했습니다.");
        Board.AcceptQuest(ParentQuest);
        SetAcceptQuestText(Board._accpetQuestList.Count);
        CloseDetailQuest();
    }

    private void SetAcceptQuestText(int count)
    {
        _currentAcceptQuest.text = "( " + count + " / " + Board.MaxAcceptQuestCount + " )";
    }
}
