using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftResult : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private PotionBrewer _brewer;

    //UI
    [SerializeField]
    private GameObject _rewardButtons;
    [SerializeField]
    private Text _questResultText;
    [SerializeField]
    private Text _moneyValueText;
    [SerializeField]
    private Text _recipeNameText;
    [SerializeField]
    private Text _selectText;

    [SerializeField]
    private Text _potionQualityValueText;
    [SerializeField]
    private Text _resultText;

    [SerializeField]
    private Button _selectButton;
    [SerializeField]
    private Button _moneyButton;
    [SerializeField]
    private Button _recipeButton;
    [SerializeField]
    private Outline _moneyOutline;
    [SerializeField]
    private Outline _recipeOutline;

    private int _selectId;
    private bool _result;

    public int PotionQuality
    {
        set { _potionQualityValueText.text = value.ToString(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvas.worldCamera = GameManager.MainCamera;

        _selectButton.onClick.AddListener(RewardSelect);
        _moneyButton.onClick.AddListener(SelectMoney);
        _recipeButton.onClick.AddListener(SelectRecipe);
    }

    public void ShowCraftResult(bool result)
    {
        _result = result;
        gameObject.SetActive(true);

        var quest = _brewer._currentQuest;

        if (result)
        {
            _rewardButtons.SetActive(true);
            _moneyValueText.text = quest.QuestRewardMoney.ToString() + " ���";
            _recipeNameText.text = quest.QuestGrade.ToString() + " ������";
            _selectText.text = "����";
            _questResultText.text = "�Ƿ� ����";
            _resultText.text = "����";
            GameManager.GM._playInfo._currentMoney += quest.QuestRewardMoney;
            GameManager.GM._playInfo._questSuccCnt++;
        }
        else
        {
            _rewardButtons.SetActive(false);
            _selectText.text = "����";
            _questResultText.text = "�Ƿ� ����";
            _resultText.text = "����";
            //���̳ʽ� ��带 ������ ���ΰ�?
            GameManager.GM._playInfo._currentMoney -= (int)(quest.QuestRewardMoney * 0.1);
        }
    }

    public void RewardSelect()
    {
        if (_result)
        {
            if (_selectId == 1)
            {
                Debug.Log(_brewer._currentQuest.QuestRewardMoney.ToString() + " ȹ���߽��ϴ�.");
            }
            else if (_selectId == 2)
            {
                Debug.Log(_brewer._currentQuest.QuestGrade.ToString() + " ������ �رݵǾ����ϴ�.");
            }
            else return;
        }

        _brewer.GetNextCraft();
        gameObject.SetActive(false);
    }

    public void SelectMoney()
    {
        // ���̶���Ʈ ȿ��
        _selectId = 1;
        _moneyOutline.effectColor = Color.black;
        _recipeOutline.effectColor = Color.white;
    }

    public void SelectRecipe()
    {
        _selectId = 2;
        _moneyOutline.effectColor = Color.white;
        _recipeOutline.effectColor = Color.black;
    }


}
