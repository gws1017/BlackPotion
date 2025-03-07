using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//상점 판매 제품 버튼 하나에 할당되는 클래스
public class ItemSlot : MonoBehaviour
{
    //판매 제품의 정보
    public int _itemId;
    public int _itemCost;
    public Image _itemImage;
    public Text _itemNameText;
    public Text _itemCostText;
    public Button _itemSlotButton;

    //상점의 레퍼런스
    public Store _parentStore;
    private PlayInfo _playInfo;

    void Start()
    {
        _playInfo = GameManager.GM.PlayInformation;
        _itemSlotButton.onClick.AddListener(ShowPurchaseUI);
    }

    //판매되는 버프정보를 초기화
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
    //판매되는 포션레시피 정보를 초기화
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
            _itemNameText.text = "매진";
        }
        else
        {
            //추후 UI로 표기
            Debug.Log("골드가 부족합니다.");
        }
        _parentStore._purchaseUI.SetActive(false);
    }
}
