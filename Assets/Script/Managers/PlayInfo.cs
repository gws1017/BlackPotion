using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static PlayInfo;

public class PlayInfo : MonoBehaviour
{

    public const string TARGET_SUCCESS_IMAGE = "Images/targetSuccess";
    public const string TARGET_FAIL_IMAGE = "Images/targetFail";

    public const int RESTART_GOLD = 50; //����� ���
    public const int MAX_RECIPE_GRADE = 4; //������ �ְ� ���
    public const int MAX_ACCEPT_QUEST_COUNT = 5;
    public const int MAX_BUFF_COUNT = 3;
    public const int MAX_QUEST_COUNT_LAYER = 5;
    public const float QUEST_PENALTY_RATIO = 0.1f;
    public const float CRITICAL_SUCCESS = 1.5f; //�뼺��

    public struct PotionCraftGrade
    {
        public const string RANK_1 = "A";
        public const string RANK_2 = "B+";
        public const string RANK_3 = "B";
        public const string RANK_4 = "C+";
        public const string RANK_5 = "C";

        public const int BORDER_1 = 100;
        public const int BORDER_2 = 80;
        public const int BORDER_3 = 60;
        public const int BORDER_4 = 40;
        public const int BORDER_5 = 20;
    };

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
        GameManager.GM.BM.ClearBuffList();
        GameManager.GM.SM.SavePlayInfo();
        IntializeRecipeDict();
    }
    public static string CheckPotionCraftGrade(float qualityPercent)
    {
        if (qualityPercent <= PotionCraftGrade.BORDER_5)
        {
            return PotionCraftGrade.RANK_5;
        }
        else if (qualityPercent <= PotionCraftGrade.BORDER_4 && qualityPercent > PotionCraftGrade.BORDER_5 + 1)
        {
            return PotionCraftGrade.RANK_4;
        }
        else if (qualityPercent <= PotionCraftGrade.BORDER_3 && qualityPercent > PotionCraftGrade.BORDER_4 + 1)
        {
            return PotionCraftGrade.RANK_3;
        }
        else if (qualityPercent <= PotionCraftGrade.BORDER_2 && qualityPercent > PotionCraftGrade.BORDER_3 + 1)
        {
            return PotionCraftGrade.RANK_2;
        }
        else if (qualityPercent <= PotionCraftGrade.BORDER_1 && qualityPercent > PotionCraftGrade.BORDER_2 + 1)
        {
            return PotionCraftGrade.RANK_1;
        }
        return PotionCraftGrade.RANK_5;
    }

}
