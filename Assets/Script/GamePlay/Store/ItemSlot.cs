using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    //판매 제품의 정보
    public int ItemId;
    public int ItemCost;
    public Image ItemImage;
    public Text ItemNameText;
    public Text ItemCostText;
    public Button ItemSlotButton;

    public float _startY;
    public Store ParentStore;
    private PlayInfo _playInfo;
    [SerializeField] private Image _soldOutImage;
    [SerializeField] private float _floatSpeed = 1.0f;
    [SerializeField] private float _floatDistance = 0.5f;
    [SerializeField] private GameObject _purchaseFailUIPrefab;

    void Start()
    {
        _playInfo = GameManager.GM.PlayInformation;
        ItemSlotButton.onClick.AddListener(ShowPurchaseUI);
    }

    public void InitializeItemSlot(BuffInfo buffInfo)
    {
        SetupItemSlot(buffInfo.buffId, buffInfo.buffCost, buffInfo.buffImage, buffInfo.buffName);
        ItemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(258.0f,433.0f);

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
        ItemCostText.text = $"가격 : {ItemCost}골드";

        ParentStore.PurchaseCancelButton.onClick.RemoveAllListeners();
        ParentStore.PurchaseCancelButton.onClick.AddListener(ClosePurchaseUI);
        _soldOutImage.gameObject.SetActive(false);
        StartBoxFloat();
    }

    public void ShowPurchaseUI()
    {
        ParentStore.PurchaseUI.SetActive(true);
        ParentStore.ConfirmText.text = $"{ItemNameText.text}를 구매합니다.";
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
            StopBoxFloat();
            _playInfo.ConsumeGold(ItemCost);
            _soldOutImage.gameObject.SetActive(true);
            ItemSlotButton.interactable = false;
            ItemNameText.text = "매진";
            SoundManager._Instance.PlaySFX2D(SFXType.Money);

        }
        else
        {
            //추후 UI로 표기
            GameObject uiPrefab = Instantiate(_purchaseFailUIPrefab, ParentStore.StoreCanvas.transform);
            Vector3 pos = uiPrefab.transform.localPosition;
            pos.y = -7;
            pos.z = -7;
            uiPrefab.transform.localPosition = pos;
            uiPrefab.transform.localScale = Vector3.one*5;
            uiPrefab.GetComponent<ConfirmUI>().InitializeUI("골드가 부족합니다.");
        }
        ParentStore.PurchaseUI.SetActive(false);
    }

    public void StartBoxFloat()
    {
        StartCoroutine(FloatingBox());
    }

    public void StopBoxFloat()
    {
        StopCoroutine(FloatingBox());
    }
    private IEnumerator FloatingBox()
    {
        while(true)
        {
            float delta = 0;
            float direction = 1;
            while(true)
            {
                delta += direction * _floatSpeed * Time.deltaTime;
                delta = Mathf.Clamp(delta, -_floatDistance, _floatDistance);

                Vector3 boxPosition = gameObject.transform.position;
                boxPosition.y += delta;
                gameObject.transform.position = boxPosition;

                if (boxPosition.y >= _startY+_floatDistance) direction = -1;
                else if(boxPosition.y <= _startY) direction = 1;

                yield return null;
            }
        }
    }
}
