using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QuestInfo
{
    public int questId;
    public int questGrade;
    public string questText;
    public int potionId;
    public int money;
    public int recipeGrade;
    public int minQuality;
    public int maxQuality;
}

[System.Serializable]
public struct QuestData
{
    public List<QuestInfo> root;
}

[System.Serializable]
public struct PotionInfo
{
    public int potionId;
    public string potionName;
    public int potionGrade;
    public int ingredientCount;
    public string[] ingredientName;
    public int[] maxMount;
    public int recipeCost;
    public string potionImage;
}

[System.Serializable]
public struct PotionData
{
    public List<PotionInfo> root;
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

    private void Awake()
    {
        TextAsset questJson = Resources.Load<TextAsset>("Json/quest");
        TextAsset potionJson = Resources.Load<TextAsset>("Json/potion");
        TextAsset buffJson = Resources.Load<TextAsset>("Json/buff");

        QuestData questData = JsonUtility.FromJson<QuestData>(questJson.ToString());
        PotionData potionData = JsonUtility.FromJson<PotionData>(potionJson.ToString());
        BuffData buffData = JsonUtility.FromJson<BuffData>(buffJson.ToString());

        _dictQuest = new Dictionary<int, QuestInfo>();
        _dictPotion = new Dictionary<int, PotionInfo>();
        _dictBuff = new Dictionary<int, BuffInfo>();

        foreach (var info in questData.root)
        {
            _dictQuest.Add(info.questId, info);
        }
        foreach (var info in potionData.root)
        {
            _dictPotion.Add(info.potionId, info);
        }
        foreach (var info in buffData.root)
        {
            _dictBuff.Add(info.buffId, info);
        }
    }
    
}
