using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    [System.Serializable]
    public class SaveData
    {
        public List<int> acceptQuestIds = new List<int>();
        public List<bool> isQuestRestarted = new List<bool>();
        public List<int> buffIds = new List<int>();
        public List<int> recipeIds = new List<int>();
        public Quaternion cameraRotation = Quaternion.identity;
        public int currentGold;
        public int currentDay;
        //퀘스트 진행도 기본값은 -1(시작x)
        public int currentQuestOrder = -1;
    }

    private const string SaveKey = "Save";

    [SerializeField] private SaveData _saveData = new SaveData();


    void Start()
    {
        if (SceneSharedData.loadSaveData)
            GameLoad();
        else
            GameManager.GM.PlayInformation.ResetInfo(false);
    }

    private void OnDestroy()
    {
        //GameSave();
    }

    public void SaveQuestOrder(int value)
    {
        _saveData.currentQuestOrder = value;
        GameSave();
    }

    public void SavePlayInfo()
    {
        var gm = GameManager.GM;
        _saveData.currentGold = gm.PlayInformation.CurrentGold;
        _saveData.currentDay = gm.PlayInformation.CurrentDay;
        _saveData.buffIds = gm.BM.GetCurrentBuffList();
        _saveData.recipeIds = gm.PlayInformation.PossessRecipeList;
        GameSave();
    }

    public void SaveQuest()
    {
        var gm = GameManager.GM;

        _saveData.acceptQuestIds.Clear();
        _saveData.isQuestRestarted.Clear();

        foreach (var quest in gm.Board.AcceptQuestList)
        {
            _saveData.acceptQuestIds.Add(quest.QuestID);
            _saveData.isQuestRestarted.Add(quest.IsRestart);
        }
        GameSave();
    }

    public void SaveBuff()
    {
        _saveData.buffIds = GameManager.GM.BM.GetCurrentBuffList();
        GameSave();
    }

    public void SaveStage()
    {
        _saveData.cameraRotation = GameManager.GM.MainCamera.transform.rotation;
        GameSave();
    }

    private void GameSave()
    {
        try
        {
            string json = JsonUtility.ToJson(_saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("Game Save Success");
        }
        catch(Exception ex)
        {
            Debug.LogError("Save Error : " + ex.Message);
        }

        
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("Save"))
        {
            Debug.LogError("Save is not found");
            return;
        }

        string loadJson = PlayerPrefs.GetString(SaveKey);
        try
        {
            _saveData = JsonUtility.FromJson<SaveData>(loadJson);
            if (_saveData == null)
            {
                Debug.LogError("Load Fail : Data is null");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Load Error " + ex.Message);
            return;
        }

        var gm = GameManager.GM;
        QuestBoard board = gm.Board;
        PotionBrewer brewer = gm.Brewer;
        PlayInfo playInfo = gm.PlayInformation;

        playInfo.SetGold(_saveData.currentGold);
        playInfo.SetDay(_saveData.currentDay);

        //Stage 로드(카메라 회전) Load
        gm.MainCamera.transform.rotation = _saveData.cameraRotation;
        brewer.InitializeBrewer();

        //수주 의뢰 Load
        if (_saveData.acceptQuestIds != null && _saveData.acceptQuestIds.Count > 0)
        {
            board.InitializeQuestBoard();
            //_isSaveData = true;
            int idCnt = Mathf.Min(PlayInfo.MAX_ACCEPT_QUEST_COUNT, _saveData.acceptQuestIds.Count);

            for (int i = 0; i < idCnt; ++i)
            {
                int id = _saveData.acceptQuestIds[i];
                var questObject = Instantiate(board.QuestPrefab);

                Quest quest = questObject.GetComponent<Quest>();
                quest.InitializeQuestFromID(id);

                if(_saveData.isQuestRestarted!= null && _saveData.isQuestRestarted.Count > i)
                    quest.IsRestart = _saveData.isQuestRestarted[i];

                gm.DestoryQuest(questObject);
                board.AcceptQuest(quest);
            }
            brewer.UpdateQuestInfo(_saveData.currentQuestOrder);

        }
        //레시피 로드
        if(_saveData.recipeIds != null && _saveData.recipeIds.Count > 0)
        {
            foreach(int recipeID in _saveData.recipeIds)
            {
                gm.PlayInformation.AddRecipe(recipeID);
            }
        }
        //버프 로드
        if (_saveData.buffIds != null && _saveData.buffIds.Count > 0)
        {
           foreach(int buffID in _saveData.buffIds)
           {
               gm.BM.AddBuff(buffID);
           }
        }

    }
}
