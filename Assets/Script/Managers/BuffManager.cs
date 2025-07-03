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
    EvenNumber = 5001,
    OddNumber = 5002,
    PlusPowder = 5003,
    UpgradeBrew = 5004,
    StrangeBrew = 5005,
    InsightPowder = 5006
}

public class BuffManager : MonoBehaviour
{
    [SerializeField] private Dictionary<int, List<BuffObject>> _buffDictionary;

    [SerializeField] private GameObject _buffUIPrefab;
    [SerializeField] private Button _buffListButton;
    [SerializeField] private GameObject _buffInventoryObject;

    [SerializeField] private Vector2 _uiStartPosition = new Vector2(-40, -10);
    [SerializeField] private float _uiSpacing = 0.015f;
    [SerializeField] private int _buffInventoryToggleDistance = -55;

    private int _disableCount = 0;
    private bool _canActiveBuff = true;
    //Getter
    public int GetStateFromBuffId(int id) => ReadJson._dictBuff[id].buffState;
    public string GetNameFromBuffId(int id) => ReadJson._dictBuff[id].buffName;
    public string GetExplainFromBuffId(int id) => ReadJson._dictBuff[id].buffExplain;
    public List<int> GetCurrentBuffList()
    {
        if (_buffDictionary.Count > 0)
            return _buffDictionary.Keys.ToList();
        else
            return null;
    }

    private int GetBuffCount() => _buffDictionary.Values.Sum(list => list.Count);

    void Start()
    {
        InitializeBuffList();
    }

    private void InitializeBuffList()
    {
        _buffDictionary = new Dictionary<int, List<BuffObject>>();
        _buffListButton.onClick.RemoveAllListeners();
        _buffListButton.onClick.AddListener(ToggleBuffInventory);
    }

