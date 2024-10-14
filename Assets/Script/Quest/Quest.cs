using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class Quest : MonoBehaviour
{
    //������Ʈ
    private Canvas _canvas;

    //UI
    //�Ƿ� ����
    [SerializeField]
    protected Text _questText;
    //���� ǰ�� ����
    [SerializeField]
    protected Text _potionQualityValue;
    //�Ƿ� ����� ��Ÿ���� �׵θ� ����
    [SerializeField]
    protected Image _gradeColor;
    //���� �̹���
    [SerializeField]
    protected Image _potionImage;
    //�Ƿ� �󼼳��� ���� ��ư
    private Button _openDetailQuestButton;

    //JSON���� ���� ������
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

    //�Ƿ� �󼼳��� ������Ʈ
    [SerializeField]
    private GameObject _detailQuestPrefab;
    private GameObject _detailQuestObject;

    //��Ȱ��ȭ�� ����Ʈ üũ
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

    //�Ƿ� ����
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

    //���콺 ������ �Ƿڸ� �����ϱ����� �߰���
    private void OnMouseEnter()
    {
        //��ư ��Ȱ��ȭ���� �ƹ��͵���������
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

    //QuestID ������� UI�� ������ �ʱ�ȭ�Ѵ�
    public void InitializeQuestFromID(int id)
    {
        _questID = id;
        InitializeQuestInfo();
        InitilizeData();
    }

    //QuestID�� �Ƿ������� JSON���� �о�´�
    protected void InitializeQuestInfo()
    {
        _questInfo = ReadJson._dictQuest[_questID];
    }

    //DetailQuest ��ü���� �θ�Quest������ �ʱ�ȭ�� �� ���
    protected void InitializeQuestInfo(Quest quest)
    {
        _questInfo = quest._questInfo;
    }

    //Ques UI ǥ�� �����ͳ� ���� ������ �ʱ�ȭ�ȴ�
    virtual protected void InitilizeData()
    {
        //UI ���� �ʱ�ȭ
        _questText.text = _questInfo.questText;
        _questGrade = _questInfo.questGrade;
        _minPotionQuality = _questInfo.minQuality;
        _maxPotionQuality = _questInfo.maxQuality;

        _potionQualityValue.text = "( " + _minPotionQuality.ToString() + " ~ " 
            + _maxPotionQuality.ToString() + " )";

        CheckQuestGrade();
        //����ID�� ����ؼ� ������ ����
        _potionInfo = ReadJson._dictPotion[_questInfo.potionId];
        _potionImage.sprite = Resources.Load<Sprite>(_potionInfo.potionImage);

        //���� ǰ�� ���������� ������ ������ �䱸 ǰ�� ���� �����Ѵ�
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);

        //�̺��� ������ �Ƿ� ���콺�� ��ȣ�ۿ���� �ʾƾ���
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

    //�Ƿڵ�޿� �°� �׵θ� ���� ������
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

    //�Ƿ� �� ���� ����
    public void OpenDetailQuest()
    {
        DetailQuestObject.SetActive(true);
        //������Ʈ ����������� �� �κ� Ŭ���� �����Ѵ�.
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
