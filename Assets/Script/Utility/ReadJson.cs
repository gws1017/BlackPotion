using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadJson : MonoBehaviour
{
    public static Dictionary<int, QuestInfo> _dictQuest;
    public static Dictionary<int, PotionInfo> _dictPotion;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset questJson = Resources.Load<TextAsset>("Json/quest");
        TextAsset potionJson = Resources.Load<TextAsset>("Json/potion");

        QuestData questData = JsonUtility.FromJson<QuestData>(questJson.ToString());
        PotionData potionData = JsonUtility.FromJson<PotionData>(potionJson.ToString());

        _dictQuest = new Dictionary<int, QuestInfo>();
        _dictPotion = new Dictionary<int, PotionInfo>();

        foreach (var info in questData.root)
        {
            _dictQuest.Add(info.questId, info);
        }
        foreach (var info in potionData.root)
        {
            _dictPotion.Add(info.potionId, info);
        }
    }

    [System.Serializable]
    public struct QuestInfo
    {
        public int questId;
        public int questGrade;
        public string questText;
        public int potionId;
        public int money;
        public int recipeGrade;
    }
    [System.Serializable]
    public class QuestData
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
        public string ingredient1;
        public string ingredient2;
        public string ingredient3;
        public string potionImage;
    }
    [System.Serializable]
    public class PotionData
    {
        public List<PotionInfo> root;
    }
}
