using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //싱글톤으로 바꿨으니까 static으로 교체해도 상관없지않을까
    private static GameManager _instance;
    private Camera _camera;

    private QuestBoard _board;
    private PotionBrewer _brewer;
    private CraftReceipt _craftReceipt;

    public PlayInfo _playInfo;

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
            if(_camera == null)
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

    // Start is called before the first frame update
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
        _questStartButton.onClick.AddListener(QuestStart);
    }

    private void QuestStart()
    {
        if (_board.CurrrentAcceptQuestCnt > 0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
            Brewer.UpdateQuestInfo();
        }
        else
        {
            Debug.Log("의뢰를 1개이상 수주하셔야합니다.");
        }
    }

    public void ShowCraftReceipt()
    {
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    public void ShowQuestBoard()
    {
        _board.IntitilizeQuestBoard();
        _camera.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

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
}
