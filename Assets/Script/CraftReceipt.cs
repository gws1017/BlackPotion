using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    private QuestBoard _board;

    [SerializeField]
    private Store _storeUI;

    //UI
    [SerializeField]
    private Button _nextButton;
    [SerializeField]
    private Text[] _potionNameText;
    [SerializeField]
    private Text[] _moneyText;
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private Text _targetMoneyText;

    [SerializeField]
    private int[] _targetMoney;

    // Start is called before the first frame update
    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;
        _nextButton.onClick.AddListener(ShowRecipeStore);
    }

    public void UpdateReceipt()
    {
        var questList = _board._accpetQuestList;
        var info = _board._questResultDict;
        int totalGold = 0;
        //������ ���� ���� ��尡 �ƴ϶� �Ƿ� �������ο� ���� ���͸� üũ�Ѵ�
        //�������� ����� ���� ������� �ʴ´�.
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].text = questList[i].PotionName + " ����";
            int gold = 0;
            if (info[questList[i]])
            {
                _potionNameText[i].color = Color.green;
                _moneyText[i].color = Color.green;
                gold = questList[i].QuestRewardMoney;
            }
            else
            {
                _potionNameText[i].color = Color.red;
                _moneyText[i].color = Color.red;
                //�Ƿ� ���н� ������� 10�ۼ�Ʈ ����� ����
                gold -= (int)(questList[i].QuestRewardMoney * 0.1);
            }
            totalGold += gold;
            _moneyText[i].text = questList[i].QuestRewardMoney.ToString() + "G";
        }

        for (int i = questList.Count; i < _potionNameText.Length; i++)
        {
            _potionNameText[i].enabled = false;
            _moneyText[i].enabled = false;
        }

        if(totalGold >= _targetMoney[GameManager.GM._playInfo.CurrentDay])
        {
            //�̹��� ��θ� �����ϴ� Ŭ������ ���θ����?
            //�ƴϸ� #define?
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetSucc");
        }
        else
        {
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetFail");
        }
        _targetMoneyText.text = totalGold.ToString() + " / " + _targetMoney[GameManager.GM._playInfo.CurrentDay].ToString();
    }

    public void ShowRecipeStore()
    {
        _storeUI.OpenStoreUI(Store.StoreType.Recipe);
    }
}
