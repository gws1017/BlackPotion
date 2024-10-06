using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField]
    private int _maxQuestCnt;
    [SerializeField]
    private int _maxAcceptQuestCnt;
    [SerializeField]
    private int _rowCnt;
    [ReadOnly]
    public bool _CanActiveSelectEffect = true;


    [SerializeField]
    private GameObject _questPrefab;

    [ReadOnly, SerializeField]
    private GameObject[] _questList;

    [ReadOnly]
    public List<Quest> _accpetQuestList;

    public Dictionary<Quest, bool> _questResultDict;

    public int MaxAcceptQuestCount
    {
        get { return _maxAcceptQuestCnt; }
    }

    public int CurrrentAcceptQuestCnt
    {
        get { return _accpetQuestList.Count; }
    }

    void Start()
    {
        IntitilizeQuestBoard();
    }

    public void IntitilizeQuestBoard()
    {
        if(_questList != null)
        {
            foreach(GameObject _quest in _questList)
                Destroy(_quest);

        }
        _questList = new GameObject[_maxQuestCnt];
        _accpetQuestList = new List<Quest>();
        _questResultDict = new Dictionary<Quest, bool>();

        Vector3 pos = new Vector3(-22f, 5f, 0f);
        int col = _maxQuestCnt / _rowCnt;
        for (int j = 0; j < col; ++j)
        {
            for (int i = 0; i < _rowCnt; ++i)
            {
                int index = i + j * _rowCnt;
                var Clone = Instantiate(_questPrefab, pos, Quaternion.identity);
                _questList[index] = Clone;
                pos.x += 11f;
            }
            pos.x = -22f;
            pos.y -= 11f;
        }
    }

    public void OpenCurrentQuest()
    {
        Debug.Log("현재 수락한 퀘스트");
    }

    //상세 퀘스트 오픈시 호출되는 함수
    public void DisableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().DisableOpenButton();
        }
        //전체 블러
        QuestDisableEffectOn(null);
        _CanActiveSelectEffect = false;
    }
    //상세 퀘스트 닫을 시 호출 됨
    public void EnableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().EnableOpenButton();
        }
        QuestDisableEffectOff();
        _CanActiveSelectEffect = true;
    }
    //---------------------------------

    public void QuestDisableEffectOn(GameObject gameObject)
    {
        foreach(GameObject go in _questList)
        {
            if(go!= gameObject)
            {
                Vector3 originPos = go.transform.position;
                originPos.z = 10;
                go.transform.position = originPos;
                go.transform.position = originPos;
            }
        }
    }
    public void QuestDisableEffectOff()
    {
        foreach (GameObject go in _questList)
        {
            Vector3 originPos = go.transform.position;
            originPos.z = 0;
            go.transform.position = originPos;
        }
    }

    public void AcceptQuest(Quest questObject)
    {
        if(_accpetQuestList.Contains(questObject))
        {
            Debug.Log("이미 수락한 퀘스트 입니다.");
            return;
        }
        if (_accpetQuestList.Count < _maxAcceptQuestCnt)
        {
            _accpetQuestList.Add(questObject);
            _questResultDict.Add(questObject, false);
        }
        else
        {
            Debug.Log("퀘스트를 최대치로 수락했습니다.");
            //현재 의뢰 리스트에추가하기
        }
    }

    public void SetQuestResult(Quest quest, bool value)
    {
        _questResultDict[quest] = value;
    }
}
