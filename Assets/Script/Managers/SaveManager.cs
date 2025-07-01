using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveManager;

public class SaveManager : MonoBehaviour
{

    [System.Serializable]
    public class SlotInfo
    {
        public int slotId;
        public int uiIngridientIdx;
        public List<int> inputAmountList;
    }

    [System.Serializable]
    public class AcceptQuestInfo
    {
        public int questId = 0;
        public int selectedReward = 0;
        public int requireCapacity = 0;
        public bool isQuestResult = false;
        public bool isQuestRestarted = false;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<AcceptQuestInfo> acceptQuestInfos = new List<AcceptQuestInfo>();
        public List<int> acceptQuestIds = new List<int>();
        public List<bool> isQuestRestarted = new List<bool>();
        public List<int> buffIds = new List<int>();
        public List<int> recipeIds = new List<int>();
        public List<int> purchasedStorebuffIds = new List<int>();
        //Slot
        public List<SlotInfo> slotInfoList = new List<SlotInfo>
        {
            new SlotInfo(),
            new SlotInfo(),
            new SlotInfo()
        };
        public Quaternion cameraRotation = Quaternion.identity;
        public int currentGold;
        public int currentDay;
        //퀘스트 진행도 기본값은 -1(시작x)
        public int currentQuestOrder = -1;
        public bool processPenalty;
        public bool isVisibleCraftResult;
    }

    private const string SaveKey = "Save";

    [SerializeField] private SaveData _saveData = new SaveData();
    private bool _isLoading = false;

    public bool ProcessPenalty => _saveData.processPenalty;

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
    public void SaveVisibleCraftResult(bool value)
    {
        if (_isLoading) return;
        _saveData.isVisibleCraftResult = value;
        GameSave();
    }
    public void SaveFailPenalty(bool value)
    {
        if (_isLoading) return;
        _saveData.processPenalty = value;
        GameSave();
    }
    public void SaveInputAmount(int slotId, int value)
    {
        if (_isLoading) return;
        if (slotId >= 0 && slotId < 3)
        {
            //투입내역을 전부 슬롯 별로 기록한다
            _saveData.slotInfoList[slotId].inputAmountList.Add(value);
            GameSave();
        }
    }

    public void SaveSlotInfo(int slotId, int uiId)
    {
        if (_isLoading) return;
        if (slotId >= 3 || slotId < 0) return;
        if (_saveData.slotInfoList[slotId].inputAmountList != null) return;

        SlotInfo slotInfo = new SlotInfo();
        slotInfo.slotId = (int)slotId;
        slotInfo.uiIngridientIdx = (int)uiId;
        slotInfo.inputAmountList = new List<int>();

        _saveData.slotInfoList[(int)slotId] = slotInfo;
    }

    public void SaveQuestOrder(int value)
    {
        if (_isLoading) return;
        //다음 퀘스트 넘어갈때 호출

        //투입내역 초기화
        _saveData.slotInfoList = new List<SlotInfo>
        {
            new SlotInfo(),
            new SlotInfo(),
            new SlotInfo()
        };
        _saveData.purchasedStorebuffIds.Clear();
        _saveData.currentQuestOrder = value;
        _saveData.processPenalty = false;
        GameSave();
    }

    public void SavePlayInfo()
    {
        if (_isLoading) return;
        var gm = GameManager.GM;
        _saveData.currentGold = gm.PlayInformation.CurrentGold;
        _saveData.currentDay = gm.PlayInformation.CurrentDay;
        SaveBuff();
        _saveData.recipeIds = gm.PlayInformation.PossessRecipeList;
        GameSave();
    }

