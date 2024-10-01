using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�̱������� �ٲ����ϱ� static���� ��ü�ص� �������������
    private static GameManager _instance;
    private Camera _camera;

    private QuestBoard _board;
    private PotionBrewer _brewer;

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
            Debug.Log("�Ƿڸ� 1���̻� �����ϼž��մϴ�.");
        }
    }

    public void ShowCraftReceipt()
    {
        //CraftReceipt�� GameManger�Ʒ����ΰ� UpdateReceipt�� ȣ���ϰ�;�����
        //�� �Լ����� ȣ��� CraftReceipt�� static�� ������������ static�Լ�����
        //ȣ���� �� ���� , CraftReceipt������ static ������ �ʿ����������� ���̴°�
        //�ƴϵǹǷ�, �� �Լ��� ȣ���ϴ� potionBrwer�� CraftReceipt�� �ΰ� ȣ���Ѵ�
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
