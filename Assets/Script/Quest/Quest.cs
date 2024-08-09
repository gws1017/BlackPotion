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

    //컴포넌트
    private Canvas _canvas;

    //데이터
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

    //의뢰 데이터
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
        //ID는 생성시 무작위로 부여
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
        //포션ID를 사용해서 정보를 읽자
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
       Debug.Log("상세퀘스트 오픈합니다");
    }
}
