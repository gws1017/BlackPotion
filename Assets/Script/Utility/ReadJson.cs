using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadJson : MonoBehaviour
{
    public static Dictionary<int, QuestInfo> _dictQuest;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset questJson = Resources.Load<TextAsset>("Json/quest");
        QuestData questData = JsonUtility.FromJson<QuestData>(questJson.ToString());
        _dictQuest = new Dictionary<int, QuestInfo>();
        foreach (var info in questData.root)
        {
            _dictQuest.Add(info.questId, info);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
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
}
