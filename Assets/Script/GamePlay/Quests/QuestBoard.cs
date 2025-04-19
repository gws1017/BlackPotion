using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoard : MonoBehaviour
{
    private const float LAYER_OFFSET_MULTIPLIER = 2.5f;
    public enum ZLayer
    {
        QuestFloor1 = 3,
        QuestFloor2 = 2,
        QuestFloor3 = 1,
        Curtain = 0,
        Highlight = -1
    }

    [Header("UI")]
    [SerializeField] private GameObject _curtainPanel;
    [ReadOnly] public float _layerOffset;
    [ReadOnly] public bool _CanActiveSelectEffect = true;

    [Header("Quest")]
    [ReadOnly, SerializeField] private Dictionary<ZLayer, List<GameObject>> _questList;
    [SerializeField] private List<Quest> _acceptQuestList;
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private Vector3 _questScale = new Vector3(7.5f, 7.5f, 2.5f);
    public Dictionary<Quest, bool> _questResultDict;
    private List<int> _acceptableQuestList; //보유 레시피
    private List<int> _unAcceptableQuestList; //미보유 레시피

    [Header("Current Quest")]
    [SerializeField] public Button _questStartButton;
    [SerializeField] private Button _questBoardButton;
    [SerializeField] private Button _questCanelButton;
    [SerializeField] private Button _questNextButton;
    [SerializeField] private Button _currentQuestButton;
    [SerializeField] private Outline _buttonActiveOuline;
    [SerializeField] private GameObject _currentQuestUIObject;
    [SerializeField] private float _oulineEffectSpeed = 1;
    [SerializeField] private float _currentQuestPositionX = 1.5f;
    [SerializeField] private Quaternion _currentQuestRotation = Quaternion.Euler(0, 0, -10);
    [SerializeField] private Vector3 _currentQuestScale = new Vector3(10f, 10f, 2.5f);

    [Header("Quest Create Setting")]
    [SerializeField] private int _maxLoopCount;//충돌 검사 횟수
    [SerializeField] private float _zRotRandRange; //Quest 회전 범위
    [SerializeField] private int _maxQuestCnt; //의뢰 최대 생성수
    private Vector3 _meshExtents;

    //Getter Setter
    public GameObject QuestPrefab { get => _questPrefab; }

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
        //기존 오브젝트 삭제
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

        //변수 초기화
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
        _questNextButton.onClick.AddListener(NextCurrentQuest);
        _questCanelButton.onClick.AddListener(QuestCancel);
        _questBoardButton.onClick.AddListener(CloseCurrentQuestUI);

        QuestDisableEffectOff(); //커튼 패널 초기위치 조정
    }

    public void CreateQuestObject()
    {
        //레시피 나누기
        GameManager.GM.PlayInformation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);

        foreach (ZLayer layer in Enum.GetValues(typeof(ZLayer)))
        {
            if (layer == ZLayer.Curtain || layer == ZLayer.Highlight) continue;
            _questList[layer] = new List<GameObject>();
        }

        int acceptableCount = 0, unacceptableCount = 0;

        //의뢰 객체를 생성
        for (int i = 0; i < _maxQuestCnt; ++i)
        {
            Vector3 pos;
            Quaternion rot;

            ZLayer selectedlayer = ZLayer.QuestFloor3;
            foreach (ZLayer layer in new[] { ZLayer.QuestFloor3, ZLayer.QuestFloor2, ZLayer.QuestFloor1 })
            {
                if (_questList[layer].Count < Constants.MAX_QUEST_COUNT_LAYER)
                    selectedlayer = layer;
            }

            GenerateRandomPositionAndRotation(selectedlayer, out pos, out rot);

            //생성 현황 확인하면서 ID 생성
            int questId = GetQuestID(ref acceptableCount,ref unacceptableCount);

            GameObject Clone = Instantiate(_questPrefab, pos, rot);
            Quest quest = Clone.GetComponent<Quest>();
            Canvas questCanvas = Clone.GetComponentInChildren<Canvas>();

            quest.QuestID = questId;
            quest.OriginPosition = pos;
            quest.OriginRotation = rot;
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
            Debug.Log("이미 수락한 퀘스트 입니다.");
            return;
        }
        if (_acceptQuestList.Count >= Constants.MAX_ACCEPT_QUEST_COUNT)
        {
            Debug.Log("퀘스트를 최대치로 수락했습니다.");
            return;
        }

        _acceptQuestList.Add(quest);
        if(!_questResultDict.ContainsKey(quest))
            _questResultDict.Add(quest, false);

        if (_acceptQuestList.Count >= Constants.MAX_ACCEPT_QUEST_COUNT)
            CurrentQuestOutlineEffectOn();
        else
            CurrentQuestOutlineEffectOff();

        if (_questList.TryGetValue(quest.QuestLayer, out List<GameObject> questLayerList))
        {
            questLayerList.Remove(quest.gameObject);
        }
        quest.DisableOpenButton();

        //수락한 의뢰는 현재 의뢰 UI로 이동
        float zSpacing = (AcceptQuestList.Count-1) * LAYER_OFFSET_MULTIPLIER;
        float zStartPos = (float)ZLayer.Highlight *_layerOffset * LAYER_OFFSET_MULTIPLIER;

        Vector3 position = new Vector3(0, 0, zStartPos + zSpacing);
        Quaternion rotation = Quaternion.identity;
        if (_acceptQuestList.Count > 1)
        {
            position.x = _currentQuestPositionX;
            rotation = _currentQuestRotation;
        }
        quest.gameObject.transform.SetParent(_currentQuestUIObject.transform);
        quest.transform.SetPositionAndRotation(position, rotation);
        quest.gameObject.transform.localScale = _currentQuestScale;

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

    public void QuestCancel()
    {
        if (AcceptQuestList.Count <= 0) return;

        if (AcceptQuestList.Count <= Constants.MAX_ACCEPT_QUEST_COUNT)
            CurrentQuestOutlineEffectOff();

        Quest quest = AcceptQuestList[0];
        GameObject questObject = quest.gameObject;

        ShiftCurrentQuest();

        AcceptQuestList.Remove(quest);
        _questList[quest.QuestLayer].Add(questObject);

        questObject.transform.SetParent(null);
        questObject.transform.SetLocalPositionAndRotation(quest.OriginPosition, quest.OriginRotation);
        questObject.transform.localScale = _questScale;
        questObject.SetActive(false);

        quest.EnableOpenButton();
    }

    public void NextCurrentQuest()
    {
        if (AcceptQuestList.Count <= 1) return;
        ShiftCurrentQuest();
    }

    private void ShiftCurrentQuest()
    {
        Quest lastQuest = AcceptQuestList.Last<Quest>();

        Vector3 lastPosition = lastQuest.gameObject.transform.position;
        Quaternion lastRotation = lastQuest.gameObject.transform.rotation;

        for (int i = AcceptQuestList.Count - 1; i > 0; --i)
        {
            Transform currentQuestTransform = AcceptQuestList[i].gameObject.transform;
            Transform prevQuestTransform = AcceptQuestList[i - 1].gameObject.transform;
            currentQuestTransform.position = prevQuestTransform.position;
            currentQuestTransform.rotation = prevQuestTransform.rotation;
        }
        Quest frontQuest = AcceptQuestList[0];
        frontQuest.gameObject.transform.position = lastPosition;
        frontQuest.gameObject.transform.rotation = lastRotation;

        AcceptQuestList.RemoveAt(0);
        AcceptQuestList.Add(frontQuest);

        Transform frontQuestTransform = AcceptQuestList[0].gameObject.transform;
        Vector3 frontPosition = frontQuestTransform.position;

        frontQuestTransform.position = frontPosition;
    }

    //마우스 오버효과On
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

    //마우스 오버 효과 Off
    public void QuestDisableEffectOff()
    {
        if (_currentQuestUIObject.activeSelf)
            return;

        Vector3 curtainPos = _curtainPanel.transform.position;
        curtainPos.z = (float)ZLayer.QuestFloor1 * _layerOffset * LAYER_OFFSET_MULTIPLIER;
        _curtainPanel.transform.position = curtainPos;

        if(_questList == null)
        {
            Debug.LogError("Quest List is null");
            return;
        }

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

    //버튼 Off
    public void DisableOpenButtons()
    {
        foreach (var quests in _questList.Values)
        {
            foreach (GameObject questObj in quests)
            {
                questObj.GetComponent<Quest>().DisableOpenButton();
            }
        }

        //전체 블러
        QuestDisableEffectOn(null);
        _CanActiveSelectEffect = false;
    }
    //버튼 On
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

    public void CurrentQuestOutlineEffectOff()
    {
        StopAllCoroutines();
        _buttonActiveOuline.effectColor = Color.clear;
    }
    public void CurrentQuestOutlineEffectOn()
    {
        StartCoroutine(BlinkCurrentQuestOutline());
    }
    IEnumerator BlinkCurrentQuestOutline()
    {
        while (true)
        {
            float alpha = 0;
            float direction = 1;

            while (true)
            {
                alpha += direction * _oulineEffectSpeed * Time.deltaTime * 255f;
                alpha = Mathf.Clamp(alpha, 0, 255);

                Color color = _buttonActiveOuline.effectColor;
                color.a = alpha / 255f;
                _buttonActiveOuline.effectColor = color;

                if (alpha >= 255) direction = -1; 
                else if (alpha <= 0) direction = 1;

                yield return null;
            }
        }
    }

    //정산용
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

    //가져올 의뢰가 보유 레시피 / 미보유레시피 둘중에하나를 결정하는 함수
    private bool SelectPossess(int acceptableCount,int unacceptableCount)
    {
        bool selectAcceptable = (UnityEngine.Random.value > 0.5f);

        if (_unAcceptableQuestList.Count == 0) return true;
        else if (_acceptableQuestList.Count == 0) return false;

        int maxAcceptableCount = Convert.ToInt32(_maxQuestCnt * 0.65f);
        int maxUnacceptableCount = _maxQuestCnt - maxAcceptableCount;

        if (selectAcceptable)
        {
            //65% 못채웠으면 그대로
            if (acceptableCount >= maxAcceptableCount) selectAcceptable = !selectAcceptable;
        }
        else if (!selectAcceptable)
        {
            if (unacceptableCount >= maxUnacceptableCount) selectAcceptable = !selectAcceptable;
        }
        return selectAcceptable;
    }

    //보유 레시피에 해당하는 의뢰가 전체 의뢰중 65% 이상 차지하도록 무작위로 의뢰 ID를 얻어오는 함수
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
