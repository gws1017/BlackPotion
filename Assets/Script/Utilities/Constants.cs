

using UnityEngine;
using UnityEngine.UI;

public static class Constants
{
    //Play Information
    public const int RETRY_GOLD = 50; //재시작 골드
    public const int MAX_RECIPE_GRADE = 4; //레시피 최고 등급
    public const int MAX_ACCEPT_QUEST_COUNT = 5;
    public const int MAX_BUFF_COUNT = 3;
    public const int MAX_QUEST_COUNT_LAYER = 5;
    public const float QUEST_PENALTY_RATIO = 0.1f;
    public const float CRITICAL_SUCCESS = 1.5f; //대성공

    public enum QuestGrade
    {
        Small,
        Middle,
        Large
    }

    public enum RecipeGrade
    {
        Normal,
        Common,
        Rare,
        Uncommon,
        Legendary
    }
    public static string RecipeGradeToString(RecipeGrade grade)
    {
        switch (grade)
        {
            case RecipeGrade.Normal:
                return "Normal";
            case RecipeGrade.Common:
                return "Common";
            case RecipeGrade.Rare:
                return "Rare";
            case RecipeGrade.Uncommon:
                return "Uncommon";
            case RecipeGrade.Legendary:
                return "Legendary";
        }
        return "";
    }

    public struct PotionCraftGrade
    {
        public const string RANK_A = "A";
        public const string RANK_BP = "B+";
        public const string RANK_B = "B";
        public const string RANK_CP = "C+";
        public const string RANK_C = "C";
        public const string RANK_F = "F";

        public const int BORDER_A = 80;
        public const int BORDER_BP = 60;
        public const int BORDER_B = 40;
        public const int BORDER_CP = 20;
        public const int BORDER_C = 0;
    };

    public static string CheckPotionCraftGrade(float qualityPercent)
    {
        if (qualityPercent > PotionCraftGrade.BORDER_A)
        {
            return PotionCraftGrade.RANK_A;
        }
        else if (qualityPercent > PotionCraftGrade.BORDER_BP && qualityPercent <= PotionCraftGrade.BORDER_A)
        {
            return PotionCraftGrade.RANK_BP;
        }
        else if (qualityPercent > PotionCraftGrade.BORDER_B && qualityPercent <= PotionCraftGrade.BORDER_BP)
        {
            return PotionCraftGrade.RANK_B;
        }
        else if (qualityPercent > PotionCraftGrade.BORDER_CP && qualityPercent <= PotionCraftGrade.BORDER_B)
        {
            return PotionCraftGrade.RANK_CP;
        }
        else if (qualityPercent >= PotionCraftGrade.BORDER_C && qualityPercent <= PotionCraftGrade.BORDER_CP)
        {
            return PotionCraftGrade.RANK_C;
        }
        else return PotionCraftGrade.RANK_F;
    }

    public static void SetRecipeIcon(UnityEngine.UI.Image recipeImage,int recipeGrade)
    {
        RecipeGrade grade = (RecipeGrade)recipeGrade;
        switch (grade)
        {
            case RecipeGrade.Normal:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_NORMAL);
                break;
            case RecipeGrade.Common:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_COMMON);
                break;
            case RecipeGrade.Rare:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_RARE);
                break;
            case RecipeGrade.Uncommon:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_UNCOMMON);
                break;
            case RecipeGrade.Legendary:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_LEGEND);
                break;
        }
    }

    //Scene

    public const string GAME_PLAY_SCENE = "GamePlayScene";
    public const string MAIN_MENU_SCENE = "MainMenuScene";

    //Ingridient Slot
    public const int INGRIDIENT_REFILL_GOLD = 10;
    public const int INGRIDIENT_MAX_NUMBER = 10;
    public const int INGRIDIENT_SUM_NUMBER = ((INGRIDIENT_MAX_NUMBER + 1) * INGRIDIENT_MAX_NUMBER) / 2;

    public static readonly Color32 POTION_SUCC_GREEN = new Color32(99, 164, 14, 255);
    public static readonly Color32 REWARD_SELECT_HILIGHT = new Color32(177, 18, 33, 255);
}
