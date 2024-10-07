using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    //UI
    [SerializeField]
    protected Text _questText;
    [SerializeField]
    protected Text _potionQualityValue;

    [SerializeField]
    protected Image _gradeColor;
    [SerializeField]
    protected Image _potionImage;

    private Button _openDetailQuestButton;

    //������Ʈ
    private Canvas _canvas;

    //������
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

    //�Ƿ� ������
    protected QuestInfo _questInfo;
    protected PotionInfo _potionInfo;

    [SerializeField]
    private GameObject _detailQuestPrefab;
    private GameObject _detailQuestObject;

    //���۷���
    protected QuestBoard _questBoard;

    //Getter&Setter
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
                //�� ����Ʈ ��ü ����
                var dq = _detailQuestObject.GetComponent<DetailQuest>();
                //�θ� ID�� ��ü�� Set�Ѵ�.
                dq.QuestID = _questID;
                dq.ParentQuest = this;
                //�θ� ������ ������� �����͸� �ʱ�ȭ�Ѵ�.
                dq.InitilizeData();
                Debug.Log("������Ʈ �����Ǿ����ϴ�.");

            }
                return _detailQuestObject;
        }
    }
    public QuestBoard Board
    {
        get
        {
            if( _questBoard == null )
            {
                _questBoard = GameManager.GM.Board;
            }
            return _questBoard;
        }
    }
    
    public Canvas CanvasRef
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

    // Start is called before the first frame update
    void Start()
    {
        CanvasRef.worldCamera = GameManager.GM.MainCamera;
        _openDetailQuestButton = GetComponent<Button>();
        _openDetailQuestButton.onClick.AddListener(OpenDetailQuest);
        //ID�� ������ �������� �ο�
        _questID = Random.Range(3001, 3005);
        InitializeQuestInfo();
        InitilizeData();
    }

    private void OnMouseEnter()
    {
        if(Board._CanActiveSelectEffect)
            Board.QuestDisableEffectOn(gameObject);
    }

    private void OnMouseExit()
    {
        if(Board._CanActiveSelectEffect)
            Board.QuestDisableEffectOff();
    }

    protected void InitializeQuestInfo()
    {
        _questInfo = ReadJson._dictQuest[_questID];
    }

    protected void InitializeQuestInfo(Quest quest)
    {
        _questInfo = quest._questInfo;
    }

    virtual protected void InitilizeData()
    {
        _questText.text = _questInfo.questText;
        _questGrade = _questInfo.questGrade;
        _minPotionQuality = _questInfo.minQuality;
        _maxPotionQuality = _questInfo.maxQuality;

        _potionQualityValue.text = "( " + _minPotionQuality.ToString() + " ~ " 
            + _maxPotionQuality.ToString() + " )";

        CheckQuestGrade();
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);
        //����ID�� ����ؼ� ������ ����
        _potionInfo = ReadJson._dictPotion[_questInfo.potionId];
        _potionImage.sprite = Resources.Load<Sprite>(_potionInfo.potionImage);
    }

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

    public void OpenDetailQuest()
    {
        DetailQuestObject.SetActive(true);
        Debug.Log("������Ʈ �����մϴ�");
        //������Ʈ ����������� 
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
