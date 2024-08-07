using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField]
    private int _maxQuestCnt;
    [SerializeField]
    private int _rowCnt;


    [SerializeField]
    private GameObject _questPrefab;

    [ReadOnly,SerializeField]
    private GameObject[] _questList;
    void Start()
    {
        _questList = new GameObject[_maxQuestCnt];
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
