using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    //GameManager 인스턴스 GM으로 접근
    private static GameManager _instance;

    //게임 각 핵심 클래스를 허브처럼 한곳에 모아둠
    private Camera _camera;
    private QuestBoard _board;
    private PotionBrewer _brewer;
    private CraftReceipt _craftReceipt;
    private PlayInfo _playInfo;
    private SaveManager _saveManager;
    private BuffManager _buffManager;

    public List<GameObject> destroyOjbect;
    [SerializeField]
    private Button _questStartButton;

    public static GameManager GM
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
            }
            return _instance;
        }
    }

    //Getter Setter
    public Camera MainCamera
    {
        get
        {
            if (_camera == null)
            {
                _camera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
            }
            return _camera;
        }
    }


    public QuestBoard Board
    {
        get
        {
            if (_board == null)
            {
                _board = GameObject.Find("QuestBoard").GetComponent<QuestBoard>();
            }
            return _board;
        }
    }

    public PotionBrewer Brewer
    {
        get
        {
            if (_brewer == null)
            {
                _brewer = GameObject.Find("PotionBrewer").GetComponent<PotionBrewer>();
            }
            return _brewer;
        }
    }

    public CraftReceipt Receipt
    {
        get
        {
            if (_craftReceipt == null)
            {
                _craftReceipt = GameObject.Find("CraftReceipt").GetComponent<CraftReceipt>();
            }
            return _craftReceipt;
        }
    }

    public PlayInfo PlayInfomation
    {
        get { 
            if(_playInfo == null)
            {
                _playInfo = GetComponent<PlayInfo>();
            }
            return _playInfo; 
        }

    }

    public SaveManager SM
    {
        get
        {
            if (_saveManager == null)
            {
                _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>(); ;
            }
            return _saveManager;
        }
    }

    public BuffManager BM
    {
        get
        {
            if(_buffManager == null)
            {
                _buffManager = GameObject.Find("Pixel Perfect Camera").GetComponentInChildren<BuffManager>();
            }
            return _buffManager;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        _camera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
        _board = GameObject.Find("QuestBoard").GetComponent<QuestBoard>();
        _brewer = GameObject.Find("PotionBrewer").GetComponent<PotionBrewer>(); 
        _craftReceipt = GameObject.Find("CraftReceipt").GetComponent<CraftReceipt>();
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        _buffManager = GameObject.Find("Pixel Perfect Camera").GetComponentInChildren<BuffManager>();
        _playInfo = GetComponent<PlayInfo>();

        _questStartButton.onClick.AddListener(QuestStart);
    }

    //의뢰 시작(포션제조) 단계 전환
    private void QuestStart()
    {
        //수주 의뢰 저장
        if (_board.CurrrentAcceptQuestCnt > 0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
            Brewer.UpdateQuestInfo();
            SM.SaveQuest();
            SM.SaveStage();
        }
        else
        {
            Debug.Log("의뢰를 1개이상 수주하셔야합니다.");
        }
    }
    //정산 단계로 전환
    public void ShowCraftReceipt()
    {
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
        SM.SaveStage();
    }
    //의뢰 준비 단계(다음날) 전환
    public void ShowQuestBoard()
    {
        Board.IntitilizeQuestBoard();
        Board.CreateQuestObject();
        if (destroyOjbect.Count > 0)
        {
            foreach(var item in destroyOjbect)
                Destroy(item);
            destroyOjbect = new List<GameObject>();
        }
        _camera.transform.rotation = Quaternion.Euler(0, 0, 0);
        SM.SaveQuest();
        SM.SaveStage();

    }

    //정산 결과에 따라 0일차로 재시작인지 다음날로 넘어가는 지 확인한다
    public void CheckRecipt()
    {
        if(Receipt.TargetSuccess)
        {
            _playInfo.IncrementCraftDay();
        }
        else
        {
            _playInfo.Resetinfo();
        }
        ShowQuestBoard();
    }

    public void DestoryQuest(GameObject gameObject)
    {
        destroyOjbect.Add(gameObject);
    }

}
