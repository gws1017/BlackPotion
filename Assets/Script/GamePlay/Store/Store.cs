using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    private const int MAX_ITEMS = 6;

    public enum StoreType
    {
        None,
        Buff,
        Recipe,
    }

    [Header("Compnent")]
    [SerializeField] private Canvas _canvas;

    [Header("UI")]
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _nextText;
    [SerializeField] private Button _nextButton;

    [Header("Purchase UI")]
    public GameObject PurchaseUI;
    public Button PurchaseCancelButton;
    public Button PurchaseAcceptButton;
    public Text ConfirmText;

    [Header("Store")]
    [SerializeField] private GameObject _itemListObject;
    [SerializeField] private GameObject _itemSlotPrefab;
    private List<GameObject> _itemSlotsList;
    [SerializeField] List<int> _itemIdList;
    [SerializeField] private StoreType _storeType;

    public StoreType SType => _storeType;
    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
    }

    public void OpenStoreUI(StoreType type)
    {
        gameObject.SetActive(true);
        InitializeStore(type);
    }

    public void CloseStoreUI()
    {
        foreach (var itemSlot in _itemSlotsList)
        {
            Destroy(itemSlot);
        }
        gameObject.SetActive(false);
    }

    private void InitializeStore(StoreType type)
    {
        _nextButton.onClick.RemoveAllListeners();
        _storeType = type;
        _itemIdList = new List<int>();
        _itemSlotsList = new List<GameObject>();

        int maxItems = 0;

        switch (_storeType)
        {
            case StoreType.Buff:
                _titleText.text = "버프 상점";
                _nextText.text = "다음 의뢰";
                _nextButton.onClick.AddListener(NextQuest);

                foreach (var ID in ReadJson._dictBuff.Keys)
                    _itemIdList.Add(ID);

                maxItems = Mathf.Min(MAX_ITEMS, _itemIdList.Count);
                break;

            case StoreType.Recipe:
                _titleText.text = "레시피 상점";
                _nextText.text = "다음 날";
                _nextButton.onClick.AddListener(NextDay);

                foreach (var ID in ReadJson._dictPotion.Keys)
                {
                    if (!GameManager.GM.PlayInformation.HasRecipe(ID))
                        _itemIdList.Add(ID);
                }
                maxItems = Mathf.Min(MAX_ITEMS, _itemIdList.Count);

                break;
            default:
                break;
        }

        for (int i = 0; i < maxItems; ++i)
            InitializeItemSlot(i);

    }

    private void InitializeItemSlot(int index)
    {
        Vector3 startPos = new Vector3(-600, 145, 0);
        Vector3 position = startPos + new Vector3((index % 3) * 400,index / 3 * -445, 0);

        var slotInstance = Instantiate(_itemSlotPrefab, _itemListObject.transform);
        slotInstance.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        slotInstance.transform.localScale = Vector3.one;
        _itemSlotsList.Add(slotInstance);

        ItemSlot itemSlotClass = slotInstance.GetComponent<ItemSlot>();
        itemSlotClass.ParentStore = this;
        switch (_storeType)
        {
            case StoreType.Buff:
                itemSlotClass.InitializeItemSlot(ReadJson._dictBuff[_itemIdList[index]]);
                break;
            case StoreType.Recipe:
                itemSlotClass.InitializeItemSlot(ReadJson._dictPotion[_itemIdList[index]]);
                break;
        }
    }

    private void NextQuest()
    {
        CloseStoreUI();
        GameManager.GM.Brewer.GetNextCraft();
    }

    private void NextDay()
    {
        CloseStoreUI();
        GameManager.GM.CheckRecipt();
    }
}
