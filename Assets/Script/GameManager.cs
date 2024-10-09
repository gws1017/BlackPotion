using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    //�̱������� �ٲ����ϱ� static���� ��ü�ص� �������������
    private static GameManager _instance;
    private Camera _camera;

    private QuestBoard _board;
    private PotionBrewer _brewer;
    private CraftReceipt _craftReceipt;

    public PlayInfo _playInfo;

    public List<GameObject> destroyOjbect;
    [SerializeField]
    private Button _questStartButton;

    [System.Serializable]

    public struct SaveData
    {
        public List<int> acceptQuestId;
        public Quaternion camRotation;
    }

    [SerializeField]
    private SaveData _saveData;

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
        GameLoad();
        _questStartButton.onClick.AddListener(QuestStart);
    }

    private void QuestStart()
    {
        //���� �Ƿ� ����
        if (_board.CurrrentAcceptQuestCnt > 0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
            Brewer.UpdateQuestInfo();
            SaveQuest();
            SaveStage();
        }
        else
        {
            Debug.Log("�Ƿڸ� 1���̻� �����ϼž��մϴ�.");
        }
    }

    public void ShowCraftReceipt()
    {
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
        SaveStage();
    }
    public void ShowQuestBoard()
    {
        Board.IntitilizeQuestBoard();
        if (destroyOjbect.Count > 0)
        {
            foreach(var item in destroyOjbect)
                Destroy(item);
            destroyOjbect = new List<GameObject>();
        }
        _camera.transform.rotation = Quaternion.Euler(0, 0, 0);
        SaveQuest();
        SaveStage();

    }

    // ���̺� �����Լ�

    public void SaveQuest()
    {
        _saveData.acceptQuestId = new List<int>();
        foreach (var quest in Board._accpetQuestList)
        {
            _saveData.acceptQuestId.Add(quest.QuestID);
        }
        GameSave();
    }

    public void SaveStage()
    {
        _saveData.camRotation = MainCamera.transform.rotation;
        GameSave();
    }

    private void GameSave()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(_saveData));
        PlayerPrefs.Save();
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("Save"))
            return;
        string loadJson = PlayerPrefs.GetString("Save");
        _saveData = JsonUtility.FromJson<SaveData>(loadJson);

        //���� �ܰ�(ī�޶� ȸ��) Load
        MainCamera.transform.rotation = _saveData.camRotation;

        //���� �Ƿ� Load
        if (_saveData.acceptQuestId.Count > 0)
        {
            Board._accpetQuestList = new List<Quest>();
            int idCnt = Mathf.Min(5, _saveData.acceptQuestId.Count);
            for (int i = 0; i < idCnt; ++i)
            {
                int id = _saveData.acceptQuestId[i];
                var questObject = Instantiate(Board.QuestPrefab);
                questObject.GetComponent<Quest>().InitializeQuestFromID(id);
                destroyOjbect.Add(questObject);
                Board._accpetQuestList.Add(questObject.GetComponent<Quest>());
            }
        }


        Brewer.UpdateQuestInfo();

    }


}
