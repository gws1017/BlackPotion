using System;
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
    private Dictionary<int, List<BuffObject>> _currentBuffList;

    [SerializeField]
    private GameObject _buffUIPrefab;
    [SerializeField]
    private Button _buffListButton;
    [SerializeField]
    private GameObject _buffListUI;

    public const int MAX_BUFF_COUNT = 3;

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

    public List<int> GetCurrentBuffList()
    {
        return _currentBuffList.Keys.ToList();
    }
    void Start()
    {
        InitializeBuffList();
    }

    private void InitializeBuffList()
    {
        _currentBuffList = new Dictionary<int, List<BuffObject>>();
        _buffListButton.onClick.RemoveAllListeners();
        _buffListButton.onClick.AddListener(ToggleBuffListUI);

        //Test Code
        //AddBuff(4001);
        //AddBuff(4001);
        //AddBuff(4002);
    }

    public void ToggleBuffListUI()
    {
        _buffListUI.SetActive(!_buffListUI.activeSelf);
        InitializeBuffUI();
    }

    private int GetBuffCount()
    {
        int ret = 0;
        foreach (var buffObject in _currentBuffList.Values)
        {
            ret += buffObject.Count;
        }
        return ret;
    }

    private void InitializeBuffUI()
    {
        //RectTransform listAnchor = _buffListUI.GetComponent<RectTransform>();
        int buffCount = GetBuffCount();
        //listAnchor.offsetMin = new Vector2( 40 + (buffCount - 1) * -20,0);

        //����1���̻� ���� �� ��ġ����
        if (buffCount < 1) return;
        int index = 0;
        foreach (var buffObjectList in _currentBuffList.Values)
        {
            int startX = -40;
            int startY = -10;
            foreach (var buffObject in buffObjectList)
            {
                RectTransform anchor = buffObject.buffUI.GetComponent<RectTransform>();
                int offsetX = 40 * (index++);
                anchor.anchoredPosition = new Vector2(startX + offsetX, startY);
            }
        }
    }

    public void ResetBuffList()
    {
        foreach (var buffObjectList in _currentBuffList.Values)
        {
            foreach (var buffObject in buffObjectList)
            {
                Destroy(buffObject.buffUI);
            }
        }
        _currentBuffList.Clear();
        InitializeBuffList();
    }

    //���� Type�� ����ϸ� BuffId�� ����Ǿ ���⼭�� �����ϸ�ȴ�
    //������ ���� ������ ��ġ���� �Ķ���ͷ� �Է��Ѵ�
    public void CheckBuff(BuffType type, ref int value)
    {
        switch (type)
        {
            case BuffType.EvenOddNumber:
                //¦�� ����
                if (IsActiveBuff(4001) && !IsActiveBuff(4002))
                {
                    if (value % 2 == 0) break;
                    //���� 1�̸� -> 2 �׿ܴ� -1 �ؼ� ¦���� ����
                    if (value == 1) value = 2;
                    else value -= 1;
                }
                //Ȧ�� ����
                else if (IsActiveBuff(4002) && !IsActiveBuff(4001))
                {
                    if (value % 2 == 1) break;
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
                    value += UnityEngine.Random.Range(1, IngredientSlot.MAX_NUMBER);
                }
                else return;
                break;

        }
        //��� �� �����Ѵ�
        RemoveUsedBuff();
    }
    public bool IsFullBuffList()
    {
        if (GetBuffCount() >= MAX_BUFF_COUNT)
        {
            Debug.Log("���� ������ �ִ�� �������Դϴ� " + MAX_BUFF_COUNT);
            return true;
        }
        return false;
    }
    public void AddBuff(int id)
    {
        if (IsFullBuffList()) return;

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
        buffObject.buffUI.GetComponentInChildren<Button>().onClick.AddListener(() => ActivateBuff(buffObject));

        buffObject.id = id;
        buffObject.isActive = false;
        buffObject.count = 1;
        if (_currentBuffList.ContainsKey(id) == false)
        {
            _currentBuffList.Add(id, new List<BuffObject>());
        }
        _currentBuffList[id].Add(buffObject);

        InitializeBuffUI();
    }

    public void RemoveBuff(BuffObject buffObject)
    {
        if (buffObject == null) return;
        int id = buffObject.id;
        if (_currentBuffList.ContainsKey(id))
        {
            _currentBuffList[id].Remove(buffObject);
            buffObject.buffUI.GetComponentInChildren<Button>().interactable = true;
            buffObject.isActive = false;

            Destroy(buffObject.buffUI);
            if (_currentBuffList[id].Count == 0)
                _currentBuffList.Remove(id);

        }
    }

    public bool IsActiveBuff(int buffID = -1)
    {
        if (buffID == -1)
        {
            foreach (var buffList in _currentBuffList.Values)
            {
                foreach(var buff in buffList)
                {
                    if (buff.isActive == true) return true;
                }
            }
            return false;
        }
        else
        {
            if(_currentBuffList.ContainsKey(buffID))
            {
                foreach (var buff in _currentBuffList[buffID])
                {
                    if (buff.isActive == true) return true;
                }
            }
            return false;
        }
    }

    public bool ActivateBuff(BuffObject buffObject)
    {
        //������ �ѹ��� �Ѱ��� Ȱ��ȭ ����
        if (IsActiveBuff() == true) return false;

        buffObject.isActive = true;
        buffObject.buffUI.GetComponentInChildren<Button>().interactable = false;
        //_currentBuffList[id].buffUI.GetComponentInChildren<Text>().text = "������";
        return true;

    }

    //�Ƿ� �ϳ��� ����Ǹ� ȣ��ȴ�
    public void RemoveUsedBuff()
    {
        List<BuffObject> removeBuffList = new List<BuffObject>();
        foreach (var list in _currentBuffList.Values)
        {
            foreach (var buffObject in list)
            {
                if (buffObject.isActive)
                    removeBuffList.Add(buffObject);
            }

        }

        foreach (var buff in removeBuffList)
        {
            RemoveBuff(buff);
        }

        InitializeBuffUI();
    }

}
