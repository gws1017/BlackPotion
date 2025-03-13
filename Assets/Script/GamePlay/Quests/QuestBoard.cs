using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoard : MonoBehaviour
{
    public const float LAYER_OFFSET_MULTIPLIER = 2.5f;
    public enum ZLayer
    {
        QuestFloor1 = 3,
        QuestFloor2 = 2,
        QuestFloor3 = 1,
        Curtain = 0,
        Highlight = -1
    }

    //UI
    [SerializeField]
    private GameObject _curtainPanel;
    

    [ReadOnly]
    public float _layerOffset;

    //Quest ���� ���
    [ReadOnly, SerializeField]
    private Dictionary<ZLayer, List<GameObject>> _questList;
    //���� Quest ���
    [SerializeField]
    private List<Quest> _acceptQuestList;

    //Quest ���
    public Dictionary<Quest, bool> _questResultDict;

    //���� / �̺��� ������ ���
    private List<int> _acceptableQuestList;
    private List<int> _unAcceptableQuestList;

    //�Ƿ� ������
    [SerializeField]
    private GameObject _questPrefab;

    [SerializeField]
    private int _maxLoopCount;//�浹 �˻� Ƚ��
    [SerializeField]
    private float _zRotRandRange; //Quest ȸ�� ����
    [SerializeField]
    private int _maxQuestCnt; //�Ƿ� �ִ� ������
    [SerializeField]
    private int _maxAcceptQuestCount; //�ִ� �������� Quest
    private Vector3 _meshExtents;

    [SerializeField]
    private Vector3 _firstRowStartPos = new Vector3(-20, 4, -23);
    [SerializeField]
    private Vector3 _secondRowStartPos = new Vector3(-10, -8, -23);

    [ReadOnly]
    public bool _CanActiveSelectEffect = true;

    [Header("Current Quest")]
    [SerializeField]
    private Button _questBoardButton;
    [SerializeField]
    private Button _questCanelButton;
    [SerializeField]
    private Button _questNextButton;
    [SerializeField]
    private Button _currentQuestButton;
    [SerializeField]
    private GameObject _currentQuestUIObject;

    public GameObject QuestPrefab { get=> _questPrefab; }

    public int MaxAcceptQuestCount { get => _maxAcceptQuestCount; }

    public int CurrentAcceptQuestCount => _acceptQuestList.Count;

    public List<Quest> AcceptQuestList { get => _acceptQuestList; }

    public Quest GetCurrentQuest(int id)
    {
        id = Mathf.Clamp(id, 0, AcceptQuestList.Count - 1);
        return AcceptQuestList[id];
    }

    void Start()
    {
        MeshFilter meshFilter = _questPrefab.GetComponent<MeshFilter>();
        _meshExtents = meshFilter.sharedMesh.bounds.extents;
        _meshExtents.Scale(_questPrefab.transform.localScale);
        _layerOffset = _meshExtents.z;

        InitializeQuestBoard();
        CreateQuestObject();
    }


    public void InitializeQuestBoard()
    {
        //���� ������Ʈ ����
        if (_questList != null)
        {
            foreach (var quests in _questList.Values)
            {
                foreach (var questObj in quests)
                {
                    Destroy(questObj);
                }
            }
        }
        if (_acceptQuestList != null)
        {
            foreach (Quest quest in _acceptQuestList)
                Destroy(quest.gameObject);
        }

        //���� �ʱ�ȭ
        _currentQuestUIObject.SetActive(false);
        if (_questList == null || _questList.Count > 0)
            _questList = new Dictionary<ZLayer, List<GameObject>>();
        if (_acceptQuestList == null || _acceptQuestList.Count > 0)
            _acceptQuestList = new List<Quest>();
        if (_questResultDict == null || _questResultDict.Count > 0)
            _questResultDict = new Dictionary<Quest, bool>();

        _questNextButton.onClick.RemoveAllListeners();
        _questCanelButton.onClick.RemoveAllListeners();
        _questBoardButton.onClick.RemoveAllListeners();
        _currentQuestButton.onClick.RemoveAllListeners();
        _currentQuestButton.onClick.AddListener(OpenCurrentQuestUI);
        //_questNextButton.onClick.AddListener(OpenAcceptQuestUI);
        //_questCanelButton.onClick.AddListener(OpenAcceptQuestUI);
        _questBoardButton.onClick.AddListener(CloseCurrentQuestUI);
    }

    public void CreateQuestObject()
    {
        //������ ������
        GameManager.GM.PlayInformation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);

        foreach (ZLayer layer in Enum.GetValues(typeof(ZLayer)))
        {
            if (layer == ZLayer.Curtain || layer == ZLayer.Highlight) continue;
            _questList[layer] = new List<GameObject>();
        }

        int acceptableCount = 0, unacceptableCount = 0;

        //�Ƿ� ��ü�� ����
        for (int i = 0; i < _maxQuestCnt; ++i)
        {
            Vector3 pos;
            Quaternion rot;

            ZLayer selectedlayer = ZLayer.QuestFloor3;
            foreach (ZLayer layer in new[] { ZLayer.QuestFloor3, ZLayer.QuestFloor2, ZLayer.QuestFloor1 })
            {
                if (_questList[layer].Count < PlayInfo.MAX_QUEST_COUNT_LAYER)
                    selectedlayer = layer;
            }

            GenerateRandomPositionAndRotation(selectedlayer, out pos, out rot);

            //���� ��Ȳ Ȯ���ϸ鼭 ID ����
            int questId = GetQuestID(ref acceptableCount,ref unacceptableCount);

            GameObject Clone = Instantiate(_questPrefab, pos, rot);
            Quest quest = Clone.GetComponent<Quest>();
            Canvas questCanvas = Clone.GetComponentInChildren<Canvas>();

            quest.QuestID = questId;
            quest.OriginZ = pos.z;
            quest.QuestLayer = selectedlayer;
            //questCanvas.overrideSorting = true;

            _questList[selectedlayer].Add(Clone);
        }
    }

    private void GenerateRandomPositionAndRotation(ZLayer layer, out Vector3 position, out Quaternion rotation)
    {
        Camera cam = GameManager.GM.MainCamera;
        Bounds bounds = cam.GetComponentInChildren<Collider2D>().bounds;

        float randZ = (float)layer * _layerOffset * LAYER_OFFSET_MULTIPLIER;
        float randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        position = new Vector3(randX, randY, randZ);

        Collider[] colliders = Physics.OverlapBox(position, _meshExtents);
        int loopCount = 0;
        while (colliders.Length != 0)
        {
            if (loopCount > _maxLoopCount) break;
            loopCount++;
            randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            position = new Vector3(randX, randY, randZ);
            colliders = Physics.OverlapBox(position, _meshExtents);
        }

        float randRotZ = UnityEngine.Random.Range(-_zRotRandRange, _zRotRandRange);
        rotation = Quaternion.Euler(0f, 0f, randRotZ);
    }

    public void AcceptQuest(Quest quest)
    {
        if (_acceptQuestList.Contains(quest))
        {
            Debug.Log("�̹� ������ ����Ʈ �Դϴ�.");
            return;
        }
        if (_acceptQuestList.Count >= _maxAcceptQuestCount)
        {
            Debug.Log("����Ʈ�� �ִ�ġ�� �����߽��ϴ�.");
            return;
        }
        
        _acceptQuestList.Add(quest);
        _questResultDict.Add(quest, false);

        if (_questList.TryGetValue(quest.QuestLayer, out List<GameObject> questLayerList))
        {
            questLayerList.Remove(quest.gameObject);
        }
        quest.IsDisable = true;

        //������ �Ƿڴ� ���� �Ƿ� UI�� �̵�
        Vector3 position = new Vector3(0, 0, (float)ZLayer.Highlight) *_layerOffset * LAYER_OFFSET_MULTIPLIER;
        quest.gameObject.transform.SetParent(_currentQuestUIObject.transform);
        quest.transform.SetPositionAndRotation(position, Quaternion.identity);
        //x 1.5 , rot -10

    }

    public void OpenCurrentQuestUI()
    {
        _currentQuestUIObject.SetActive(true);
        _currentQuestButton.gameObject.SetActive(false);

        foreach(GameObject questObject in _questList.SelectMany(Dict => Dict.Value))
        {
            questObject.SetActive(false);
        }

    }
    public void CloseCurrentQuestUI()
    {
        _currentQuestUIObject.SetActive(false);
        _currentQuestButton.gameObject.SetActive(true);

        foreach(GameObject questObject in _questList.SelectMany(Dict => Dict.Value))
        { 
            questObject.SetActive(true);
        }
    }

    //���콺 ����ȿ��On
    public void QuestDisableEffectOn(GameObject target)
    {
        if (_currentQuestUIObject.activeSelf)
            return;

        Vector3 curtainPos = _curtainPanel.transform.position;
        curtainPos.z = (float)ZLayer.Curtain;
        _curtainPanel.transform.position = curtainPos;

        if (target == null)
            return;

        Vector3 targetPos = target.transform.position;
        targetPos.z = (float)ZLayer.Highlight * _layerOffset;
        target.transform.position = targetPos;
    }

    //���콺 ���� ȿ�� Off
    public void QuestDisableEffectOff()
    {
        if (_currentQuestUIObject.activeSelf)
            return;

        Vector3 curtainPos = _curtainPanel.transform.position;
        curtainPos.z = (float)ZLayer.QuestFloor1 * _layerOffset * LAYER_OFFSET_MULTIPLIER;
        _curtainPanel.transform.position = curtainPos;

        foreach (var quests in _questList.Values)
        {
            foreach (GameObject questObj in quests)
            {
                Quest quest = questObj.GetComponent<Quest>();
                if (quest.IsDisable)
                    continue;

                Vector3 originPos = questObj.transform.position;
                originPos.z = quest.OriginZ;
                questObj.transform.position = originPos;
            }
        }
    }

    //��ư Off
    public void DisableOpenButtons()
    {
        foreach (var quests in _questList.Values)
        {
            foreach (GameObject questObj in quests)
            {
                questObj.GetComponent<Quest>().DisableOpenButton();
            }
        }

        //��ü ��
        QuestDisableEffectOn(null);
        _CanActiveSelectEffect = false;
    }
    //��ư On
    public void EnableOpenButtons()
    {
        foreach (var quests in _questList.Values)
        {
            foreach (GameObject questObj in quests)
            {
                questObj.GetComponent<Quest>().EnableOpenButton();
            }
        }

        QuestDisableEffectOff();
        _CanActiveSelectEffect = true;
    }

    public void SetQuestResult(Quest quest, bool value)
    {
        if (_questResultDict.ContainsKey(quest))
            _questResultDict[quest] = value;
        else
        {
            Debug.LogWarning("Quest Not Found.");
            _questResultDict.Add(quest, value);
        }
    }

    //������ �Ƿڰ� ���� ������ / �̺��������� ���߿��ϳ��� �����ϴ� �Լ�
    private bool SelectPossess(int acceptableCount,int unacceptableCount)
    {
        bool selectAcceptable = (UnityEngine.Random.value > 0.5f);

        if (_unAcceptableQuestList.Count == 0) return true;
        else if (_acceptableQuestList.Count == 0) return false;

        int maxAcceptableCount = Convert.ToInt32(_maxQuestCnt * 0.65f);
        int maxUnacceptableCount = _maxQuestCnt - maxAcceptableCount;

        if (selectAcceptable)
        {
            //65% ��ä������ �״��
            if (acceptableCount >= maxAcceptableCount) selectAcceptable = !selectAcceptable;
        }
        else if (!selectAcceptable)
        {
            if (unacceptableCount >= maxUnacceptableCount) selectAcceptable = !selectAcceptable;
        }
        return selectAcceptable;
    }

    //���� �����ǿ� �ش��ϴ� �Ƿڰ� ��ü �Ƿ��� 65% �̻� �����ϵ��� �������� �Ƿ� ID�� ������ �Լ�
    private int GetQuestID(ref int acceptableCount, ref int unacceptableCount)
    {
        int questID = 0;

        bool selectAcceptable = SelectPossess(acceptableCount,unacceptableCount);

        if (selectAcceptable)
        {
            acceptableCount++;
            int rndID = UnityEngine.Random.Range(0, _acceptableQuestList.Count - 1);
            questID = _acceptableQuestList[rndID];
        }
        else
        {
            unacceptableCount++;
            int rndID = UnityEngine.Random.Range(0, _unAcceptableQuestList.Count - 1);
            questID = _unAcceptableQuestList[rndID];
        }

        return questID;
    }
}
