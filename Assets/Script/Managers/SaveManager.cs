using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveManager;

public class SaveManager : MonoBehaviour
{
    [System.Serializable]
    public enum InputEventType
    {
        FreeInput,
        Refill,
        FreeRefill,
        StrageBrew
    }

    [System.Serializable]
    public class IngridientEvent
    {
        public int slotId;
        public InputEventType type;
        public int value;
    }

    [System.Serializable]
    public class SlotInfo
    {
        public int slotId;
        public int uiIngridientIdx;
        public int strangeBrewValue;
        public List<int> usedAmountList;
        public List<int> postStrangeBrewAmountList;
    }

    [System.Serializable]
    public class AcceptQuestInfo
    {
        public int questId = 0;
        public int selectedReward = 0;
        public int requireCapacity = 0;
        public int strangeBrewValue = 0;
        public List<IngridientEvent> inputEvents = new List<IngridientEvent>();
        public bool isQuestResult = false;
        public bool isQuestRestarted = false;
        public bool isInsightPowder = false;
        public bool isStrageBrew = false;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<AcceptQuestInfo> acceptQuestInfos = new List<AcceptQuestInfo>();
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
        //퀘스트 진행도
        public int currentQuestOrder = 0;

        public bool processPenalty;
        public bool isVisibleCraftResult;
    }

    //항상 불러오는 시스템 데이터
    [System.Serializable]

    public class SystemData
    {
        public float bgmVolume;
        public float sfxVolume;
        public int MaxGold;
        public string version;
        public bool isContinue;
    }

    private const string SaveKey = "Save";
    private const string SystemKey = "SystemSave";

    [SerializeField] private SaveData _saveData = new SaveData();
    [SerializeField] private SystemData _systemData = new SystemData();
    private bool _isLoading = false;

    public int MaxGold => _systemData.MaxGold;
    public bool ProcessPenalty => _saveData.processPenalty;
    public float BGMVolume
    {
        get
        {
            SystemLoad();
            return _systemData.bgmVolume;
        }
        
    }
    public float SFXVolume
    {
        get
        {
            SystemLoad();
            return _systemData.sfxVolume;
        }
    }


    void Start()
    {
        SystemLoad();

        Scene current = SceneManager.GetActiveScene();

        if (current.name == Constants.GAME_PLAY_SCENE)
        {
            if (SceneSharedData.loadSaveData)
                GameLoad();
            else
                GameManager.GM.PlayInformation.ResetInfo(false);
        }
    }

    private void OnDestroy()
    {
        //GameSave();
    }

    public void SaveMaxGold(int gold)
    {
        _systemData.MaxGold = Mathf.Max(gold, _systemData.MaxGold);
        SystemSave();
    }

