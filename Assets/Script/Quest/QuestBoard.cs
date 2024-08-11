using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField]
    private int _maxQuestCnt;
    [SerializeField]
    private int _maxAcceptQuestCnt;
    [SerializeField]
    private int _rowCnt;


    [SerializeField]
    private GameObject _questPrefab;

    [ReadOnly,SerializeField]
    private GameObject[] _questList;

    [ReadOnly]
    public List<Quest> _accpetQuestList;

    public int MaxAcceptQuestCount
    {
        get { return _maxAcceptQuestCnt; }
    }

    void Start()
    {
        _questList = new GameObject[_maxQuestCnt];
        _accpetQuestList = new List<Quest>();

        Vector3 pos = new Vector3(-22f, 5f, 0f);
        int col = _maxQuestCnt / _rowCnt;
        for(int j =0; j < col; ++j)
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

    public void DisableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().DisableOpenButton();
        }
    }
    public void EnableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().EnableOpenButton();
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
        }
        else
        {
            Debug.Log("퀘스트를 최대치로 수락했습니다.");
            //현재 의뢰 리스트에추가하기
        }
    }
}
