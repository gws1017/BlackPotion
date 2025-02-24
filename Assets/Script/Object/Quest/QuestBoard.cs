using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class QuestBoard : MonoBehaviour
{
    //UI
    [SerializeField]
    private Button _currentQuestButton;
    [SerializeField]
    private Button _curretnQuestUIHidePannel;

    [ReadOnly, SerializeField]
    private List<GameObject> _questList;
    //������ �Ƿ� ���
    
    [SerializeField]
    private List<Quest> _acceptQuestList;

    //���� ���� ����� �Ƿں��� �����ϴ� Dictionary
    public Dictionary<Quest, bool> _questResultDict;

    //��� �Ƿ��� �������� �����Ǹ� �������� ���� ���� / ���� �Ұ� ����� �����ϱ� ���� List
    private List<int> _acceptableQuestList;
    private List<int> _unAcceptableQuestList;

    //�Ƿ� ������
    [SerializeField]
    private GameObject _questPrefab;
    //���� �Ƿ� ��ư�� ������ Ȱ��ȭ�Ǵ� UI
    [SerializeField]
    private GameObject _currentQuestUIObject;

    //�Ƿ� ������ ȸ�� �� �⺻ 30
    [SerializeField]
    private float _zRotRandRange;
    //�Ƿ� �Խ��ǿ� �ٴ� �ִ� �Ƿ� ��
    [SerializeField]
    private int _maxQuestCnt;
    //�ִ�� ���������� �Ƿ� ��
    [SerializeField]
    private int _maxAcceptQuestCnt;
    //�� �࿡ ���� �ϴ� �Ƿ� ��
    [SerializeField]
    private int _rowCnt;
    //�Ƿ� ���콺 ���� ȿ���� ���Ǵ� ����
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
        IntitilizeQuestBoard();
        CreateQuestObject();
    }


    public void IntitilizeQuestBoard()
    {
        Debug.Log("����Ʈ���� �ʱ�ȭ");

        //2������ ��� ���� �Ƿڸ� �����ϰ� ���� �����Ѵ�.
        if (_questList != null)
        {
            foreach (GameObject _quest in _questList)
                Destroy(_quest);

        }
        if(_acceptQuestList != null)
        {
            foreach(Quest obj in _acceptQuestList)
                Destroy(obj.gameObject);
        }

        //������ �ʱ�ȭ
        _currentQuestUIObject.SetActive(false);
        if(_questList == null || _questList.Count > 0)
        _questList = new List<GameObject>();
        if(_acceptQuestList == null || _acceptQuestList.Count > 0)
        _acceptQuestList = new List<Quest>();
        
        if(_questResultDict == null || _questResultDict.Count > 0)
        _questResultDict = new Dictionary<Quest, bool>();

        _currentQuestButton.onClick.RemoveAllListeners();
        _curretnQuestUIHidePannel.onClick.RemoveAllListeners();
        _currentQuestButton.onClick.AddListener(OpenAcceptQuestUI);
        _curretnQuestUIHidePannel.onClick.AddListener(CloseAcceptQuestUI);

        //��ü �����ǿ��� ����/ �̺��� ����Ʈ�� ������ �����´�
        GameManager.GM.PlayInfomation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);
    }

    public void CreateQuestObject()
    {
        GameManager.GM.PlayInfomation.SplitQuest(out _acceptableQuestList, out _unAcceptableQuestList);
        int pCnt = 0, upCnt = 0;
        //�Ƿ� ��ü�� ������
        int col = _maxQuestCnt / _rowCnt;
        for (int j = 0; j < col; ++j)
        {
            for (int i = 0; i < _rowCnt; ++i)
            {
                Vector3 pos;
                Quaternion rot;
                GenarateRandomPositionAndRotation(out pos, out rot);

                int id = GetQuestID(ref pCnt, ref upCnt);

                var Clone = Instantiate(_questPrefab, pos, rot);
                var quest = Clone.GetComponent<Quest>();
                var questCanvas = Clone.GetComponentInChildren<Canvas>();
                quest.QuestID = id;
                quest.OriginZ = pos.z;
                questCanvas.overrideSorting = true;
                //questCanvas.sortingOrder = Mathf.RoundToInt(pos.z * -10);
                _questList.Add(Clone);
            }
        }
    }

    private void GenarateRandomPositionAndRotation(out Vector3 Position , out Quaternion Rotation)
    {
        var cam = GameManager.GM.MainCamera;
        Bounds bounds = cam.GetComponentInChildren<Collider2D>().bounds;
        float randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        float randZ = UnityEngine.Random.Range(0f, 9f);
        Position = new Vector3(randX, randY, randZ);
       
        float randRotZ = UnityEngine.Random.Range(-_zRotRandRange, _zRotRandRange);
        Rotation = Quaternion.Euler(0f, 0f, randRotZ);
    }

    //�Ƿ� �����ϴ� �Լ�
    public void AcceptQuest(Quest questObject)
    {
        if (_acceptQuestList.Contains(questObject))
        {
            Debug.Log("�̹� ������ ����Ʈ �Դϴ�.");
            return;
        }
        if (_acceptQuestList.Count < _maxAcceptQuestCnt)
        {
            _acceptQuestList.Add(questObject);
            _questResultDict.Add(questObject, false);

            _questList.Remove(questObject.gameObject);
            questObject.IsDisable = true;

            //������ �Ƿڴ� ���� �Ƿ� UI�� �̵��ȴ�.
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
            Debug.Log("����Ʈ�� �ִ�ġ�� �����߽��ϴ�.");
            //���� �Ƿ� ����Ʈ���߰��ϱ�
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

    //���콺 ������ ������Ʈ�� �����ϰ� �������� ��Ӱ� ���̰Ը���
    public void QuestDisableEffectOn(GameObject gameObject)
    {
        if (_currentQuestUIObject.activeSelf)
            return;
        foreach (GameObject go in _questList)
        {
            if (go != gameObject)
            {
                Vector3 originPos = go.transform.position;
                originPos.z += 20f;
                go.transform.position = originPos;
                go.transform.position = originPos;
            }
        }
    }
    //���콺 ���� ȿ���� ������
    public void QuestDisableEffectOff()
    {
        if (_currentQuestUIObject.activeSelf)
            return;
        foreach (GameObject go in _questList)
        {
            if (go.GetComponent<Quest>().IsDisable) continue;
            Vector3 originPos = go.transform.position;
            originPos.z = go.GetComponent<Quest>().OriginZ;
            go.transform.position = originPos;
        }
    }

    //�� ����Ʈ ���½� �ٸ� ��ư �Է��� ����
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
    //�� ����Ʈ ���� �� ��ư �Է� ����
    public void EnableOpenButtons()
    {
        foreach (GameObject go in _questList)
        {
            go.GetComponent<Quest>().EnableOpenButton();
        }
        QuestDisableEffectOff();
        _CanActiveSelectEffect = true;
    }

    //����Ʈ ����� �����ϴ� �Լ�, �ܺο��� ȣ��ȴ�
    public void SetQuestResult(Quest quest, bool value)
    {
        _questResultDict[quest] = value;
    }

    //������ �Ƿڰ� ���� ������ / �̺��������� ���߿��ϳ��� �����ϴ� �Լ�
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
    
    //���� �����ǿ� �ش��ϴ� �Ƿڰ� ��ü �Ƿ��� 65% �̻� �����ϵ��� �������� �Ƿ� ID�� ������ �Լ�
    private int GetQuestID(ref int pCnt, ref int upCnt)
    {
        //���� �Ƿ�ID
        int questID = 0;

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
