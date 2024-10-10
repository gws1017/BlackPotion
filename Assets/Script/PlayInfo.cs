using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{
    //public DaylightTime _playTime;
    //���� ����
    private int _currentCraftDay = 0;
    //���� �����ϰ� �ִ� ��差
    private int _currentGold = 0;

    //���� ������
    public int _questSuccCnt = 0;
    public int _maxCraftDay = 0;

    //���� ������
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

    //������ �Լ�
    //������ ���� ���� �ʱ�ȭ
    public void IntializeGetRecipeDict()
    {
        _getRecipeDict = new Dictionary<int, bool>();
        foreach (var RecipeData in ReadJson._dictPotion)
        {
            if(RecipeData.Value.recipeCost !=0)
                _getRecipeDict.Add(RecipeData.Key, false);
            else // 0����� �����Ǵ� �⺻ �������̴�.
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
    //��� �Լ�
    public void ConsumeGold(int value)
    {
        _currentGold -= value; 
    }

    public void IncreamentGold(int value)
    {
        _currentGold += value;
    }

    //��¥ �Լ�
    public void IncrementCraftDay()
    {
        _currentCraftDay++;
        _maxCraftDay = Mathf.Max(_maxCraftDay, _currentCraftDay);
    }

    //0���� �ʱ�ȭ�� ����
    public void Resetinfo()
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        IntializeGetRecipeDict();
    }

}
