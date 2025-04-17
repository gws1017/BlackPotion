using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayInfo : MonoBehaviour
{

    private int _currentCraftDay = 0;//���� ����
    private int _currentGold = 0;//���� �����ϰ� �ִ� ��差

    //���� ������
    //public DaylightTime _playTime;
    public int _questSuccCount = 0;
    public int _maxCraftDay = 0;
    
    private Dictionary<int, bool> _recipeDict; //���� ������ - ������ true �̺����� false

    //Getter Setter
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
    public void ResetInfo(bool isSave = true)
    {
        _currentCraftDay = 0;
        _currentGold = 0;
        GameManager.GM.BM.ClearBuffList();
        GameManager.GM.InitializeGameManager();
        if (isSave)
            GameManager.GM.SM.SavePlayInfo();
        IntializeRecipeDict();
    }
    public static string CheckPotionCraftGrade(float qualityPercent)
    {
        if (qualityPercent <= Constants.PotionCraftGrade.BORDER_5)
        {
            return Constants.PotionCraftGrade.RANK_5;
        }
        else if (qualityPercent <= Constants.PotionCraftGrade.BORDER_4 && qualityPercent > Constants.PotionCraftGrade.BORDER_5 + 1)
        {
            return Constants.PotionCraftGrade.RANK_4;
        }
        else if (qualityPercent <= Constants.PotionCraftGrade.BORDER_3 && qualityPercent > Constants.PotionCraftGrade.BORDER_4 + 1)
        {
            return Constants.PotionCraftGrade.RANK_3;
        }
        else if (qualityPercent <= Constants.PotionCraftGrade.BORDER_2 && qualityPercent > Constants.PotionCraftGrade.BORDER_3 + 1)
        {
            return Constants.PotionCraftGrade.RANK_2;
        }
        else if (qualityPercent <= Constants.PotionCraftGrade.BORDER_1 && qualityPercent > Constants.PotionCraftGrade.BORDER_2 + 1)
        {
            return Constants.PotionCraftGrade.RANK_1;
        }
        return Constants.PotionCraftGrade.RANK_5;
    }

}
