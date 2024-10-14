using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    //�Ƿ� ����(��������) �ܰ� ��ȯ
    private void QuestStart()
    {
        if (_board.CurrrentAcceptQuestCnt > 0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
            Brewer.UpdateQuestInfo();
        }
        else
        {
            Debug.Log("�Ƿڸ� 1���̻� �����ϼž��մϴ�.");
        }
    }
    //���� �ܰ�� ��ȯ
    public void ShowCraftReceipt()
    {
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    //�Ƿ� �غ� �ܰ�(������) ��ȯ
    public void ShowQuestBoard()
    {
        _board.IntitilizeQuestBoard();
        _camera.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    //���� ����� ���� 0������ ��������� �������� �Ѿ�� �� Ȯ���Ѵ�
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
