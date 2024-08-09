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


    // Start is called before the first frame update
    void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.worldCamera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
        _openDetailQuestButton = GetComponent<Button>();
        _openDetailQuestButton.onClick.AddListener(OpenDetailQuest);
        //ID�� ������ �������� �ο�
        _questID = Random.Range(3001, 3004);
        InitializeQuestInfo();
        InitilizeData();
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
    }
}
