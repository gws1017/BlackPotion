using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuffObject
{
    public int id;
    public int count;
    public bool isActive;
    public GameObject buffUI;
}

public enum BuffType
{
    EvenOddNumber,
    PlusPowder,
    UpgradeBrew,
    StrangeBrew
}

public class BuffManager : MonoBehaviour
{


    [SerializeField]
    private Dictionary<int, BuffObject> _currentBuffList;

    [SerializeField]
    private GameObject _buffUIPrefab;
    [SerializeField]
    private Button _buffListButton;
    [SerializeField]
    private GameObject _buffListUI;

    //Getter
    public string GetNameFromBuffId(int id)
    {
        return ReadJson._dictBuff[id].buffName;
    }
    public int GetStateFromBuffId(int id)
    {
        return ReadJson._dictBuff[id].buffState;
    }
    public string GetExplainFromBuffId(int id)
    {
        return ReadJson._dictBuff[id].buffExplain;
    }

    void Start()
    {
        InitializeBuffList();
    }

    private void InitializeBuffList()
    {
        _currentBuffList = new Dictionary<int, BuffObject>();
        _buffListButton.onClick.RemoveAllListeners();
        _buffListButton.onClick.AddListener(ToggleBuffListUI);

        //Test Code
        AddBuff(4001);
        AddBuff(4002);
        AddBuff(4003);
    }

    public void ToggleBuffListUI()
    {
        _buffListUI.SetActive(!_buffListUI.activeSelf);
        InitializeBuffUI();
    }

    private void InitializeBuffUI()
    {
        //�ߺ� ���� ������ ������ �ڵ� �߰� �ٶ�
        RectTransform listAnchor = _buffListUI.GetComponent<RectTransform>();
        int buffCount = _currentBuffList.Count;
        listAnchor.offsetMin = new Vector2(0, 40 + (buffCount - 1) * -20);

        //����1���̻� ���� �� ��ġ����
        if (buffCount < 1) return;
        int index = 0;
        foreach (var buffObject in _currentBuffList.Values)
        {
            RectTransform anchor = buffObject.buffUI.GetComponent<RectTransform>();
            int startY = (buffCount - 1) * 10;
            int offsetY = -25 * (index++);
            anchor.anchoredPosition = new Vector2(0, startY + offsetY);
        }
    }

    public void ResetBuffList()
    {
        foreach (var buffObject in _currentBuffList.Values)
        {
            buffObject.isActive = false;
        }
    }

    //���� Type�� ����ϸ� BuffId�� ����Ǿ ���⼭�� �����ϸ�ȴ�
    //������ ���� ������ ��ġ���� �Ķ���ͷ� �Է��Ѵ�
    public void CheckBuff(BuffType type , ref int value)
    {
        switch(type)
        {
            case BuffType.EvenOddNumber:
                //¦�� ����
                if (IsActiveBuff(4001) && !IsActiveBuff(4002))
                {
                    if (value % 2 == 0) return;
                    //���� 1�̸� -> 2 �׿ܴ� -1 �ؼ� ¦���� ����
                    if (value == 1) value = 2;
                    else value -= 1;
                }
                //Ȧ�� ����
                else if (IsActiveBuff(4002) && !IsActiveBuff(4001))
                {
                    if (value % 2 == 1) return;
                    value -= 1;
                }
                else return;
                break;
            case BuffType.PlusPowder:
                //���� ����
                if (IsActiveBuff(4003))
                {
                    value += ReadJson._dictBuff[4003].buffState;
                }
                else return;
                break;
            case BuffType.UpgradeBrew:
                //������ ��ȭ
                if (IsActiveBuff(4004))
                {
                    value += ReadJson._dictBuff[4003].buffState;
                }
                else return;
                break;
            case BuffType.StrangeBrew:
                //�̻��� ������
                if (IsActiveBuff(4005))
                {
                    value += Random.Range(1, Slot.MAX_NUMBER);
                }
                else return;
                break;

        }
        //��� �� �����Ѵ�
        RemoveUsedBuff();
    }

    public void AddBuff(int id)
    {
        if (_currentBuffList.ContainsKey(id) == false)
        {
            BuffObject buffObject = new BuffObject();

            //����UI(������, ����ư) ������ ����
            buffObject.buffUI = Instantiate(_buffUIPrefab, Vector3.zero, Quaternion.identity);
            //���� ����Ʈ UI������ ����
            buffObject.buffUI.transform.SetParent(_buffListUI.transform, false);
            //���� �̹��� ����
            buffObject.buffUI.GetComponentInChildren<Image>().sprite
                = Resources.Load<Sprite>(ReadJson._dictBuff[id].buffImage);
            buffObject.buffUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            //���� Ȱ��ȭ ��ư ����
            //�Ķ���� ���� �ÿ��� ���ٸ� ����϶�
            buffObject.buffUI.GetComponentInChildren<Button>().onClick.AddListener(() => ActivateBuff(id));

            buffObject.id = id;
            buffObject.isActive = false;

            _currentBuffList.Add(id, buffObject);
        }
        else
            _currentBuffList[id].count += 1;

        InitializeBuffUI();
    }

    public void RemoveBuff(int id)
    {
        if (_currentBuffList.ContainsKey(id))
        {
            if (_currentBuffList[id].count>1)
            {
                _currentBuffList[id].count -= 1;
            }
            else
            {
                Destroy(_currentBuffList[id].buffUI);
                _currentBuffList.Remove(id);
            }
        }
    }

    public bool IsActiveBuff(int id = -1)
    {
        if (id == -1)
        {
            foreach (var buff in _currentBuffList.Values)
            {
                if (buff.isActive == true) return true;
            }
            return false;
        }
        else if(_currentBuffList.ContainsKey(id))
        {
            return _currentBuffList[id].isActive;
        }
        return false;
    }

    public bool ActivateBuff(int id)
    {
        //������ �ѹ��� �Ѱ��� Ȱ��ȭ ����
        if (IsActiveBuff() == true) return false;

        if (_currentBuffList.ContainsKey(id))
        {
            _currentBuffList[id].isActive = true;
            _currentBuffList[id].buffUI.GetComponentInChildren<Button>().interactable = false;
            _currentBuffList[id].buffUI.GetComponentInChildren<Text>().text = "��� �Ϸ�";
            return true;
        }
        return false;
    }

    //�Ƿ� �ϳ��� ����Ǹ� ȣ��ȴ�
    public void RemoveUsedBuff()
    {
        List<int> RemoveBuffList = new List<int>();
        foreach (var buffID in _currentBuffList.Keys)
        {
            if (_currentBuffList[buffID].isActive == true)
                RemoveBuffList.Add(buffID);
        }
        foreach(var buffID in RemoveBuffList)
        {
            RemoveBuff(buffID);
        }
        InitializeBuffUI();
    }

    //��Ȱ��ȭ ������ ���� �� �����ϴ�. (����� ���ŵȴ�)
    //�̻�� �Լ�, ���� OFF Ȥ�� ��� ��ҽ� ���� �� �ִ�.
    public void DeactivateBuff(int id)
    {
        if (_currentBuffList.ContainsKey(id))
        {
            _currentBuffList[id].isActive = false;
        }
    }
}
