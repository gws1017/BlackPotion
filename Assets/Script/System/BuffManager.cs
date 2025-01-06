using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, bool> currentBuffList;

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
        currentBuffList = new Dictionary<int, bool>();
    }

    public void ResetBuffList()
    {
        foreach (var data in currentBuffList)
        {
            currentBuffList[data.Key] = false;
        }
    }
    
    public void AddBuff(int id)
    {
        currentBuffList.Add(id, false);
    }

    public void RemoveBuff(int id)
    {
        if (currentBuffList.ContainsKey(id))
        {
            currentBuffList.Remove(id);
        }
    }

    public bool ActivateBuff(int id)
    {
        if(currentBuffList.ContainsKey(id))
        {
            currentBuffList[id] = true;
        }
        return currentBuffList[id];
    }

    public void DeactivateBuff(int id)
    {
        if (currentBuffList.ContainsKey(id))
        {
            currentBuffList[id] = false;
        }
    }
}
