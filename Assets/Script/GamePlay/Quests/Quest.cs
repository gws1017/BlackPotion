using UnityEngine;
using UnityEngine.UI;


public class Quest : MonoBehaviour
{
    //컴포넌트
    private Canvas _canvas;
    protected QuestBoard _board;

    [Header("UI")]
    [SerializeField] protected Text _questText;
    [SerializeField] protected Text _potionQualityValue;
    [SerializeField] protected Image _gradeColor;
    [SerializeField] protected Image _potionImage;
    [SerializeField] private Button _openDetailQuestButton;

    [Header("Quest Data")]
    [ReadOnly,SerializeField] protected int _questID;
    [SerializeField] protected int _minPotionQuality;
    [SerializeField] protected int _maxPotionQuality;
    [ReadOnly,SerializeField] protected int _reqPotionQuality;
    [SerializeField] protected int _questGrade;
    protected QuestInfo _questInfo;
    protected PotionInfo _potionInfo;

    private Vector3 _originPosition;
    private Quaternion _originRotation;
    private QuestBoard.ZLayer _layer;
    private bool _isRestart;
    private bool _disableQuest = false;

    [Header("Deatil Quest")]
    [SerializeField] private GameObject _detailQuestPrefab;
    private GameObject _detailQuestObject;

    //Getter&Setter
    public float OriginZ => _originPosition.z; 
    public bool IsRestart { get => _isRestart; set => _isRestart = value; }
    public Vector3 OriginPosition { get => _originPosition; set => _originPosition = value; }
    public Quaternion OriginRotation { get => _originRotation; set => _originRotation = value; }
    public bool IsDisable { get => _disableQuest; set => _disableQuest = value; }
    public int QuestID { get=> _questID; set => _questID = value; }
    public int QuestGrade => _questGrade;
    //의뢰 보상
    public int QuestRewardMoney => QInfo.money;
    public int QuestRewardRecipeGrade => QInfo.recipeGrade;

    public string QuestText { get => _questInfo.questText;  set => _questText.text = value; }

    public int RequirePotionQuality => _reqPotionQuality;
    public string PotionName => _potionInfo.potionName;

    public string PotionQualityValue
    {
        get
        {
            if(_potionQualityValue == null)
            {
               return $"({_minPotionQuality} ~ {_maxPotionQuality} )";
            }
            return _potionQualityValue.text;
        }
    }

    public QuestBoard.ZLayer QuestLayer { get => _layer; set => _layer = value; }
    public Sprite PotionImage
    {
        get
        {
            if (_potionImage.sprite == null)
            {
                _potionImage.sprite = Resources.Load<Sprite>(_potionInfo.potionImage);
            }
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
                var dq = _detailQuestObject.GetComponent<DetailQuest>();
                dq.QuestID = _questID;
                dq.ParentQuest = this;
                dq.InitializeData();

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

    protected QuestBoard Board => _board ?? (_board = GameManager.GM.Board);
    public QuestInfo QInfo => _questInfo;
    public PotionInfo PInfo => _potionInfo;


    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;

        _openDetailQuestButton = GetComponent<Button>();
        _openDetailQuestButton.onClick.RemoveAllListeners();
        _openDetailQuestButton.onClick.AddListener(OpenDetailQuest);

        if(_questID == 0 )
            _questID = Random.Range(3001, 3005);

        InitializeQuestInfo();
        InitializeData();
    }

    private void OnMouseEnter()
    {
        if (_disableQuest || !Board._CanActiveSelectEffect) return;
        Board.QuestDisableEffectOn(gameObject);
    }

    private void OnMouseExit()
    {
        if (_disableQuest || !Board._CanActiveSelectEffect) return;
        Board.QuestDisableEffectOff();
    }

    public void InitializeQuestFromID(int id)
    {
        _questID = id;
        InitializeQuestInfo();
        InitializeData();
    }

    protected void InitializeQuestInfo()
    {
        _questInfo = ReadJson._dictQuest[_questID];
    }

    protected void CopyQuestInfo(Quest quest)
    {
        _questInfo = quest._questInfo;
    }

    virtual protected void InitializeData()
    {
        //데이터 초기화
        _questGrade = _questInfo.questGrade;
        _minPotionQuality = _questInfo.minQuality;
        _maxPotionQuality = _questInfo.maxQuality;
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);
        _potionInfo = ReadJson._dictPotion[_questInfo.potionId];

        //UI 업데이트
        _questText.text = _questInfo.questText;
        _potionQualityValue.text = $"( {_minPotionQuality} ~ {_maxPotionQuality}  )";
        CheckQuestGrade();
        _potionImage.sprite = Resources.Load<Sprite>(_potionInfo.potionImage);
    }

    private void CheckQuestGrade()
    {
        switch (_questGrade)
        {
            case 1:
                _gradeColor.color = Color.black;
                break;
            case 2:
                _gradeColor.color = Color.magenta;
                break;
            case 3:
                _gradeColor.color = Color.yellow;
                break;
            case 4:
                _gradeColor.color = Color.red;
                break;
        }
    }

    public void OpenDetailQuest()
    {
        DetailQuestObject.SetActive(true);
        Board._CanActiveSelectEffect = false;
        Board.DisableOpenButtons();
    }

    public void DisableOpenButton()
    {
        if(_openDetailQuestButton != null)
        _openDetailQuestButton.interactable = false;
        IsDisable = true;
    }
    public void EnableOpenButton()
    {
        if(_openDetailQuestButton != null)
            _openDetailQuestButton.interactable = true;
        IsDisable = false;
    }
}
