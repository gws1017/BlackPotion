using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{
    //public DaylightTime _playTime;
    //현재 일차
    private int _currentCraftDay = 0;
    //현재 소유하고 있는 골드량
    private int _currentGold = 0;

    //누적 데이터
    public int _questSuccCnt = 0;
    public int _maxCraftDay = 0;

    //보유 레시피 - 보유는 true 미보유는 false
    private Dictionary<int, bool> _recipeDict;

    public int CurrentGold
    {
        get { return _currentGold; }
    }

    public int CurrentDay
    {
        get { return _currentCraftDay; }
    }

    public List<int> PossessRecipeList
    {
        get
        {
            List<int> ret = new List<int>();
            foreach (var data in _recipeDict)
            {
                if (data.Value == true)
                    ret.Add(data.Key);
            }
            return ret;
        }
    }

    //데이터 로드시에만 사용
    public void SetGold(int value) { _currentGold = value; }
    public void SetDay(int value) { _currentCraftDay = value; }

    void Start()
    {
       IntializeGetRecipeDict();
        //Test Code
        //_currentGold = 1000;
    }

    //레시피 함수
    //레시피 보유 정보 초기화
    public void IntializeGetRecipeDict()
    {
        _recipeDict = new Dictionary<int, bool>();
        foreach (var RecipeData in ReadJson._dictPotion)
        {
            if(RecipeData.Value.recipeCost !=0)
                _recipeDict.Add(RecipeData.Key, false);
            else // 0골드인 레시피는 기본 레시피이다.
                _recipeDict[RecipeData.Key] = true;
        }
    }
    
    //레시피 획득 함수
    public void AddRecipe(int recipeID)
    {
        _recipeDict[recipeID] = true;
    }
    //레시피 보유 확인 함수
    public bool HasRecipe(int recipeID)
    {
        if (_recipeDict == null) return false;
        if (_recipeDict.ContainsKey(recipeID))
            return _recipeDict[recipeID];
        else 
            return false;
    }

    //보유중인 레시피와 미보유레시피의 리스트를 돌려준다.
    public void SplitQuest(out List<int> AcceptableQuest, out List<int>UnaccpeptableQuest)
    {
        //레시피를 분리하는게아니라 퀘스트를 분리해야한다
        AcceptableQuest = new List<int>();
        UnaccpeptableQuest = new List<int>();

        foreach(var Data in ReadJson._dictQuest.Values)
        {
            if(HasRecipe(Data.potionId))
                AcceptableQuest.Add(Data.questId);
            else
                UnaccpeptableQuest.Add(Data.questId);
        }
    }
    //골드 소모 함수
    public void ConsumeGold(int value)
    {
        _currentGold -= value;
        GameManager.GM.SM.SavePlayInfo();
    }
    //골드 획득 함수
    public void IncreamentGold(int value)
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
    public void Resetinfo()
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        GameManager.GM.BM.ResetBuffList();
        GameManager.GM.SM.SavePlayInfo();
        IntializeGetRecipeDict();
    }

}
