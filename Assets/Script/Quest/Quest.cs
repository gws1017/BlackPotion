using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class Quest : MonoBehaviour
{
    //컴포넌트
    private Canvas _canvas;

    //UI
    //의뢰 내용
    [SerializeField]
    protected Text _questText;
    //포션 품질 범위
    [SerializeField]
    protected Text _potionQualityValue;
    //의뢰 등급을 나타내는 테두리 색상
    [SerializeField]
    protected Image _gradeColor;
    //포션 이미지
    [SerializeField]
    protected Image _potionImage;
    //의뢰 상세내용 오픈 버튼
    private Button _openDetailQuestButton;

    //JSON에서 읽은 데이터
    [ReadOnly,SerializeField]
    protected int _questID;
    [SerializeField]
    protected int _minPotionQuality;
    [SerializeField]
    protected int _maxPotionQuality;
    [ReadOnly,SerializeField]
    protected int _reqPotionQuality;
    [SerializeField]
    protected int _questGrade;
    protected QuestInfo _questInfo;
    protected PotionInfo _potionInfo;

    //의뢰 상세내용 오브젝트
    [SerializeField]
    private GameObject _detailQuestPrefab;
    private GameObject _detailQuestObject;

    //비활성화된 퀘스트 체크
    private bool _disableQuest = false;

    //Getter&Setter
    public bool IsDisable
    {
        get {return _disableQuest;}
        set { _disableQuest = value;}
    }

    public int QuestID
    {
        get
        {
            return _questID;
        }
        set
        {
            _questID = value;
        }
    }
    public int RequirePotionQuality
    {
        get
        {
            return _reqPotionQuality;
        }
    }

    public int QuestGrade
    {
        get { return _questGrade; }
    }

    //의뢰 보상
    public int QuestRewardMoney
    {
        get { return QInfo.money; }
    }
    public int QuestRewardRecipeGrade
    {
        get { return QInfo.recipeGrade; }
    }

    public string QuestText
    {
        get
        {
            return _questText.text;
        }
        set
        {
            _questText.text = value;
        }
    }
    public string PotionName
    {
        get
        {
            return _potionInfo.potionName;
        }
    }
    public string PotionQualityValue
    {
        get
        {
            return _potionQualityValue.text;
        }
    }
    public Sprite PotionImage
    {
        get
        {
            return _potionImage.sprite;
        }
    }
    public GameObject DetailQuestObject
    {
        get
        {
            if(_detailQuestObject == null)
            {
                Vector3 pos = new Vector3(0, 0, -10);
                _detailQuestObject = Instantiate(_detailQuestPrefab, pos,Quaternion.identity);
                //상세 퀘스트 객체 생성
                var dq = _detailQuestObject.GetComponent<DetailQuest>();
                //부모 ID와 객체를 Set한다.
                dq.QuestID = _questID;
                dq.ParentQuest = this;
                //부모 정보를 기반으로 데이터를 초기화한다.
                dq.InitilizeData();
                Debug.Log("상세퀘스트 생성되었습니다.");

            }
                return _detailQuestObject;
        }
    }
    
    protected Canvas CanvasRef
    {
        get
        {
            if(_canvas == null)
            {
                _canvas = GetComponentInChildren<Canvas>();
            }
            return _canvas;
        }
    }

    protected QuestBoard Board
    {
        get { return GameManager.GM.Board; }
    }
    

    public QuestInfo QInfo
    {
        get
        {
            return _questInfo;
        }
    }
    public PotionInfo PInfo
    {
        get
        {
            return _potionInfo;
        }
    }

    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        _openDetailQuestButton = GetComponent<Button>();
        _openDetailQuestButton.onClick.AddListener(OpenDetailQuest);

        if(_questID == 0 )
            _questID = Random.Range(3001, 3005);

        InitializeQuestInfo();
        InitilizeData();
    }

    //마우스 오버된 의뢰를 강조하기위해 추가함
    private void OnMouseEnter()
    {
        //버튼 비활성화때는 아무것도하지마라
        if (_disableQuest)
            return;
        if(Board._CanActiveSelectEffect)
            Board.QuestDisableEffectOn(gameObject);
    }

    private void OnMouseExit()
    {
        if (_disableQuest)
            return;
        if (Board._CanActiveSelectEffect)
            Board.QuestDisableEffectOff();
    }

    //QuestID 기반으로 UI와 정보를 초기화한다
    public void InitializeQuestFromID(int id)
    {
        _questID = id;
        InitializeQuestInfo();
        InitilizeData();
    }

    //QuestID로 의뢰정보를 JSON에서 읽어온다
    protected void InitializeQuestInfo()
    {
        _questInfo = ReadJson._dictQuest[_questID];
    }

    //DetailQuest 객체에서 부모Quest정보로 초기화할 때 사용
    protected void InitializeQuestInfo(Quest quest)
    {
        _questInfo = quest._questInfo;
    }

    //Ques UI 표기 데이터나 실제 값들이 초기화된다
    virtual protected void InitilizeData()
    {
        //UI 정보 초기화
        _questText.text = _questInfo.questText;
        _questGrade = _questInfo.questGrade;
        _minPotionQuality = _questInfo.minQuality;
        _maxPotionQuality = _questInfo.maxQuality;

        _potionQualityValue.text = "( " + _minPotionQuality.ToString() + " ~ " 
            + _maxPotionQuality.ToString() + " )";

        CheckQuestGrade();
        //포션ID를 사용해서 정보를 읽자
        _potionInfo = ReadJson._dictPotion[_questInfo.potionId];
        _potionImage.sprite = Resources.Load<Sprite>(_potionInfo.potionImage);

        //포션 품질 범위내에서 무작위 값으로 요구 품질 값을 생성한다
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);

        //미보유 레시피 의뢰 마우스와 상호작용되지 않아야함
        if(GameManager.GM.PlayInfomation.HasRecipe(PInfo.potionId) == false)
        {
            _disableQuest = true;
            Vector3 originPos = gameObject.transform.position;
            originPos.z = 10;
            gameObject.transform.position = originPos;
        }
        else
        {
            _disableQuest = false;
        }
    }

    //의뢰등급에 맞게 테두리 색을 변경함
    private void CheckQuestGrade()
    {
        if (_questGrade == 1)
        {
            _gradeColor.color = Color.black;
        }
        else if (_questGrade == 2 )
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

    //의뢰 상세 내용 오픈
    public void OpenDetailQuest()
    {
        DetailQuestObject.SetActive(true);
        //상세퀘스트 열려있을경우 뒷 부분 클릭을 차단한다.
        Board._CanActiveSelectEffect = false;
        Board.DisableOpenButtons();
    }

    public void DisableOpenButton()
    {
        _openDetailQuestButton.interactable = false;
    }
    public void EnableOpenButton()
    {
        _openDetailQuestButton.interactable = true;
    }
}
