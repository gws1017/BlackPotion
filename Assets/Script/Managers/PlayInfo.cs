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
    public int _questSuccCount = 0;
    public int _maxCraftDay = 0;

    //���� ������ - ������ true �̺����� false
    private Dictionary<int, bool> _recipeDict;

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

    //������ �ε�ÿ��� ���
    public void SetGold(int value) { _currentGold = value; }
    public void SetDay(int value) { _currentCraftDay = value; }

    private void Start()
    {
       IntializeRecipeDict();
    }

    //������ �Լ�
    //������ ���� ���� �ʱ�ȭ
    public void IntializeRecipeDict()
    {
        _recipeDict = new Dictionary<int, bool>();
        foreach (var recipeData in ReadJson._dictPotion)
        {
            // 0����� �����Ǵ� �⺻ �������̴�.
            if (recipeData.Value.recipeCost != 0)
                _recipeDict.Add(recipeData.Key, false);
            else
                _recipeDict.Add(recipeData.Key, true);
        }
    }
    
    //������ ȹ�� �Լ�
    public void AddRecipe(int recipeID)
    {
        
        if (_recipeDict == null) return;

        if(_recipeDict.ContainsKey(recipeID))
            _recipeDict[recipeID] = true;
    }
    //������ ���� Ȯ�� �Լ�
    public bool HasRecipe(int recipeID)
    {
        return _recipeDict != null &&
                _recipeDict.TryGetValue(recipeID, out bool hasRecipe) &&
                hasRecipe;
    }

    //�������� �����ǿ� �̺����������� ����Ʈ�� �и��ؼ� ��ȯ�Ѵ�
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
    //��� �Ҹ� �Լ�
    public void ConsumeGold(int value)
    {
        _currentGold -= value;
        GameManager.GM.SM.SavePlayInfo();
    }
    //��� ȹ�� �Լ�
    public void IncrementGold(int value)
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
    public void ResetInfo()
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        GameManager.GM.BM.ResetBuffList();
        GameManager.GM.SM.SavePlayInfo();
        IntializeRecipeDict();
    }

}
