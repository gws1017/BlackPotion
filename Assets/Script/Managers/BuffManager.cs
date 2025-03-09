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

        //버프1개이상 있을 때 위치조정
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

    //버프 Type을 사용하면 BuffId가 변경되어도 여기서만 수정하면된다
    //버프로 인해 변동될 수치값을 파라미터로 입력한다
    public void CheckBuff(BuffType type, ref int value)
    {
        switch (type)
        {
            case BuffType.EvenOddNumber:
                //짝수 버프
                if (IsActiveBuff(4001) && !IsActiveBuff(4002))
                {
                    if (value % 2 == 0) break;
                    //원래 1이면 -> 2 그외는 -1 해서 짝수로 변경
                    if (value == 1) value = 2;
                    else value -= 1;
                }
                //홀수 버프
                else if (IsActiveBuff(4002) && !IsActiveBuff(4001))
                {
                    if (value % 2 == 1) break;
                    value -= 1;
                }
                else return;
                break;
            case BuffType.PlusPowder:
                //증가 가루
                if (IsActiveBuff(4003))
                {
                    value += ReadJson._dictBuff[4003].buffState;
                }
                else return;
                break;
            case BuffType.UpgradeBrew:
                //양조기 강화
                if (IsActiveBuff(4004))
                {
                    value += ReadJson._dictBuff[4003].buffState;
                }
                else return;
                break;
            case BuffType.StrangeBrew:
                //이상한 양조기
                if (IsActiveBuff(4005))
                {
                    value += UnityEngine.Random.Range(1, IngredientSlot.MAX_NUMBER);
                }
                else return;
                break;

        }
        //사용 후 제거한다
        RemoveUsedBuff();
    }
    public bool IsFullBuffList()
    {
        if (GetBuffCount() >= MAX_BUFF_COUNT)
        {
            Debug.Log("버프 아이템 최대로 보유중입니다 " + MAX_BUFF_COUNT);
            return true;
        }
        return false;
    }
    public void AddBuff(int id)
    {
        if (IsFullBuffList()) return;

        BuffObject buffObject = new BuffObject();

        //버프UI(아이콘, 사용버튼) 프리펩 생성
        buffObject.buffUI = Instantiate(_buffUIPrefab, Vector3.zero, Quaternion.identity);
        //버프 리스트 UI하위로 설정
        buffObject.buffUI.transform.SetParent(_buffListUI.transform, false);
        //버프 이미지 설정
        buffObject.buffUI.GetComponentInChildren<Image>().sprite
            = Resources.Load<Sprite>(ReadJson._dictBuff[id].buffImage);
        buffObject.buffUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        //버프 활성화 버튼 연결
        //파라미터 전달 시에는 람다를 사용하라
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
        //버프는 한번에 한개만 활성화 가능
        if (IsActiveBuff() == true) return false;

        buffObject.isActive = true;
        buffObject.buffUI.GetComponentInChildren<Button>().interactable = false;
        //_currentBuffList[id].buffUI.GetComponentInChildren<Text>().text = "적용중";
        return true;

    }

    //의뢰 하나가 종료되면 호출된다
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