    public void ToggleBuffInventory()
    {
        SoundManager._Instance.PlaySFXAtObject(_buffInventoryObject, SFXType.Item);
        Vector3 position = _buffInventoryObject.transform.localPosition;
        if (position.y == _buffInventoryToggleDistance)
        {
            position.y = 0;
        }
        else
        {
            position.y = _buffInventoryToggleDistance;
        }
        _buffInventoryObject.transform.localPosition = position;
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
                Transform anchor = buffObject.BuffUI.transform;

                float offsetX = _uiSpacing * (index++);
                anchor.localPosition = new Vector3(_uiStartPosition.x + offsetX, 0, _uiStartPosition.y);
            }
        }
    }

    public void ClearBuffList()
    {
        if (_buffDictionary == null) return;

        var buffLists = _buffDictionary.Values;
        foreach (var buffList in buffLists)
        {
            foreach (var buff in buffList)
            {
                Destroy(buff.BuffUI);
            }
        }
        _buffDictionary.Clear();
        InitializeBuffList();
    }

    public void InsightCapacity()
    {
        var brewer = GameManager.GM.Brewer;
        int reqCapacity = brewer.CurrentQuest.RequirePotionCapacity;//128 

        string reqCapacityText = $"{reqCapacity / 10}X";
        brewer.ReqCapacityValueText.text = reqCapacityText;
    }

    public void CheckBuff(BuffType type, ref int value)
    {
        int BuffID = (int)type;
        switch (type)
        {
            case BuffType.EvenNumber:
                //¦�� ����
                if (IsActiveBuff(BuffID))
                {
                    if (value % 2 == 0) break;
                    if (value == 1) value = 2;
                    else value -= 1;
                }
                else return;
                break;
            case BuffType.OddNumber:

                //Ȧ�� ����
                if (IsActiveBuff(BuffID))
                {
                    if (value % 2 == 1) break;
                    value -= 1;
                }
                else return;
                break;
            case BuffType.PlusPowder:
                //���� ����
                if (IsActiveBuff(BuffID))
                {
                    value += ReadJson._dictBuff[BuffID].buffState;
                }
                else return;
                break;
            case BuffType.UpgradeBrew:
                //������ ��ȭ
                if (IsActiveBuff(BuffID))
                {
                    value += ReadJson._dictBuff[BuffID].buffState;
                }
                else return;
                break;
            case BuffType.StrangeBrew:
                //�̻��� ������
                if (IsActiveBuff(BuffID))
                {
                    var brewer = GameManager.GM.Brewer;
                    var slots = brewer.Slots;
                    int[] ma = brewer.MaxAmount;

                    if (ma != null )
                    {
                        for(int i =0; i<ma.Length; ++i)
                        {
                            int amount = ma[i];
                            if (amount != 0)
                            {
                                int val = UnityEngine.Random.Range(0, GameManager.GM.Brewer.MaxAmount[i]);
                                int id = slots[i].GetComponent<IngredientSlot>().IngridientIndex;
                                brewer.SetCurrentAmount(i,id, val);
                                //���� ��� ������ �������߰�
                                GameManager.GM.SM.SaveInputAmount(i, val,true);
                            }
                        }
                    }
                    
                }
                else return;
                break;
            case BuffType.InsightPowder:
                //������ ����
                if (IsActiveBuff(BuffID))
                {
                    InsightCapacity();
                    GameManager.GM.SM.SaveInsightPowder();
                }
                else return;
                break;

        }
        RemoveUsedBuff();
    }
    public bool IsFullBuffList()
    {
        if (GetBuffCount() >= Constants.MAX_BUFF_COUNT)
        {
            Canvas camCanvas = GameManager.GM.MainCamera.GetComponentInChildren<Canvas>();
            GameManager.GM.CreateInfoUI("�������� ����á���ϴ�.",
                camCanvas.transform,null,Vector3.one * Constants.UI_SCALE);
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

        //���� UI ����
        buffObject.BuffUI.transform.SetParent(_buffInventoryObject.transform, false);
        var image = buffObject.BuffUI.GetComponentInChildren<Image>();
        image.sprite = Resources.Load<Sprite>(ReadJson._dictBuff[id].buffImage);
        var button = buffObject.BuffUI.GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ToggleBuff(buffObject));

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
            GameManager.GM.SM.SaveBuff();
    }

    public bool IsActiveBuff(int buffID = -1)
    {
        
        if (buffID == -1)
        {
            return _buffDictionary.Values.Any(list => list.Any(buff => buff.IsActive));
        }
        else
        {
            if (_buffDictionary.ContainsKey(buffID))
            {
                return _buffDictionary[buffID].Any(buff => buff.IsActive);
            }
            return false;
        }
    }


    public bool ToggleBuff(BuffObject buffObject)
    {
        if (buffObject.IsActive)
            return DeActivateBuff(buffObject);
        else
            return ActivateBuff(buffObject);
    }

    public bool ActivateBuff(BuffObject buffObject)
    {
        if (_canActiveBuff == false)
            return false;
        //������ �ѹ��� �Ѱ��� Ȱ��ȭ ����
        if (IsActiveBuff() == true) return false;
        if(buffObject.Id == (int)BuffType.PlusPowder)
        {
           GameManager.GM.Brewer._activePlusPowder = true;
        }
     
        buffObject.IsActive = true;

        Button BuffButton = buffObject.BuffUI.GetComponentInChildren<Button>();
        ColorBlock colorBlock = BuffButton.colors;
        colorBlock.normalColor = new Color(1, 1, 1, 0.1f);
        colorBlock.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 0.1f);
        colorBlock.pressedColor = new Color(0.78f, 0.78f, 0.78f, 0.1f);
        BuffButton.colors = colorBlock;

        SoundManager._Instance.PlaySFXAtObject(buffObject.BuffUI, SFXType.Click);
        int val= 0 ;
        //�ӽ÷� �������
        CheckBuff(BuffType.StrangeBrew, ref val);
        CheckBuff(BuffType.InsightPowder, ref val);
        return true;

    }

    //��������
    public bool DeActivateBuff(BuffObject buffObject)
    {
        if (_canActiveBuff == false)
            return false;
        if (buffObject.IsActive == false) return false;

        if (buffObject.Id == (int)BuffType.PlusPowder)
        {
            GameManager.GM.Brewer._activePlusPowder = false;
        }

        buffObject.IsActive = false;

        Button BuffButton = buffObject.BuffUI.GetComponentInChildren<Button>();
        ColorBlock colorBlock = BuffButton.colors;
        colorBlock.normalColor = new Color(1f, 1f, 1f, 1f);
        colorBlock.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
        colorBlock.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        BuffButton.colors = colorBlock;

        SoundManager._Instance.PlaySFXAtObject(buffObject.BuffUI, SFXType.Click);

        return true;

    }

    //�Ƿ� �ϳ��� ����Ǹ� ȣ��ȴ�
    public void RemoveUsedBuff()
    {
        List<BuffObject> buffsToRemove = new List<BuffObject>();
        foreach (var list in _buffDictionary.Values)
            buffsToRemove.AddRange(list.Where(buff => buff.IsActive));

        foreach (var buff in buffsToRemove)
            RemoveBuff(buff);

        UpdateBuffUIPositions();
    }

    public void EnableBuffInventory()
    {
        _disableCount--;
        if(_disableCount <= 0)
        {
            _disableCount = 0;
            _canActiveBuff = true;
        }
    }
    public void DisableBuffInventory()
    {
        _disableCount++;
        _canActiveBuff = false;
    }
}
