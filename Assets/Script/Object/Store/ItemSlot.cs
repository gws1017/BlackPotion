using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//���� �Ǹ� ��ǰ ��ư �ϳ��� �Ҵ�Ǵ� Ŭ����
public class ItemSlot : MonoBehaviour
{
    //�Ǹ� ��ǰ�� ����
    public int _itemId;
    public int _itemCost;
    public Image _itemImage;
    public Text _itemNameText;
    public Text _itemCostText;
    public Button _itemSlotButton;

    //������ ���۷���
    public Store _parentStore;
    private PlayInfo _playInfo;

    void Start()
    {
        _playInfo = GameManager.GM.PlayInformation;
        _itemSlotButton.onClick.AddListener(ShowPurchaseUI);
    }

    //�ǸŵǴ� ���������� �ʱ�ȭ
    public void InitializeItemSlot(BuffInfo ItemInfo)
    {
        _itemId = ItemInfo.buffId;
        _itemImage.sprite = Resources.Load<Sprite>(ItemInfo.buffImage);
        _itemNameText.text = ItemInfo.buffName;
        _itemCost = ItemInfo.buffCost;
        _itemCostText.text = "���� : " + _itemCost.ToString() + "���";
        _parentStore._puchaseCancelButton.onClick.RemoveAllListeners();
        _parentStore._puchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
    }
    //�ǸŵǴ� ���Ƿ����� ������ �ʱ�ȭ
    public void InitializeItemSlot(PotionInfo ItemInfo)
    {
        _itemId = ItemInfo.potionId;
        _itemImage.sprite = Resources.Load<Sprite>(ItemInfo.potionImage);
        _itemNameText.text = ItemInfo.potionName;
        _itemCost = ItemInfo.recipeCost;
        _itemCostText.text = "���� : " + _itemCost.ToString() + "���";
        _parentStore._puchaseCancelButton.onClick.RemoveAllListeners();
        _parentStore._puchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
    }
    //���� Ȯ�� â ����
    public void ShowPurchaseUI()
    {
        _parentStore._purchaseUI.SetActive(true);
        _parentStore._ConfirmText.text = _itemNameText.text + "�� " + " �����մϴ�.";
        _parentStore._purchaseAcceptButton.onClick.RemoveAllListeners();
        _parentStore._purchaseAcceptButton.onClick.AddListener(AcceptPurchase);
    }

    //���� Ȯ�� â �ݱ�
    public void ClosePurchaseUI()
    {
        _parentStore._purchaseUI.SetActive(false);
    }

    //���� Ȯ��
    public void AcceptPurchase()
    {
        int currGold = _playInfo.CurrentGold;

        if (currGold >= _itemCost)
        {
            if (_parentStore.SType == Store.StoreType.Recipe)
            {
                _playInfo.AddRecipe(_itemId);
            }
            else if (_parentStore.SType == Store.StoreType.Buff)
            {
                if (GameManager.GM.BM.IsFullBuffList())
                {
                    _parentStore._purchaseUI.SetActive(false);
                    return;
                }
                GameManager.GM.BM.AddBuff(_itemId);
            }
            _playInfo.ConsumeGold(_itemCost);
            _itemSlotButton.enabled = false;
            _itemNameText.text = "����";
        }
        else
        {
            //���� UI�� ǥ��
            Debug.Log("��尡 �����մϴ�.");
        }
        _parentStore._purchaseUI.SetActive(false);
    }
}
