using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int _itemId;
    public int _itemCost;
    public Image _itemImage;
    public Text _itemNameText;
    public Text _itemCostText;
    public Button _itemSlotButton;

    public Store _parentStore;


   

    void Start()
    {
        _itemSlotButton.onClick.AddListener(ShowPurchaseUI);
        
    }

    public void InitializeItemSlot(BuffInfo ItemInfo)
    {
        _itemId = ItemInfo.buffId;
        _itemImage.sprite = Resources.Load<Sprite>(ItemInfo.buffImage);
        _itemNameText.text = ItemInfo.buffName;
        _itemCost = ItemInfo.buffCost;
        _itemCostText.text = "가격 : " + _itemCost.ToString() + "골드";
        _parentStore._puchaseCancelButton.onClick.RemoveAllListeners();
        _parentStore._puchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
    }

    public void InitializeItemSlot(PotionInfo ItemInfo)
    {
        _itemId = ItemInfo.potionId;
        _itemImage.sprite = Resources.Load<Sprite>(ItemInfo.potionImage);
        _itemNameText.text = ItemInfo.potionName;
        _itemCost = ItemInfo.recipeCost;
        _itemCostText.text = "가격 : " + _itemCost.ToString() + "골드";
        _parentStore._puchaseCancelButton.onClick.RemoveAllListeners();
        _parentStore._puchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
    }
    //구매 확인 창 오픈
    public void ShowPurchaseUI()
    {
        _parentStore._purchaseUI.SetActive(true);
        _parentStore._ConfirmText.text = _itemNameText.text + "를 " + " 구매합니다.";
        _parentStore._purchaseAcceptButton.onClick.RemoveAllListeners();
        _parentStore._purchaseAcceptButton.onClick.AddListener(AcceptPurchase);
    }

    //구매 확인 창 닫기
    public void ClosePurchaseUI()
    {
        _parentStore._purchaseUI.SetActive(false);
    }

    //구매 확정
    public void AcceptPurchase()
    {
        int currGold = GameManager.GM._playInfo._currentMoney;

        if (currGold >=_itemCost)
        {
            GameManager.GM._playInfo._currentMoney -= _itemCost;
            Debug.Log("골드 " + _itemCost.ToString() + " 소모");
            Debug.Log("현재 골드 " + GameManager.GM._playInfo._currentMoney.ToString());
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
        _parentStore._purchaseUI.SetActive(false);
    }
}
