using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private GameObject _currentQuestUIObject;
    [SerializeField]
    private GameObject _questPrefab;
    [SerializeField]
    private Button _currentQuestButton;
    [SerializeField]
    private Button _curretnQuestUIHidePannel;

    [ReadOnly, SerializeField]
    private List<GameObject> _questList;

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
        if (_questList != null)
        {
            foreach (GameObject _quest in _questList)
                Destroy(_quest);

        }
        if(_accpetQuestList != null)
        {
            foreach(Quest obj in _accpetQuestList)
                Destroy(obj.gameObject);
        }
        _currentQuestUIObject.SetActive(false);
        _questList = new List<GameObject>();
        _accpetQuestList = new List<Quest>();
        _questResultDict = new Dictionary<Quest, bool>();

        _currentQuestButton.onClick.RemoveAllListeners();
        _curretnQuestUIHidePannel.onClick.RemoveAllListeners();
        _currentQuestButton.onClick.AddListener(ShowAcceptQuest);
        _curretnQuestUIHidePannel.onClick.AddListener(CloseAcceptQuestUI);

        //��ü �����ǿ��� ����/ �̺��� ����Ʈ�� ������ �����´�
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
                _questList.Add(Clone);
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
        if (_currentQuestUIObject.activeSelf)
            return;
        foreach (GameObject go in _questList)
        {
            if (go != gameObject)
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
        if (_currentQuestUIObject.activeSelf)
            return;
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
        if (_accpetQuestList.Contains(questObject))
        {
            Debug.Log("�̹� ������ ����Ʈ �Դϴ�.");
            return;
        }
        if (_accpetQuestList.Count < _maxAcceptQuestCnt)
        {
            _accpetQuestList.Add(questObject);
            _questResultDict.Add(questObject, false);

            _questList.Remove(questObject.gameObject);
            questObject.IsDisable = true;

            questObject.gameObject.transform.SetParent(_currentQuestUIObject.transform);
            int i = _accpetQuestList.Count - 1;
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
            Debug.Log("����Ʈ�� �ִ�ġ�� �����߽��ϴ�.");
            //���� �Ƿ� ����Ʈ���߰��ϱ�
        }
    }

    public void ShowAcceptQuest()
    {
        _currentQuestUIObject.SetActive(!_currentQuestUIObject.activeSelf);



    }
    public void CloseAcceptQuestUI()
    {
        _currentQuestUIObject.SetActive(false);
    }
    public void SetQuestResult(Quest quest, bool value)
    {
        _questResultDict[quest] = value;
    }

    private bool SelectPossess(int pCnt, int upCnt)
    {
        //true == possess false == unpossess
        bool possess = (UnityEngine.Random.value > 0.5f);

        //��� ������ �����߰ų� ��ѷ����ǵ� ������ �� Ȯ���� ������ ����
        if (_unAcceptableQuestList.Count == 0) return true;
        else if (_acceptableQuestList.Count == 0) return false;

        int maxPCnt = Convert.ToInt32(_maxQuestCnt * 0.65);
        int maxUpCnt = _maxQuestCnt - maxPCnt;

        if (possess)
        {
            //65% ��ä������ �״��
            if (pCnt >= maxPCnt) possess = !possess;
        }
        else if (!possess)
        {
            //���� ����
            if (upCnt >= maxUpCnt) possess = !possess;
        }
        return possess;
    }
    private int GetQuestID(ref int pCnt, ref int upCnt)
    {
        //���� �Ƿ�ID
        int questID = 0;

        //���� �̺����� �ϳ��� ����

        //��ü ����Ʈ�� 65%������ ��������Ʈ�� ���� �� �ִ�.

        bool possess = SelectPossess(pCnt, upCnt);

        //�������̰�,���� �������� 65%������ �ʾ�����, Ȥ�� ��緹���Ǹ� �������̶��
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
