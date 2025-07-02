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
    [SerializeField] private GameObject[] _rackObjects;
    [SerializeField] private GameObject _itemSlotPrefab;
    private List<GameObject> _itemSlotsList;
    [SerializeField] List<int> _itemIdList;
    [SerializeField] private StoreType _storeType;
    [SerializeField] private Vector3 _slotStartPosition = new Vector3(-600, 145, 0);
    [SerializeField] private Vector2 _boxOffset = new Vector2(400,-445);
    [SerializeField] private int _boxCountPerLine = 3;
    [SerializeField] private float _itemBoxYOffset = 0.0f;

    public Canvas StoreCanvas => _canvas;
    public StoreType SType => _storeType;
    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
    }

    public void OpenStoreUI(StoreType type)
    {
        gameObject.SetActive(true);
        InitializeStore(type);
        GameManager.GM.BM.DisableBuffInventory();
    }

    public void CloseStoreUI()
    {
        GameManager.GM.BM.EnableBuffInventory();
        foreach (var itemSlot in _itemSlotsList)
        {
            itemSlot.GetComponent<ItemSlot>().StopBoxFloat();
            Destroy(itemSlot);
        }
        Destroy(gameObject);
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
                gameObject.transform.SetPositionAndRotation(new Vector3(20, 0, -40), Quaternion.Euler(0, 90, 0));
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
                gameObject.transform.SetPositionAndRotation(new Vector3(0, 0, -80), Quaternion.Euler(0, 180, 0));

                break;
            default:
                break;
        }

        for (int i = 0; i < maxItems; ++i)
            InitializeItemSlot(i);

    }

    private void InitializeItemSlot(int index)
    {
        Vector3 position = _slotStartPosition + new Vector3((index % _boxCountPerLine) * _boxOffset.x, index / _boxCountPerLine * _boxOffset.y, 0);


        var slotInstance = Instantiate(_itemSlotPrefab, _itemListObject.transform);
        slotInstance.transform.SetLocalPositionAndRotation(position, Quaternion.identity);

        _itemSlotsList.Add(slotInstance);

        ItemSlot itemSlotClass = slotInstance.GetComponent<ItemSlot>();
        itemSlotClass.ParentStore = this;

        Vector3 slotPosition = slotInstance.transform.position;

        int rackID = (index / _boxCountPerLine);
        itemSlotClass._startY = _rackObjects[rackID].transform.position.y + _itemBoxYOffset;
        slotPosition.y = itemSlotClass._startY;
        slotInstance.transform.position = slotPosition;

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
        SoundManager._Instance.PlayClickSound();
        CloseStoreUI();
        GameManager.GM.Brewer.GetNextCraft();
    }

    private void NextDay()
    {
        SoundManager._Instance.PlayClickSound();
        CloseStoreUI();
        GameManager.GM.TryNextDay();
    }
}
