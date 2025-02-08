using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{

    class BuffObject
    {
        public int id;
        public bool isActive;
        public GameObject buffObject;
    }

    [SerializeField]
    private Dictionary<int, BuffObject> currentBuffList;

    [SerializeField]
    private GameObject buffUIPrefab;
    private GameObject buffListUI;

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
        buffListUI = GameObject.Find("Current BuffList UI");
    }

    private void InitializeBuffList()
    {
        currentBuffList = new Dictionary<int, BuffObject>();
    }

    public void ResetBuffList()
    {
        foreach (var buffObject in currentBuffList.Values)
        {
            buffObject.isActive = false;     
        }
    }
    
    public void AddBuff(int id)
    {
        BuffObject buffObject = new BuffObject();
        buffObject.id = id;
        buffObject.isActive = false;

        currentBuffList.Add(id, buffObject);
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
        if (currentBuffList.ContainsKey(id))
        { 
            currentBuffList[id].isActive = true;
            return true;
        }
        return false;
    }

    public void DeactivateBuff(int id)
    {
        if (currentBuffList.ContainsKey(id))
        {
            currentBuffList[id].isActive = false;
        }
    }
}
