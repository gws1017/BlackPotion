using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuffObject
{
    public int Id;
    public int Count;
    public bool IsActive;
    public GameObject BuffUI;
}

public enum BuffType
{
    EvenNumber = 4001,
    OddNumber = 4002,
    PlusPowder = 4003,
    UpgradeBrew = 4004,
    StrangeBrew = 4005
}

public class BuffManager : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, List<BuffObject>> _buffDictionary;

    [SerializeField]
    private GameObject _buffUIPrefab;
    [SerializeField]
    private Button _buffListButton;
    [SerializeField]
    private GameObject _buffListUI;

    [SerializeField] 
    private Vector2 uiStartPosition = new Vector2(-40, -10);
    [SerializeField] 
    private int uiSpacing = 40;

    //Getter
    public int GetStateFromBuffId(int id) =>ReadJson._dictBuff[id].buffState;
    public string GetNameFromBuffId(int id) => ReadJson._dictBuff[id].buffName;
    public string GetExplainFromBuffId(int id) => ReadJson._dictBuff[id].buffExplain;
    public List<int> GetCurrentBuffList() => _buffDictionary.Keys.ToList();

    private int GetBuffCount() => _buffDictionary.Values.Sum(list => list.Count);

    void Start()
    {
        InitializeBuffList();
    }

    private void InitializeBuffList()
    {
        _buffDictionary = new Dictionary<int, List<BuffObject>>();
        _buffListButton.onClick.RemoveAllListeners();
        _buffListButton.onClick.AddListener(ToggleBuffListUI);
    }

    public void ToggleBuffListUI()
    {
        _buffListUI.SetActive(!_buffListUI.activeSelf);
        UpdateBuffUIPositions();
    }


    private void UpdateBuffUIPositions()
    {
        int buffCount = GetBuffCount();
        if (buffCount < 1) return;

        int index = 0;
        foreach (var buffList in _buffDictionary.Values)
        {
            foreach (var buffObject in buffList)
            {
                RectTransform anchor = buffObject.BuffUI.GetComponent<RectTransform>();
                int offsetX = uiSpacing * (index++);
                anchor.anchoredPosition = new Vector2(uiStartPosition.x + offsetX
                    , uiStartPosition.y);
            }
        }
    }

    public void ClearBuffList()
    {
        foreach (var buffList in _buffDictionary.Values)
        {
            foreach (var buff in buffList)
            {
                Destroy(buff.BuffUI);
            }
        }
        _buffDictionary.Clear();
        InitializeBuffList();
    }

    public void CheckBuff(BuffType type, ref int value)
    {
        int BuffID = (int)type;
        switch (type)
        {
            case BuffType.EvenNumber:
                //짝수 버프
                if (IsActiveBuff(BuffID))
                {
                    if (value % 2 == 0) break;
                    if (value == 1) value = 2;
                    else value -= 1;
                }
                else return;
                break;
            case BuffType.OddNumber:
                
                //홀수 버프
                if (IsActiveBuff(BuffID))
                {
                    if (value % 2 == 1) break;
                    value -= 1;
                }
                else return;
                break;
            case BuffType.PlusPowder:
                //증가 가루
                if (IsActiveBuff(BuffID))
                {
                    value += ReadJson._dictBuff[BuffID].buffState;
                }
                else return;
                break;
            case BuffType.UpgradeBrew:
                //양조기 강화
                if (IsActiveBuff(BuffID))
                {
                    value += ReadJson._dictBuff[BuffID].buffState;
                }
                else return;
                break;
            case BuffType.StrangeBrew:
                //이상한 양조기
                if (IsActiveBuff(BuffID))
                {
                    value += UnityEngine.Random.Range(1, IngredientSlot.MAX_NUMBER);
                }
                else return;
                break;

        }
        RemoveUsedBuff();
    }
    public bool IsFullBuffList()
    {
        if (GetBuffCount() >= PlayInfo.MAX_BUFF_COUNT)
        {
            Debug.Log("버프 아이템 최대로 보유중입니다 " + PlayInfo.MAX_BUFF_COUNT);
            return true;
        }
        return false;
    }
    public void AddBuff(int id)
    {
        if (IsFullBuffList()) return;
        if (!ReadJson._dictBuff.ContainsKey(id))
        {
            Debug.LogError($"Not Found Buff ID {id}");
            return;
        }
        BuffObject buffObject = new BuffObject
        {
            Id = id,
            IsActive = false,
            Count = 1,
            BuffUI = Instantiate(_buffUIPrefab, Vector3.zero, Quaternion.identity)
        };

        //버프 UI 설정
        buffObject.BuffUI.transform.SetParent(_buffListUI.transform, false);
        var image = buffObject.BuffUI.GetComponentInChildren<Image>();
        image.sprite = Resources.Load<Sprite>(ReadJson._dictBuff[id].buffImage);
        var button = buffObject.BuffUI.GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ActivateBuff(buffObject));

        if (_buffDictionary.ContainsKey(id) == false)
            _buffDictionary.Add(id, new List<BuffObject>());
        _buffDictionary[id].Add(buffObject);

        UpdateBuffUIPositions();
    }

    public void RemoveBuff(BuffObject buffObject)
    {
        if (buffObject == null) return;
        int id = buffObject.Id;
        if (_buffDictionary.ContainsKey(id))
        {
            _buffDictionary[id].Remove(buffObject);
            buffObject.BuffUI.GetComponentInChildren<Button>().interactable = true;
            buffObject.IsActive = false;

            Destroy(buffObject.BuffUI);
            if (_buffDictionary[id].Count == 0)
                _buffDictionary.Remove(id);

        }
    }

    public bool IsActiveBuff(int buffID = -1)
    {
        if (buffID == -1)
        {
            return _buffDictionary.Values.Any(list => list.Any(buff => buff.IsActive));
        }
        else
        {
            if(_buffDictionary.ContainsKey(buffID))
            {
                return _buffDictionary[buffID].Any(buff => buff.IsActive);
            }
            return false;
        }
    }

    public bool ActivateBuff(BuffObject buffObject)
    {
        //버프는 한번에 한개만 활성화 가능
        if (IsActiveBuff() == true) return false;

        buffObject.IsActive = true;
        buffObject.BuffUI.GetComponentInChildren<Button>().interactable = false;
        return true;

    }

    //의뢰 하나가 종료되면 호출된다
    public void RemoveUsedBuff()
    {
        List<BuffObject> buffsToRemove = new List<BuffObject>();
        foreach (var list in _buffDictionary.Values)
            buffsToRemove.AddRange(list.Where(buff => buff.IsActive));

        foreach (var buff in buffsToRemove)
            RemoveBuff(buff);

        UpdateBuffUIPositions();
    }

}
