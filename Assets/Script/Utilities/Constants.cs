

using UnityEngine;
using UnityEngine.UI;
using static Constants;

public static class Constants
{
    //Play Information
    public const int BASE_GOLD = 50; //±âº» Áö±Þ °ñµå(½ÃÀÛ°ñµå)
    public const int RETRY_GOLD = 50; //Àç½ÃÀÛ °ñµå
    public const int MAX_RECIPE_GRADE = 4; //·¹½ÃÇÇ ÃÖ°í µî±Þ
    public const int MAX_ACCEPT_QUEST_COUNT = 5;
    public const int MAX_BUFF_COUNT = 3;
    public const int MAX_QUEST_COUNT_LAYER = 5;
    public const float QUEST_PENALTY_RATIO = 0.3f;
    public const float CRITICAL_SUCCESS = 1.5f; //´ë¼º°ø

    public enum QuestGrade
    {
        Small,
        Middle,
        Large
    }

    public enum RecipeGrade
    {
        Common,
        Uncommon,
        Rare,
        Unique,
        Legendary
    }
    public static string RecipeGradeToString(RecipeGrade grade)
    {
        switch (grade)
        {
            case RecipeGrade.Common:
                return "ÈçÇÑ";
            case RecipeGrade.Uncommon:
                return "¾ÈÈçÇÑ";
            case RecipeGrade.Rare:
                return "Èñ±Í";
            case RecipeGrade.Unique:
                return "°íÀ¯";
            case RecipeGrade.Legendary:
                return "Àü¼³";
        }
        return "";
    }
    public static Color32 RecipeGradeToColor(RecipeGrade grade)
    {
        switch (grade)
        {
            case RecipeGrade.Common:
                return COMMON_COLOR;
            case RecipeGrade.Uncommon:
                return UNCOMMON_COLOR;
            case RecipeGrade.Rare:
                return RARE_COLOR;
            case RecipeGrade.Unique:
                return UNIQUE_COLOR;
            case RecipeGrade.Legendary:
                return LEGEND_COLOR;
            default:
                return COMMON_COLOR;
        }
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
            case RecipeGrade.Common:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_NORMAL);
                break;
            case RecipeGrade.Uncommon:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_COMMON);
                break;
            case RecipeGrade.Rare:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_RARE);
                break;
            case RecipeGrade.Unique:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_UNCOMMON);
                break;
            case RecipeGrade.Legendary:
                recipeImage.sprite = Resources.Load<Sprite>(PathHelper.RECIPE_ICON_LEGEND);
                break;
        }
    }

    public static Sprite GetQuestGradeMark(QuestGrade _questGrade)
    {
        switch (_questGrade)
        {
            case QuestGrade.Small:
                return Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_SMALL);
            case QuestGrade.Middle:
                return Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_MEDIUM);
            case QuestGrade.Large:
                return Resources.Load<Sprite>(PathHelper.QUEST_GRADE_MARK_LARGE);
            default:
                Debug.LogWarning("Quest Grade value error");
               return null;
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

    public static readonly Color32 COMMON_COLOR = new Color32(160, 160, 160, 255);
    public static readonly Color32 UNCOMMON_COLOR = new Color32(170, 220, 120, 255);
    public static readonly Color32 RARE_COLOR = new Color32(80, 160, 255, 255);
    public static readonly Color32 UNIQUE_COLOR = new Color32(170, 100, 220, 255);
    public static readonly Color32 LEGEND_COLOR = new Color32(255, 153, 0, 255);

    public const int UI_SCALE = 128;
}
