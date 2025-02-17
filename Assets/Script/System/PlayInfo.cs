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

    //���� ������ - ������ true �̺����� false
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

    //������ �ε�ÿ��� ���
    public void SetGold(int value) { _currentGold = value; }
    public void SetDay(int value) { _currentCraftDay = value; }

    void Start()
    {
       IntializeGetRecipeDict();
        //Test Code
        //_currentGold = 1000;
    }

    //������ �Լ�
    //������ ���� ���� �ʱ�ȭ
    public void IntializeGetRecipeDict()
    {
        _recipeDict = new Dictionary<int, bool>();
        foreach (var RecipeData in ReadJson._dictPotion)
        {
            if(RecipeData.Value.recipeCost !=0)
                _recipeDict.Add(RecipeData.Key, false);
            else // 0����� �����Ǵ� �⺻ �������̴�.
                _recipeDict[RecipeData.Key] = true;
        }
    }
    
    //������ ȹ�� �Լ�
    public void AddRecipe(int recipeID)
    {
        _recipeDict[recipeID] = true;
    }
    //������ ���� Ȯ�� �Լ�
    public bool HasRecipe(int recipeID)
    {
        if (_recipeDict == null) return false;
        if (_recipeDict.ContainsKey(recipeID))
            return _recipeDict[recipeID];
        else 
            return false;
    }

    //�������� �����ǿ� �̺����������� ����Ʈ�� �����ش�.
    public void SplitQuest(out List<int> AcceptableQuest, out List<int>UnaccpeptableQuest)
    {
        //�����Ǹ� �и��ϴ°Ծƴ϶� ����Ʈ�� �и��ؾ��Ѵ�
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
    //��� �Ҹ� �Լ�
    public void ConsumeGold(int value)
    {
        _currentGold -= value;
        GameManager.GM.SM.SavePlayInfo();
    }
    //��� ȹ�� �Լ�
    public void IncreamentGold(int value)
    {
        _currentGold += value;
        GameManager.GM.SM.SavePlayInfo();
    }

   

    //��¥ �Լ�
    public void IncrementCraftDay()
    {
        _currentCraftDay++;
        _maxCraftDay = Mathf.Max(_maxCraftDay, _currentCraftDay);
        GameManager.GM.SM.SavePlayInfo();
    }

    //0���� �ʱ�ȭ�� ����
    public void Resetinfo()
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        GameManager.GM.BM.ResetBuffList();
        GameManager.GM.SM.SavePlayInfo();
        IntializeGetRecipeDict();
    }

}
