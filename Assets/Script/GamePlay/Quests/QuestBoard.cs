using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class QuestBoard : MonoBehaviour
{
    public const int MAX_QUEST_COUNT = 5;
    public const float LAYER_OFFSET_MULTIPLIER = 2.5f;
    public enum ZLayer
    {
        QuestFloor1 = 3,
        QuestFloor2 = 2,
        QuestFloor3 = 1,
        Curtain = 0,
        HighLight = -1
    }

    [ReadOnly]
    public float _layerOffset;
    //UI
    [SerializeField]
    private GameObject _curtainPannel;
    [SerializeField]
    private Button _currentQuestButton;
    [SerializeField]
    private Button _curretnQuestUIHidePannel;

    [ReadOnly, SerializeField]
    //private List<GameObject> _questList;
    private Dictionary<ZLayer, List<GameObject>> _questList;
    //수락한 의뢰 목록
    
    [SerializeField]
    private List<Quest> _acceptQuestList;

    //포션 제조 결과를 의뢰별로 저장하는 Dictionary
    public Dictionary<Quest, bool> _questResultDict;

    //모든 의뢰중 보유중인 레시피를 기준으로 수락 가능 / 수락 불가 목록을 저장하기 위한 List
    private List<int> _acceptableQuestList;
    private List<int> _unAcceptableQuestList;

    //의뢰 프리펩
    [SerializeField]
    private GameObject _questPrefab;
    //현재 의뢰 버튼을 누르면 활성화되는 UI
    [SerializeField]
    private GameObject _currentQuestUIObject;

    [SerializeField]
    private int _maxLoopCount;
    //의뢰 무작위 회전 값 기본 30
    [SerializeField]
    private float _zRotRandRange;
    //의뢰 게시판에 붙는 최대 의뢰 수
    [SerializeField]
    private int _maxQuestCnt;
    //최대로 수락가능한 의뢰 수
    [SerializeField]
    private int _maxAcceptQuestCnt;
    //한 행에 나열 하는 의뢰 수
    [SerializeField]
    private int _rowCnt;
    //의뢰 마우스 오버 효과에 사용되는 변수
    [ReadOnly]
    public bool _CanActiveSelectEffect = true;
    public GameObject QuestPrefab
    {
        get { return _questPrefab; }
    }

    public int MaxAcceptQuestCount
    {
        get { return _maxAcceptQuestCnt; }
    }
  
    public int CurrrentAcceptQuestCnt
    {
        get { return _acceptQuestList.Count; }
    }

    public List<Quest> AcceptQuestList
    {
        get { return _acceptQuestList; }
        set
        {
            Debug.Log(_acceptQuestList.Count);
            AcceptQuestList = value;
        }
    }
    public Quest GetCurrentQuest(int id)
    {
       id = Mathf.Clamp(id, 0, AcceptQuestList.Count-1);
       return AcceptQuestList[id];
    }

    void Start()
    {
        Vector3 extent = _questPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
        extent.Scale(_questPrefab.transform.localScale);
        _layerOffset = extent.z;
        InitilizeQuestBoard();
        CreateQuestObject();

    }


    public void InitilizeQuestBoard()
    {
        Debug.Log("퀘스트보드 초기화");

        //2일차의 경우 기존 의뢰를 삭제하고 새로 생성한다.
        if (_questList != null)
        {
            foreach (var _quests in _questList.Values)
            {
                foreach(var _quest in _quests)
                {
                    Destroy(_quest);
                }
            }

        }
        if(_acceptQuestList != null)
        {
            foreach(Quest obj in _acceptQuestList)
                Destroy(obj.gameObject);
        }

        //변수들 초기화
        _currentQuestUIObject.SetActive(false);
        if(_questList == null || _questList.Count > 0)
        _questList = new Dictionary<ZLayer, List<GameObject>>();
        if(_acceptQuestList == null || _acceptQuestList.Count > 0)
        _acceptQuestList = new List<Quest>();
        
        if(_questResultDict == null || _questResultDict.Count > 0)
        _questResultDict = new Dictionary<Quest, bool>();

        _currentQuestButton.onClick.RemoveAllListeners();
        _curretnQuestUIHidePannel.onClick.RemoveAllListeners();
        _currentQuestButton.onClick.AddListener(OpenAcceptQuestUI);
        _curretnQuestUIHidePannel.onClick.AddListener(CloseAcceptQuestUI);

        //전체 레시피에서 보유/ 미보유 리스트를 나눠서 가져온다
        GameManager.GM.PlayInformation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);
    }

    public void CreateQuestObject()
    {
        GameManager.GM.PlayInformation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);
        int pCnt = 0, upCnt = 0;
        //의뢰 객체를 생성함
        int col = _maxQuestCnt / _rowCnt;
        for (int j = 0; j < col; ++j)
        {
            for (int i = 0; i < _rowCnt; ++i)
            {
                Vector3 pos;
                Quaternion rot;

                ZLayer layer = ZLayer.QuestFloor3;
                for(ZLayer k = ZLayer.QuestFloor3; k <= ZLayer.QuestFloor1; ++k)
                {
                    if (_questList.ContainsKey(k))
                    {
                        if (_questList[k].Count < MAX_QUEST_COUNT)
                            layer = k;
                    }
                    else _questList.Add(k,new List<GameObject>());
                }

                GenarateRandomPositionAndRotation(layer, out pos, out rot);

                int id = GetQuestID(ref pCnt, ref upCnt);

                var Clone = Instantiate(_questPrefab, pos, rot);
                var quest = Clone.GetComponent<Quest>();
                var questCanvas = Clone.GetComponentInChildren<Canvas>();
                quest.QuestID = id;
                quest._originZ = pos.z;
                quest.QuestLayer = layer;
                questCanvas.overrideSorting = true;
                //questCanvas.sortingOrder = Mathf.RoundToInt(pos.z * -10);
                _questList[layer].Add(Clone);
            }
        }
    }

    private void GenarateRandomPositionAndRotation(ZLayer layer, out Vector3 Position , out Quaternion Rotation)
    {
        var cam = GameManager.GM.MainCamera;
        Bounds bounds = cam.GetComponentInChildren<Collider2D>().bounds;
        
        Vector3 extent = _questPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
        extent.Scale(_questPrefab.transform.localScale);

        float randZ = (float)layer * _layerOffset* LAYER_OFFSET_MULTIPLIER;
        float randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        Position = new Vector3(randX, randY, randZ);

        Collider[] colliders = Physics.OverlapBox(Position, extent);
        int loopCount = 0;
        while(colliders.Length != 0)
        {
            if (loopCount > _maxLoopCount) break;
            loopCount++;
            randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            Position = new Vector3(randX, randY, randZ);
            colliders = Physics.OverlapBox(Position, extent);
        }

        float randRotZ = UnityEngine.Random.Range(-_zRotRandRange, _zRotRandRange);
        Rotation = Quaternion.Euler(0f, 0f, randRotZ);
    }

    //의뢰 수락하는 함수
    public void AcceptQuest(Quest questObject)
    {
        if (_acceptQuestList.Contains(questObject))
        {
            Debug.Log("이미 수락한 퀘스트 입니다.");
            return;
        }
        if (_acceptQuestList.Count < _maxAcceptQuestCnt)
        {
            _acceptQuestList.Add(questObject);
            _questResultDict.Add(questObject, false);

            _questList[questObject.QuestLayer].Remove(questObject.gameObject);
            questObject.IsDisable = true;

            //수락한 의뢰는 현재 의뢰 UI로 이동된다.
            questObject.gameObject.transform.SetParent(_currentQuestUIObject.transform);
            int i = _acceptQuestList.Count - 1;
            Vector3 start;
            if (i < 3)
            {
                start = new Vector3(-20, 4, -23);
                questObject.gameObject.transform.position = start + new Vector3(i * 20, 0, 0);
            }
            else
            {
                start = new Vector3(-10, -8, -23);
                questObject.gameObject.transform.position = start + new Vector3((i % 3) * 20, 0, 0);
            }

        }
        else
        {
            Debug.Log("퀘스트를 최대치로 수락했습니다.");
            //현재 의뢰 리스트에추가하기
        }
    }

    public void OpenAcceptQuestUI()
    {
        _currentQuestUIObject.SetActive(!_currentQuestUIObject.activeSelf);
    }
    public void CloseAcceptQuestUI()
    {
        _currentQuestUIObject.SetActive(false);
    }

    //마우스 오버된 오브젝트를 제외하고 나머지를 어둡게 보이게만듬
    public void QuestDisableEffectOn(GameObject go)
    {
        if (_currentQuestUIObject.activeSelf)
            return;

        Vector3 newPos = _curtainPannel.transform.position;
        newPos.z = (float)ZLayer.Curtain;
        _curtainPannel.transform.position = newPos;

        if (go == null) return;
        Vector3 objectPos = go.transform.position;
        objectPos.z = (float)ZLayer.HighLight * _layerOffset;
        go.transform.position = objectPos;
        //foreach (var quests in _questList.Values)
        //{
        //    foreach(var go in quests)
        //    {
        //        if (go != gameObject)
        //        {
        //            Vector3 originPos = go.transform.position;
        //            originPos.z += 20f;
        //            go.transform.position = originPos;
        //            go.transform.position = originPos;
        //        }
        //    }
        //}
    }

    //마우스 오버 효과를 해제함
    public void QuestDisableEffectOff()
    {
        if (_currentQuestUIObject.activeSelf)
            return;

        Vector3 newPos = _curtainPannel.transform.position;
        newPos.z = (float)ZLayer.QuestFloor1 * _layerOffset * LAYER_OFFSET_MULTIPLIER;
        _curtainPannel.transform.position = newPos;

        foreach (var quests in _questList.Values)
        {
            foreach (GameObject go in quests)
            {
                if (go.GetComponent<Quest>().IsDisable) continue;
                Vector3 originPos = go.transform.position;
                originPos.z = go.GetComponent<Quest>()._originZ;
                go.transform.position = originPos;
            }
        }
    }

    //상세 퀘스트 오픈시 다른 버튼 입력을 막음
    public void DisableOpenButtons()
    {
        foreach (var quests in _questList.Values)
        {
            foreach (GameObject go in quests)
            {
                go.GetComponent<Quest>().DisableOpenButton();
            }
        }
        //전체 블러
        QuestDisableEffectOn(null);
        _CanActiveSelectEffect = false;
    }
    //상세 퀘스트 닫을 시 버튼 입력 복구
    public void EnableOpenButtons()
    {
        foreach (var quests in _questList.Values)
        {
            foreach (GameObject go in quests)
            {
                go.GetComponent<Quest>().EnableOpenButton();
            }
        }
            
        QuestDisableEffectOff();
        _CanActiveSelectEffect = true;
    }

    //퀘스트 결과를 저장하는 함수, 외부에서 호출된다
    public void SetQuestResult(Quest quest, bool value)
    {
        _questResultDict[quest] = value;
    }

    //가져올 의뢰가 보유 레시피 / 미보유레시피 둘중에하나를 결정하는 함수
    private bool SelectPossess(int pCnt, int upCnt)
    {
        //true == possess false == unpossess
        bool possess = (UnityEngine.Random.value > 0.5f);

        //모든 레시피 보유했거나 어떠한레시피도 없으면 더 확인할 이유가 없다
        if (_unAcceptableQuestList.Count == 0) return true;
        else if (_acceptableQuestList.Count == 0) return false;

        int maxPCnt = Convert.ToInt32(_maxQuestCnt * 0.65);
        int maxUpCnt = _maxQuestCnt - maxPCnt;

        if (possess)
        {
            //65% 못채웠으면 그대로
            if (pCnt >= maxPCnt) possess = !possess;
        }
        else if (!possess)
        {
            //위와 동일
            if (upCnt >= maxUpCnt) possess = !possess;
        }
        return possess;
    }
    
    //보유 레시피에 해당하는 의뢰가 전체 의뢰중 65% 이상 차지하도록 무작위로 의뢰 ID를 얻어오는 함수
    private int GetQuestID(ref int pCnt, ref int upCnt)
    {
        //최종 의뢰ID
        int questID = 0;

        bool possess = SelectPossess(pCnt, upCnt);
        //보유중이고,뽑은 레시피의 65%를넘지 않았을때, 혹은 모든레시피를 보유중이라면
        if (possess)
        {
            pCnt++;
            int rndID = UnityEngine.Random.Range(0, _acceptableQuestList.Count - 1);
            questID = _acceptableQuestList[rndID];
        }
        else
        {
            upCnt++;
            int rndID = UnityEngine.Random.Range(0, _unAcceptableQuestList.Count - 1);
            questID = _unAcceptableQuestList[rndID];
        }

        return questID;
    }
}