    public void SaveVolume(SoundType type, float value)
    {
        if(SoundType.SFX == type)
            _systemData.sfxVolume = value;
        if(SoundType.BGM == type)
            _systemData.bgmVolume = value;
        SystemSave();
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
    //public void SaveInputAmount(int slotId, int value,bool isReset = false)
    //{
    //    if (_isLoading) return;
    //    if (slotId >= 0 && slotId < 3)
    //    {
    //        int currentOrder = GameManager.GM.Brewer.CurrentQuestIndex;
    //        var currentQuest = _saveData.acceptQuestInfos[currentOrder];
    //        var slotInfo = _saveData.slotInfoList[slotId];
    //        if (isReset)
    //            slotInfo.usedAmountList.Clear();
    //        //투입내역을 전부 슬롯 별로 기록한다
    //        slotInfo.usedAmountList.Add(value);

    //        //양조기 쓴적이 있으면 따로한번더 저장
    //        if(currentQuest.isStrageBrew)
    //            slotInfo.postStrangeBrewAmountList.Add(value);
    //        GameSave();
    //    }
    //}
    public void SaveInputEvent(IngridientEvent ev,int currentQuestIndex)
    {
        if (_isLoading) return;
        _saveData.acceptQuestInfos[currentQuestIndex].inputEvents.Add(ev);
        GameSave();
    }

    public void SaveSlotInfo(int slotId, int uiId)
    {
        if (_isLoading) return;
        if (slotId >= 3 || slotId < 0) return;
        if (_saveData.slotInfoList[slotId].usedAmountList != null) return;

        SlotInfo slotInfo = new SlotInfo();
        slotInfo.slotId = (int)slotId;
        slotInfo.uiIngridientIdx = (int)uiId;
        slotInfo.usedAmountList = new List<int>();
        slotInfo.postStrangeBrewAmountList = new List<int>();
        slotInfo.strangeBrewValue = 0;
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
    public void SaveUseBuff(BuffType type, int value = -1,int slotId = -1)
    {
        if (_isLoading) return;
        int count = GameManager.GM.Brewer.CurrentQuestIndex;
        var currentQuest = _saveData.acceptQuestInfos[count];
        switch (type)
        {
            case BuffType.InsightPowder:
                currentQuest.isInsightPowder = true;
                break;
            case BuffType.StrangeBrew:
                if (value != -1 && slotId != -1)
                {
                    var slotInfo = _saveData.slotInfoList[slotId];
                    //이전 양조기 이후 기록은 초기화
                    slotInfo.postStrangeBrewAmountList.Clear();
                    slotInfo.strangeBrewValue = value;
                    currentQuest.isStrageBrew = true;

                }                  
                break;
        }
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
        {
            _saveData.acceptQuestInfos[index].isQuestRestarted = isRestart;
            _saveData.acceptQuestInfos[index].inputEvents.Clear();
        }
        else
            Debug.LogWarning("Save Data AcceptQuestInfo Count 0");
        GameSave();
    }
    public void SaveQuestList()
    {
        if (_isLoading) return;
        var board = GameManager.GM.Board;

        _saveData.acceptQuestInfos.Clear();

        foreach (var quest in board.AcceptQuestList)
        {
            AcceptQuestInfo questInfo = new AcceptQuestInfo();
            questInfo.questId = quest.QuestID;
            questInfo.requireCapacity = quest.RequirePotionCapacity;

            _saveData.acceptQuestInfos.Add(questInfo);
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
        if (GameManager.GM.CurrentStage != GameStage.Brewing) return;
        if (GameManager.GM.BM == null) return;
        _saveData.buffIds = GameManager.GM.BM.GetCurrentBuffList();
        GameSave();
    }

    public void SaveStage()
    {
        if (_isLoading) return;
        _saveData.cameraRotation = GameManager.GM.MainCamera.transform.rotation;
        GameSave();
    }
    public void SaveContinue(bool value)
    {
        _systemData.isContinue = value;
        SystemSave();
    }
    public void SaveVersion()
    {
        _systemData.version = Application.version;
        SystemSave();
    }

    private void SystemSave()
    {
        if (_isLoading) return;
        try
        {
            string json = JsonUtility.ToJson(_systemData);
            PlayerPrefs.SetString(SystemKey, json);
            PlayerPrefs.Save();
            Debug.Log("System Save Success");
        }
        catch (Exception ex)
        {
            Debug.LogError("System Save Error : " + ex.Message);
        }
    }

    private void GameSave()
    {
        if (_isLoading) return;
        try
        {
            SaveVersion();
            SaveContinue(true);
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

    public void SystemLoad()
    {
        Camera MainCamera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
        TitleHUD hud = MainCamera.GetComponentInChildren<TitleHUD>();

        if (!PlayerPrefs.HasKey(SystemKey))
        {
            Debug.LogError("System Setting is not found");
            if(hud != null)
                hud.DisableContinueButton();
            return;
        }
        string loadJson = PlayerPrefs.GetString(SystemKey);
        

        if (!loadJson.Contains("\"version\""))
        {
            if (hud != null)
                hud.DisableContinueButton();
        }
        else
        {
            try
            {
                _systemData = JsonUtility.FromJson<SystemData>(loadJson);
                if (_systemData == null)
                {
                    Debug.LogError("Load Fail : Setting Data is null");
                    return;
                }

                string currentVersion = Application.version;
                if (_systemData.version != currentVersion)
                {
                    if (hud != null)
                        hud.DisableContinueButton();
                }
                else
                {


                    if (_systemData.isContinue == false)
                    {
                        if (hud != null)
                            hud.DisableContinueButton();
                    }
                    else
                    {
                        if (hud != null)
                            hud.EnableContinueButton();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("System Setting Load Error " + ex.Message);
                return;
            }
        }

        
    }
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.LogError("Save is not found");
            return;
        }

        _isLoading = true;

        GameManager.GM.InitializeGameManager(_isLoading);

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
        gm.ChangeStage(_saveData.cameraRotation);
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
            int currentOrder = Mathf.Clamp(_saveData.currentQuestOrder, 0, 4);

            brewer.UpdateQuestInfo(currentOrder);

            var slotInfoList = _saveData.slotInfoList;
            var currentQuest = _saveData.acceptQuestInfos[currentOrder];
            var inputEvents = currentQuest.inputEvents;
            foreach(var inputEvent in inputEvents)
            {
                int sid = inputEvent.slotId;
                int amount = inputEvent.value;
                switch(inputEvent.type)
                {
                    case InputEventType.FreeInput:
                        brewer.Slots[sid].HandleInputAmount(amount);
                        break;
                    case InputEventType.FreeRefill:
                        brewer.Slots[sid].FreeReset();
                        brewer.Slots[sid].HandleInputAmount(amount);
                        break;
                    case InputEventType.Refill:
                        brewer.Slots[sid].ResetIngredientUsage();
                        break;
                    case InputEventType.StrageBrew:
                        int uiId = brewer.Slots[sid].IngridientIndex;
                        brewer.SetCurrentAmount(sid, uiId, amount);
                        break;
                }
            }

            //if (slotInfoList.Count > 0)
            //{
            //    foreach (var slot in slotInfoList)
            //    {
            //        foreach (var amount in slot.usedAmountList)
            //        {
            //            brewer.Slots[slot.slotId].HandleInputAmount(amount);
            //        }

            //        if(currentQuest.isStrageBrew)
            //        {
            //           brewer.SetCurrentAmount(slot.slotId,slot.uiIngridientIdx, slot.strangeBrewValue);
            //        }

            //        foreach (var amount in slot.postStrangeBrewAmountList)
            //        {
            //            brewer.Slots[slot.slotId].HandleInputAmount(amount);
            //        }
            //    }
            //}

            if (_saveData.acceptQuestInfos[currentOrder].isInsightPowder)
            {
                GameManager.GM.BM.InsightCapacity();
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
        
        if(gm.CurrentStage == GameStage.Receipt)
        {
            brewer.GetNextCraft();
        }
        _isLoading = false;


    }
}