    public void SaveQuestReward(int index, int reward)
    {
        if (_isLoading) return;
        if(_saveData.acceptQuestInfos.Count > index)
            _saveData.acceptQuestInfos[index].selectedReward = reward;
        else
            Debug.LogWarning("Save Data AcceptQuestInfo Count 0");
        GameSave();
    }
    public void SaveQuestResult(int index, bool isResult)
    {
        if (_isLoading) return;
        if(_saveData.acceptQuestInfos.Count > index && index >=0)
            _saveData.acceptQuestInfos[index].isQuestRestarted = isResult;
        else
            Debug.LogWarning("Save Data AcceptQuestInfo Count 0");
        GameSave();
    }
    public void SaveQuestRestart(int index, bool isRestart = true)
    {
        if (_isLoading) return;
        if(_saveData.acceptQuestInfos.Count > index)
            _saveData.acceptQuestInfos[index].isQuestRestarted = isRestart;
        else
            Debug.LogWarning("Save Data AcceptQuestInfo Count 0");
        GameSave();
    }
    public void SaveQuestList()
    {
        if (_isLoading) return;
        var board = GameManager.GM.Board;

        _saveData.acceptQuestInfos.Clear();
        //삭제
        _saveData.acceptQuestIds.Clear();


        foreach (var quest in board.AcceptQuestList)
        {
            AcceptQuestInfo questInfo = new AcceptQuestInfo();
            questInfo.questId = quest.QuestID;
            questInfo.requireCapacity = quest.RequirePotionCapacity;

            _saveData.acceptQuestInfos.Add(questInfo);

            //삭제예정
            _saveData.acceptQuestIds.Add(quest.QuestID);
            _saveData.isQuestRestarted.Add(quest.IsRestart);
        }
        GameSave();
    }

    public bool CheckPurchase(int ItemId)
    {
        return _saveData.purchasedStorebuffIds.Contains(ItemId);
    }

    public void SavePurchase(int ItemId)
    {
        if (_isLoading) return;
        _saveData.purchasedStorebuffIds.Add(ItemId);
        GameSave();
    }
    public void SaveBuff()
    {
        if (_isLoading) return;
        _saveData.buffIds = GameManager.GM.BM.GetCurrentBuffList();
        GameSave();
    }

    public void SaveStage()
    {
        if (_isLoading) return;
        _saveData.cameraRotation = GameManager.GM.MainCamera.transform.rotation;
        GameSave();
    }

    private void GameSave()
    {
        if (_isLoading) return;

        try
        {
            string json = JsonUtility.ToJson(_saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("Game Save Success");
        }
        catch (Exception ex)
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

        _isLoading = true;

        GameManager.GM.InitializeGameManager();

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
        if (_saveData.acceptQuestInfos != null && _saveData.acceptQuestInfos.Count > 0)
        {
            board.InitializeQuestBoard();
            //_isSaveData = true;
            int idCnt = Mathf.Min(Constants.MAX_ACCEPT_QUEST_COUNT, _saveData.acceptQuestInfos.Count);

            for (int i = 0; i < idCnt; ++i)
            {
                AcceptQuestInfo questInfo = _saveData.acceptQuestInfos[i];
                int id = questInfo.questId;
                var questObject = Instantiate(board.QuestPrefab);

                Quest quest = questObject.GetComponent<Quest>();
                quest.InitializeQuestFromID(id);

                quest.IsRestart = questInfo.isQuestRestarted;
                quest.RequirePotionCapacity = questInfo.requireCapacity;
                quest.SelectRewardMoney = questInfo.selectedReward;

                gm.DestoryQuest(questObject);
                board.AcceptQuest(quest);
                if(board._questResultDict.ContainsKey(quest))
                {
                    board._questResultDict[quest] = questInfo.isQuestResult;
                }
            }
            brewer.UpdateQuestInfo(_saveData.currentQuestOrder);

            var slotInfoList = _saveData.slotInfoList;

            if (slotInfoList.Count > 0)
            {
                foreach (var slot in slotInfoList)
                {
                    foreach (var amount in slot.inputAmountList)
                    {
                        brewer.InsertIngredient(slot.slotId, slot.uiIngridientIdx, amount);
                    }
                }
            }

            //결과창 한번이라도 봤으면 바로 결과창으로 진입
            if(_saveData.isVisibleCraftResult)
            {
                brewer.ShowCraftResultUI();
            }


        }
        //레시피 로드
        if (_saveData.recipeIds != null && _saveData.recipeIds.Count > 0)
        {
            foreach (int recipeID in _saveData.recipeIds)
            {
                gm.PlayInformation.AddRecipe(recipeID);
            }
        }
        //버프 로드
        if (_saveData.buffIds != null && _saveData.buffIds.Count > 0)
        {
            foreach (int buffID in _saveData.buffIds)
            {
                gm.BM.AddBuff(buffID);
            }
        }
        _isLoading = false;


    }
}
