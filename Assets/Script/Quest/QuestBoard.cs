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
    [ReadOnly]
    public bool _CanActiveSelectEffect = true;


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

    public int CurrrentAcceptQuestCnt
    {
        get { return _accpetQuestList.Count; }
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
    public void OpenCurrentQuest()
    {
        Debug.Log("���� ������ ����Ʈ");
    }

    //�� ����Ʈ ���½� ȣ��Ǵ� �Լ�
    public void DisableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().DisableOpenButton();
        }
        //��ü ��
        QuestDisableEffectOn(null);
        _CanActiveSelectEffect = false;
    }
    //�� ����Ʈ ���� �� ȣ�� ��
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
            Debug.Log("�̹� ������ ����Ʈ �Դϴ�.");
            return;
        }
        if (_accpetQuestList.Count < _maxAcceptQuestCnt)
        {
            _accpetQuestList.Add(questObject);
        }
        else
        {
            Debug.Log("����Ʈ�� �ִ�ġ�� �����߽��ϴ�.");
            //���� �Ƿ� ����Ʈ���߰��ϱ�
        }
    }
}
