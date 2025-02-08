using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuffObject
{
    public int id;
    public bool isActive;
    public GameObject buffUI;
}

public class BuffManager : MonoBehaviour
{
    

    [SerializeField]
    private Dictionary<int, BuffObject> currentBuffList;

    [SerializeField]
    private GameObject buffUIPrefab;
    [SerializeField]
    private Button buffListButton;
    [SerializeField]
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
    }

    private void InitializeBuffList()
    {
        currentBuffList = new Dictionary<int, BuffObject>();
        buffListButton.onClick.RemoveAllListeners();
        buffListButton.onClick.AddListener(ToggleBuffListUI);

        //Test Code
        AddBuff(4001);
        AddBuff(4002);
        AddBuff(4003);
    }

    public void ToggleBuffListUI()
    {
        buffListUI.SetActive(!buffListUI.activeSelf);
        RectTransform listAnchor = buffListUI.GetComponent<RectTransform>();
        int buffCount = currentBuffList.Count;
        listAnchor.offsetMin = new Vector2(0, 40+ (buffCount-1) * -20);

        //버프1개이상 있을 때 위치조정
        if (buffCount < 1) return;
        int index = 0;
        foreach ( var buffObject in currentBuffList.Values)
        {
            RectTransform anchor = buffObject.buffUI.GetComponent<RectTransform>();
            int startY = (buffCount-1) * 10;
            int offsetY = -25 * (index++);
            anchor.anchoredPosition = new Vector2(0, startY + offsetY);
        }
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

        //버프UI(아이콘, 사용버튼) 프리펩 생성
        buffObject.buffUI = Instantiate(buffUIPrefab, Vector3.zero,Quaternion.identity);
        //버프 리스트 UI하위로 설정
        buffObject.buffUI.transform.SetParent(buffListUI.transform,false);
        //버프 이미지 설정
        buffObject.buffUI.GetComponentInChildren<Image>().sprite 
            = Resources.Load<Sprite>(ReadJson._dictBuff[id].buffImage);

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
