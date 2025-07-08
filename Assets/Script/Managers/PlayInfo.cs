using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{

    private int _currentCraftDay = 0;//현재 일차
    private int _currentGold = 50;//현재 소유하고 있는 골드량

    //누적 데이터
    //public DaylightTime _playTime;
    public int _questSuccCount = 0;
    public int _maxCraftDay = 0;
    public int _maxGold = 0;

    private Dictionary<int, bool> _recipeDict; //보유 레시피 - 보유는 true 미보유는 false

    //Getter Setter
    public int MaxGold { get => _maxGold; set => _maxGold = value; }
    public int CurrentGold => _currentGold;
    public int CurrentDay => _currentCraftDay;
    public int MaxCraftDay => _maxCraftDay;
    public int QuestSuccessCount { get => _questSuccCount;set => _questSuccCount = value; }

    public List<int> PossessRecipeList
    {
        get
        {
            List<int> list = new List<int>();
            foreach (var recipe in _recipeDict)
            {
                if (recipe.Value)
                    list.Add(recipe.Key);
            }
            return list;
        }
    }

   
    public void SetGold(int value) { _currentGold = value; }
    public void SetDay(int value) { _currentCraftDay = value; }

    private void Start()
    {
       IntializeRecipeDict();
    }

    //레시피 함수
    //레시피 보유 정보 초기화
    public void IntializeRecipeDict()
    {
        _recipeDict = new Dictionary<int, bool>();
        foreach (var recipeData in ReadJson._dictPotion)
        {
           _recipeDict.Add(recipeData.Key, false);
        }
    }
    
    public List<int> GetSelectableRecipe()
    {
        List<int> SelectableReicpe = new List<int>();
        foreach (var recipeData in ReadJson._dictPotion)
        {
            if(recipeData.Value.potionGrade == 0)
                SelectableReicpe.Add(recipeData.Value.potionId);
        }

        System.Random rand = new System.Random();
        return SelectableReicpe.OrderBy(x => rand.Next()).Take(3).ToList();
    }
    //레시피 획득 함수
    public void AddRecipe(int recipeID)
    {
        
        if (_recipeDict == null) return;

        if(_recipeDict.ContainsKey(recipeID))
            _recipeDict[recipeID] = true;
        GameManager.GM.SM.SavePlayInfo();
    }
    //레시피 보유 확인 함수
    public bool HasRecipe(int recipeID)
    {
        return _recipeDict != null &&
                _recipeDict.TryGetValue(recipeID, out bool hasRecipe) &&
                hasRecipe;
    }

    //보유중인 레시피와 미보유레시피의 리스트를 분리해서 반환한다
    public void SplitQuest(out List<int> acceptableQuest, out List<int> unaccpeptableQuest)
    {
        acceptableQuest = new List<int>();
        unaccpeptableQuest = new List<int>();

        foreach(var questData in ReadJson._dictQuest.Values)
        {
            if(HasRecipe(questData.potionId))
                acceptableQuest.Add(questData.questId);
            else
                unaccpeptableQuest.Add(questData.questId);
        }
    }
    //골드 소모 함수
    public void ConsumeGold(int value)
    {
        _currentGold -= value;
        GameManager.GM.SM.SavePlayInfo();
    }
    //골드 획득 함수
    public void IncrementGold(int value)
    {
        _currentGold += value;
        GameManager.GM.SM.SavePlayInfo();
    }

    //날짜 함수
    public void IncrementCraftDay()
    {
        _currentCraftDay++;
        _maxCraftDay = Mathf.Max(_maxCraftDay, _currentCraftDay);
        GameManager.GM.SM.SavePlayInfo();
    }

    //0일차 초기화시 사용됨
    public void ResetInfo(bool isSave = true)
    {
        _currentCraftDay = 0;
        _currentGold = Constants.BASE_GOLD;

        IntializeRecipeDict();

        if(GameManager.GM.BM)
            GameManager.GM.BM.ClearBuffList();
        if (isSave)
            GameManager.GM.SM.SavePlayInfo();
        GameManager.GM.InitializeGameManager();
    }

}
