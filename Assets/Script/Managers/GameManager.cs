using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static ConfirmUI;

public enum GameStage
{
    QuestBoard,       // �Ƿ� �Խ��� (0, 0, 0)
    Brewing,          // ���� ���� (0, 90, 0)
    Receipt,          // ���� (0, 180, 0)
}

public class GameManager : MonoBehaviour
{
    

    //GameManager �ν��Ͻ� GM���� ����
    private static GameManager _instance;

    //���� �� �ٽ� Ŭ������ ���ó�� �Ѱ��� ��Ƶ�
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

    //�Ƿ� ����(��������) �ܰ� ��ȯ
    private void QuestStart()
    {
        SoundManager._Instance.PlayClickSound();
        //���� �Ƿ� ����
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
            CreateInfoUI("�Ƿڸ� 1�� �̻� �����ؾ��մϴ�.",
                camCanvas.transform, null, Vector3.one * Constants.UI_SCALE);
        }
    }
    //���� �ܰ�� ��ȯ
    public void ShowCraftReceipt()
    {
        Receipt.ActiveHidePanel();
        ChangeStage(GameStage.Receipt);
    }
    //�Ƿ� �غ� �ܰ�(������) ��ȯ
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

    //���� ����� ���� 0������ ��������� �������� �Ѿ�� �� Ȯ���Ѵ�
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
