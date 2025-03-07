using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class SaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct SaveData
    {
        public List<int> acceptQuestId;
        public List<int> buffList;
        public List<int> recipeList;
        public Quaternion camRotation;
        public int gold;
        public int day;
        //���°���� ����Ʈ�� �Ϸ��޴��� �⺻���� -1 == ����Ʈ�ƾ���۵�����
        public int currentQuest;
    }

    [SerializeField]
    private SaveData _saveData;


    // Start is called before the first frame update
    void Start()
    {
        GameLoad();
    }

    private void OnDestroy()
    {
        //GameSave();
    }

    public void SaveQuestOrder(int value)
    {
        _saveData.currentQuest = value;
    }

    public void SavePlayInfo()
    {
        _saveData.gold = GameManager.GM.PlayInformation.CurrentGold;
        _saveData.day = GameManager.GM.PlayInformation.CurrentDay;
        _saveData.buffList = GameManager.GM.BM.GetCurrentBuffList();
        _saveData.recipeList = GameManager.GM.PlayInformation.PossessRecipeList;
        GameSave();
    }

    public void SaveQuest()
    {
        _saveData.acceptQuestId = new List<int>();
        foreach (var quest in GameManager.GM.Board.AcceptQuestList)
        {
            _saveData.acceptQuestId.Add(quest.QuestID);
        }
        GameSave();
    }

    public void SaveBuff()
    {
        _saveData.buffList = GameManager.GM.BM.GetCurrentBuffList();
        GameSave();
    }

    public void SaveStage()
    {
        _saveData.camRotation = GameManager.GM.MainCamera.transform.rotation;
        GameSave();
    }

    private void GameSave()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(_saveData));
        PlayerPrefs.Save();
    }
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("Save"))
            return;
        string loadJson = PlayerPrefs.GetString("Save");
        _saveData = JsonUtility.FromJson<SaveData>(loadJson);

        var gm = GameManager.GM;
        QuestBoard board = gm.Board;
        PotionBrewer brewer = gm.Brewer;
        PlayInfo playInfo = gm.PlayInformation;

        playInfo.SetGold(_saveData.gold);
        playInfo.SetDay(_saveData.day);

        //���� �ܰ�(ī�޶� ȸ��) Load
        gm.MainCamera.transform.rotation = _saveData.camRotation;
        brewer.InitializeBrewer();

        //���� �Ƿ� Load
        if (_saveData.acceptQuestId.Count > 0)
        {
            board.InitilizeQuestBoard();
            //_isSaveData = true;
            int idCnt = Mathf.Min(5, _saveData.acceptQuestId.Count);
            for (int i = 0; i < idCnt; ++i)
            {
                int id = _saveData.acceptQuestId[i];
                var questObject = Instantiate(board.QuestPrefab);
                questObject.GetComponent<Quest>().InitializeQuestFromID(id);
                gm.DestoryQuest(questObject);
                board.AcceptQuest(questObject.GetComponent<Quest>());
            }
            brewer.UpdateQuestInfo(_saveData.currentQuest);
        }
        if(_saveData.recipeList.Count > 0)
        {
            foreach(int recipeID in _saveData.recipeList)
            {
                GameManager.GM.PlayInformation.AddRecipe(recipeID);
            }
        }
        //���� �ε�
        if (_saveData.buffList.Count > 0)
        {
           foreach(int buffID in _saveData.buffList)
           {
                GameManager.GM.BM.AddBuff(buffID);
           }
        }

    }
}
