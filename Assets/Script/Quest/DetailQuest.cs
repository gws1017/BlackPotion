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

        SetAcceptQuestText(Board._accpetQuestList.Count);

        //버튼 초기화
        _quitButton.onClick.AddListener(CloseDetailQuest);
        _acceptButton.onClick.AddListener(AcceptQuest);

    }

   
    protected override void InitilizeData()
    {
        //부모 의뢰정보로 상세 의뢰 객체를 업데이트한다
        InitializeQuestInfo(_parentQuest);
        base.InitilizeData();

        //상세의뢰에만 있는 추가적인 데이터를 업데이트
        _potionName.text = _potionInfo.potionName;
        _rewardMoney.text = _questInfo.money.ToString();
        ShowRewardRecipeGrade();
        UpdateQuestGradeMark();
    }

    //상세의뢰 Object 닫는 함수
    public void CloseDetailQuest()
    {
        Board.EnableOpenButtons();
        Debug.Log("상세의뢰 파괴");

        Destroy(gameObject);
    }

    //의뢰 수락 함수
    public void AcceptQuest()
    {
        Debug.Log("의뢰 수락했습니다.");
        Board.AcceptQuest(ParentQuest);
        SetAcceptQuestText(Board._accpetQuestList.Count);
        CloseDetailQuest();
    }

    //수락 현황 UI 업데이트
    private void SetAcceptQuestText(int count)
    {
        _currentAcceptQuest.text = "( " + count + " / " + Board.MaxAcceptQuestCount + " )";
    }

    //레시피 보상 등급에 따라 색으로 표기
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

    //의뢰 등급에 맞는 이미지를 업데이트함
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
