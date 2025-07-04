using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static ConfirmUI;

public enum GameStage
{
    QuestBoard,       // 의뢰 게시판 (0, 0, 0)
    Brewing,          // 포션 제조 (0, 90, 0)
    Receipt,          // 정산 (0, 180, 0)
}

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

    public List<GameObject> destroyObjects;
    [SerializeField] private Button _questStartButton;

    [SerializeField] private Text _debugText;

    [SerializeField] private GameObject _infoUIPrefab;

    private readonly Dictionary<GameStage, Vector3> _stageRotationDict = new Dictionary<GameStage, Vector3>
    {
        { GameStage.QuestBoard, new Vector3(0, 0, 0) },
        { GameStage.Brewing, new Vector3(0, 90, 0) },
        { GameStage.Receipt, new Vector3(0, 180, 0) },
    };

    private GameStage _stage = GameStage.QuestBoard;

    //Getter Setter
    public GameStage CurrentStage => _stage;
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

    public PlayInfo PlayInformation
    {
        get
        {
            if (_playInfo == null)
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
            if (_buffManager == null)
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
        InitializeGameManager();

    }

    public void InitializeGameManager(bool isLoad = false)
    {
        InitializeManagerComponent();
        
        if (_questStartButton == null)
            _questStartButton = Board._questStartButton;
        ChangeStage(GameStage.QuestBoard);
        _questStartButton.onClick.RemoveAllListeners();
        _questStartButton.onClick.AddListener(QuestStart);
    }

    private void InitializeManagerComponent(bool isLoad = false)
    {
        if (_camera == null)
            _camera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
        if (_board == null)
            _board = GameObject.Find("QuestBoard").GetComponent<QuestBoard>();
        if (_brewer == null)
            _brewer = GameObject.Find("PotionBrewer").GetComponent<PotionBrewer>();
        if (_craftReceipt == null)
            _craftReceipt = GameObject.Find("CraftReceipt").GetComponent<CraftReceipt>();
        if (_saveManager == null)
            _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        if (_buffManager == null)
            _buffManager = GameObject.Find("Pixel Perfect Camera").GetComponentInChildren<BuffManager>();
        if (_playInfo == null)
            _playInfo = GetComponent<PlayInfo>();
    }

    //의뢰 시작(포션제조) 단계 전환
    private void QuestStart()
    {
        SoundManager._Instance.PlayClickSound();
        //수주 의뢰 저장
        if (Board.CurrentAcceptQuestCount > 0)
        {
            Board.CloseCurrentQuestUI();
            Board.CurrentQuestOutlineEffectOff();
            ChangeStage(GameStage.Brewing);
            Brewer.UpdateQuestInfo();
            SM.SaveQuestList();
        }
        else
        {
            Canvas camCanvas = MainCamera.GetComponentInChildren<Canvas>();
            CreateInfoUI("의뢰를 1개 이상 수락해야합니다.",
                camCanvas.transform, null, Vector3.one * Constants.UI_SCALE);
        }
    }
    //정산 단계로 전환
    public void ShowCraftReceipt()
    {
        Receipt.ActiveHidePanel();
        ChangeStage(GameStage.Receipt);
    }
    //의뢰 준비 단계(다음날) 전환
    public void ShowQuestBoard()
    {
        Receipt.DeActiveHidePanel();

        ChangeStage(GameStage.QuestBoard);
        Board.InitializeQuestBoard();

        foreach (var item in destroyObjects)
        {
            Destroy(item);
        }
        destroyObjects.Clear();

        SM.SaveQuestList();

    }

    //정산 결과에 따라 0일차로 재시작인지 다음날로 넘어가는 지 확인한다
    public void TryNextDay()
    {
        if (Receipt.TargetSuccess)
        {
            PlayInformation.IncrementCraftDay();
            ShowQuestBoard();
        }
        else
        {
            MainCamera.GetComponentInChildren<GamePlayHUD>().ShowRestartUI();
        }
    }

    public void DestoryQuest(GameObject gameObject)
    {
        destroyObjects.Add(gameObject);
    }

    public void ChangeStage(Quaternion rotation)
    {

        foreach(var stage in _stageRotationDict)
        {
            if(Quaternion.Euler(stage.Value) == rotation)
            {
                _camera.transform.rotation = (rotation);
                _stage = stage.Key;
            }
        }
        
    }

    private void ChangeStage(GameStage stage)
    {
        if (_camera != null && _stageRotationDict.TryGetValue(stage, out var rotation))
            _camera.transform.rotation = Quaternion.Euler(rotation);
        _stage = stage;
        SM.SaveStage();
    }

    public void CreateInfoUI(string infoText, Transform parentTransform, Vector3? localPosition = null, Vector3? localScale = null, UIInfoType type = UIInfoType.Confirm, Action yesFunc = null)
    {
        GameObject uiPrefab = Instantiate(_infoUIPrefab, parentTransform);
        if (localPosition.HasValue)
            uiPrefab.transform.localPosition = localPosition.Value;
        if (localScale.HasValue)
            uiPrefab.transform.localScale = localScale.Value;
        uiPrefab.GetComponent<ConfirmUI>().InitializeUI(infoText,type,yesFunc);
    }
}
