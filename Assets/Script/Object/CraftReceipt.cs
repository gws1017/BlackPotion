using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftReceipt : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    //���۷���
    private PlayInfo _playInfo;
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

    //��ǥ �ݾ�
    [SerializeField]
    private int[] _targetMoney;
    //��ǥ �޼� ����
    private bool _targetSucc;

    public bool TargetSuccess
    {
        get { return _targetSucc; }
    }

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
        _board = GameManager.GM.Board;
        _playInfo = GameManager.GM.PlayInfomation;
        _nextButton.onClick.AddListener(ShowRecipeStore);
    }

    //���� ������ ������Ʈ�Ѵ�
    public void UpdateReceipt()
    {
        var questList = _board.AcceptQuestList;
        var questResult = _board._questResultDict;
        int totalGold = 0;

        //������ ���� ���� ��尡 �ƴ϶� �Ƿ� �������ο� ���� ���͸� üũ�Ѵ�
        //�������� ����� ���� ������� �ʴ´�.
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].text = questList[i].PotionName + " ����";
            int gold = 0;

            //�Ƿں� ��� UI ������Ʈ
            if (questResult[questList[i]])
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
            _moneyText[i].text = gold.ToString() + "G";
        }

        //������ �Ƿ� ����ŭ �� �����ش�
        for (int i = 0; i < questList.Count; i++)
        {
            _potionNameText[i].enabled = true;
            _moneyText[i].enabled = true;
        }
        for (int i = questList.Count; i < _potionNameText.Length; i++)
        {
            _potionNameText[i].enabled = false;
            _moneyText[i].enabled = false;
        }

        if(totalGold >= _targetMoney[_playInfo.CurrentDay])
        {
            //�̹��� ��θ� �����ϴ� Ŭ������ ���θ����?
            //�ƴϸ� #define?
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetSucc");
            _targetSucc = true;
        }
        else
        {
            _resultImage.sprite = Resources.Load<Sprite>("Images/targetFail");
            _targetSucc = false;
        }
        _targetMoneyText.text = totalGold.ToString() + " / " + _targetMoney[_playInfo.CurrentDay].ToString();
    }

    //�����ǻ��� ����
    public void ShowRecipeStore()
    {
        _storeUI.OpenStoreUI(Store.StoreType.Recipe);
    }
}
