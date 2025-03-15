using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    //�Ǹ� ��ǰ�� ����
    public int ItemId;
    public int ItemCost;
    public Image ItemImage;
    public Text ItemNameText;
    public Text ItemCostText;
    public Button ItemSlotButton;

    public Store ParentStore;
    private PlayInfo _playInfo;

    void Start()
    {
        _playInfo = GameManager.GM.PlayInformation;
        ItemSlotButton.onClick.AddListener(ShowPurchaseUI);
    }

    public void InitializeItemSlot(BuffInfo buffInfo)
    {
        SetupItemSlot(buffInfo.buffId, buffInfo.buffCost, buffInfo.buffImage, buffInfo.buffName);
    }

    public void InitializeItemSlot(PotionInfo potionInfo)
    {
        SetupItemSlot(potionInfo.potionId, potionInfo.recipeCost, potionInfo.potionImage, potionInfo.potionName);
    }

    private void SetupItemSlot(int id, int cost, string imagePath, string name)
    {
        ItemId = id;
        ItemCost = cost;
        ItemImage.sprite = Resources.Load<Sprite>(imagePath);
        ItemNameText.text = name;
        ItemCostText.text = $"���� : {ItemCost}���";

        ParentStore.PurchaseCancelButton.onClick.RemoveAllListeners();
        ParentStore.PurchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
    }

    public void ShowPurchaseUI()
    {
        ParentStore.PurchaseUI.SetActive(true);
        ParentStore.ConfirmText.text = $"{ItemNameText.text}�� �����մϴ�.";
        ParentStore.PurchaseAcceptButton.onClick.RemoveAllListeners();
        ParentStore.PurchaseAcceptButton.onClick.AddListener(AcceptPurchase);
    }

    public void ClosePurchaseUI()
    {
        ParentStore.PurchaseUI.SetActive(false);
    }

    public void AcceptPurchase()
    {
        int currentGold = _playInfo.CurrentGold;

        if (currentGold >= ItemCost)
        {
            if (ParentStore.SType == Store.StoreType.Recipe)
            {
                _playInfo.AddRecipe(ItemId);
            }
            else if (ParentStore.SType == Store.StoreType.Buff)
            {
                if (GameManager.GM.BM.IsFullBuffList())
                {
                    ParentStore.PurchaseUI.SetActive(false);
                    return;
                }
                GameManager.GM.BM.AddBuff(ItemId);
            }
            _playInfo.ConsumeGold(ItemCost);
            ItemSlotButton.interactable = false;
            ItemNameText.text = "����";
        }
        else
        {
            //���� UI�� ǥ��
            Debug.Log("��尡 �����մϴ�.");
        }
        ParentStore.PurchaseUI.SetActive(false);
    }
}
