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

    //보유 레시피
    private Dictionary<int, bool> _getRecipeDict;

    public int CurrentGold
    {
        get { return _currentGold; }
    }

    public int CurrentDay
    {
        get { return _currentCraftDay; }
    }
    void Start()
    {
       IntializeGetRecipeDict();
    }

    //레시피 함수
    //레시피 보유 정보 초기화
    public void IntializeGetRecipeDict()
    {
        _getRecipeDict = new Dictionary<int, bool>();
        foreach (var RecipeData in ReadJson._dictPotion)
        {
            if(RecipeData.Value.recipeCost !=0)
                _getRecipeDict.Add(RecipeData.Key, false);
            else // 0골드인 레시피는 기본 레시피이다.
                _getRecipeDict[RecipeData.Key] = true;
        }
    }

    public void AddRecipe(int recipeID)
    {
        _getRecipeDict[recipeID] = true;
    }
    public bool HasRecipe(int recipeID)
    {
        if(_getRecipeDict.ContainsKey(recipeID))
            return _getRecipeDict[recipeID];
        else 
            return false;
    }
    //골드 함수
    public void ConsumeGold(int value)
    {
        _currentGold -= value; 
    }

    public void IncreamentGold(int value)
    {
        _currentGold += value;
    }

    //날짜 함수
    public void IncrementCraftDay()
    {
        _currentCraftDay++;
        _maxCraftDay = Mathf.Max(_maxCraftDay, _currentCraftDay);
    }

    //0일차 초기화시 사용됨
    public void Resetinfo()
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        IntializeGetRecipeDict();
    }

}
