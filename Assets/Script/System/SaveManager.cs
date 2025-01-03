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
        public Quaternion camRotation;
        public int gold;
        public int day;
        //몇번째까지 퀘스트를 완료햇는지 기본값은 -1 == 퀘스트아얘시작도안함
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
        GameSave();
    }

    public void SaveQuestOrder(int value)
    {
        _saveData.currentQuest = value;
    }

    public void SavePlayInfo()
    {
        _saveData.gold = GameManager.GM.PlayInfomation.CurrentGold;
        _saveData.day = GameManager.GM.PlayInfomation.CurrentDay;
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
        PlayInfo playInfo = gm.PlayInfomation;

        playInfo.SetGold(_saveData.gold);
        playInfo.SetDay(_saveData.day);

        //제조 단계(카메라 회전) Load
        gm.MainCamera.transform.rotation = _saveData.camRotation;
        board.IntitilizeQuestBoard();
        brewer.InitializeBrewer();

        //수주 의뢰 Load
        if (_saveData.acceptQuestId.Count > 0)
        {
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

    }
}
