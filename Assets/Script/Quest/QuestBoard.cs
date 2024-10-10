using System;
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

    private List<int> _acceptableQuestList;
    private List<int> _unAcceptableQuestList;

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

        //전체 레시피에서 보유/ 미보유 리스트를 나눠서 가져온다
        GameManager.GM._playInfo.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);
        int pCnt = 0, upCnt = 0;

        Vector3 pos = new Vector3(-22f, 5f, 0f);
        int col = _maxQuestCnt / _rowCnt;
        for (int j = 0; j < col; ++j)
        {
            for (int i = 0; i < _rowCnt; ++i)
            {
                int id = GetQuestID(ref pCnt, ref upCnt);
                int index = i + j * _rowCnt;
                var Clone = Instantiate(_questPrefab, pos, Quaternion.identity);
                Clone.GetComponent<Quest>().QuestID = id;
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
            if (go.GetComponent<Quest>().IsDisable) continue;
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

    private bool SelectPossess(int pCnt, int upCnt)
    {
        //true == possess false == unpossess
        bool possess = (UnityEngine.Random.value > 0.5f);

        //모든 레시피 보유했거나 어떠한레시피도 없으면 더 확인할 이유가 없다
        if (_unAcceptableQuestList.Count == 0 ) return true;
        else if(_acceptableQuestList.Count == 0 ) return false;

        int maxPCnt = Convert.ToInt32(_maxQuestCnt * 0.65);
        int maxUpCnt = _maxQuestCnt - maxPCnt;

        if (possess)
        {
            //65% 못채웠으면 그대로
            if(pCnt >= maxPCnt) possess = !possess;
        }
        else if(!possess)
        {
            //위와 동일
            if (upCnt >= maxUpCnt) possess = !possess;
        }
        return possess;
    }
    private int GetQuestID(ref int pCnt, ref int upCnt ) 
    {
        //최종 의뢰ID
        int questID = 0;

        //보유 미보유중 하나를 선택

        //전체 퀘스트의 65%까지만 보유퀘스트를 뽑을 수 있다.

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
