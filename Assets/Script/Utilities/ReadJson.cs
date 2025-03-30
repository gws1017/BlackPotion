using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QuestInfo
{
    public int questId;
    public string questName;
    public int explainTextId;
    public int questGrade;
    public int potionId;
    public int minCapacity;
    public int maxCapacity;
    public int[] reward;
}

[System.Serializable]
public struct PotionInfo
{
    public int potionId;
    public string potionName;
    public int potionGrade;
    public int[] ingredientIdList;
    public int[] materialRatioList;
    public int recipeCost;
    public string potionImage;
}

[System.Serializable]
public struct MaterialInfo
{
    public int materialId;
    public string materialName;
    public int materialGrade;
    public string materialImage;
}

[System.Serializable]
public struct QuestTextInfo
{
    public int questTextID;
    public string questContent;
}

[System.Serializable]
public struct BuffInfo
{
    public int buffId;
    public string buffName;
    public int buffCost;
    public string buffImage;
    public int buffState;
    public string buffExplain;
    public int buffType;
    public int buffTarget;
}

[System.Serializable]
public struct QuestData
{
    public List<QuestInfo> root;
}

[System.Serializable]
public struct PotionData
{
    public List<PotionInfo> root;
}

[System.Serializable]
public struct MaterialData
{
    public List<MaterialInfo> root;
}

[System.Serializable]
public struct QuestTextData
{
    public List<QuestTextInfo> root;
}


[System.Serializable]
public struct BuffData
{
    public List<BuffInfo> root;
}

public class ReadJson : MonoBehaviour
{
    public static Dictionary<int, QuestInfo> _dictQuest;
    public static Dictionary<int, PotionInfo> _dictPotion;
    public static Dictionary<int, BuffInfo> _dictBuff;
    public static Dictionary<int, MaterialInfo> _dictMaterial;
    public static Dictionary<int, QuestTextInfo> _dictQuestText;

    private void Awake()
    {
        TextAsset questJson = Resources.Load<TextAsset>("Json/quest");
        TextAsset potionJson = Resources.Load<TextAsset>("Json/potion");
        TextAsset buffJson = Resources.Load<TextAsset>("Json/buff");
        TextAsset materialJson = Resources.Load<TextAsset>("Json/material");
        TextAsset questTextJson = Resources.Load<TextAsset>("Json/questText");

        QuestData questData = JsonUtility.FromJson<QuestData>(questJson.ToString());
        PotionData potionData = JsonUtility.FromJson<PotionData>(potionJson.ToString());
        BuffData buffData = JsonUtility.FromJson<BuffData>(buffJson.ToString());
        MaterialData materialData = JsonUtility.FromJson<MaterialData>(materialJson.ToString());
        QuestTextData questTextData = JsonUtility.FromJson<QuestTextData>(questTextJson.ToString());


        _dictQuest = new Dictionary<int, QuestInfo>();
        _dictPotion = new Dictionary<int, PotionInfo>();
        _dictBuff = new Dictionary<int, BuffInfo>();
        _dictMaterial = new Dictionary<int, MaterialInfo>();
        _dictQuestText = new Dictionary<int, QuestTextInfo>();

        foreach (var info in questData.root)
        {
            _dictQuest.Add(info.questId, info);
        }
        foreach (var info in potionData.root)
        {
            _dictPotion.Add(info.potionId, info);
        }
        foreach (var info in materialData.root)
        {
            _dictMaterial.Add(info.materialId, info);
        }
        foreach (var info in questTextData.root)
        {
            _dictQuestText.Add(info.questTextID, info);
        }
        foreach (var info in buffData.root)
        {
            BuffInfo formatData = info;
            formatData.buffExplain = string.Format(info.buffExplain, info.buffState);
            _dictBuff.Add(info.buffId, formatData);
        }
        
    }
    
}
