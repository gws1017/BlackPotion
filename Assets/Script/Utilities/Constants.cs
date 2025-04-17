

public static class Constants
{
    //Play Information
    public const int RESTART_GOLD = 50; //재시작 골드
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
                return "NoUncommonrmal";
            case RecipeGrade.Legendary:
                return "Legendary";
        }
        return "";
    }

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

    //Scene

    public const string GAME_PLAY_SCENE = "GamePlayScene";
    public const string MAIN_MENU_SCENE = "MainMenuScene";

    //Ingridient Slot
    public const int INGRIDIENT_REFILL_GOLD = 10;
    public const int INGRIDIENT_MAX_NUMBER = 10;
    public const int INGRIDIENT_SUM_NUMBER = ((INGRIDIENT_MAX_NUMBER + 1) * INGRIDIENT_MAX_NUMBER) / 2;
}
