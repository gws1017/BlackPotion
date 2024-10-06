using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    //Component
    [SerializeField]
    private Canvas _canvas;

    public enum StoreType
    {
        None,
        Buff,
        Recipe,
    }

    [SerializeField]
    private StoreType _storeType;

    [SerializeField]
    private Text _titleText;

    [SerializeField]
    private Button _nextButton;

    //아이템 목록
    [SerializeField]
    GameObject _itemListObject;
    //표시할 아이템 ID 리스트
    [SerializeField]
    List<int> _ItemIdList;
    [SerializeField]
    GameObject _itemSlotPrefab;

    //구매 확인창 UI
    [Header("Purchase UI")]
    public GameObject _purchaseUI;
    public Button _puchaseCancelButton;
    public Button _purchaseAcceptButton;
    public Text _ConfirmText;
   

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
    }

    public void InitializeStore(StoreType type)
    {
        _storeType = type;
        _ItemIdList = new List<int>();
        int itemCnt = 0;

        if (_storeType == StoreType.Buff)
        {
            _titleText.text = "버프 상점";
            _nextButton.onClick.AddListener(NextQuest);

            foreach (var ID in ReadJson._dictBuff.Keys)
                _ItemIdList.Add(ID);

            itemCnt = Mathf.Min(6, _ItemIdList.Count);
            for (int i = 0; i < itemCnt; ++i)
            {
                InitializeItemSlot(i);
            }
        }
        else if (_storeType == StoreType.Recipe)
        {
            _titleText.text = "레시피 상점";
            _nextButton.onClick.AddListener(NextDay);
            foreach (var ID in ReadJson._dictPotion.Keys)
                _ItemIdList.Add(ID);

            itemCnt = Mathf.Min(6, _ItemIdList.Count);
            for (int i = 0; i < itemCnt; ++i)
            {
                InitializeItemSlot(i);
            }
        }

    }

    private void InitializeItemSlot(int i)
    {
        Vector3 startPos = new Vector3(-600, 145, 0);

        //아이템 슬롯을 생성하고 위치를 초기화
        var slotObject = Instantiate(_itemSlotPrefab);
        slotObject.transform.SetParent(_itemListObject.transform);
        slotObject.transform.SetLocalPositionAndRotation(startPos + new Vector3(i * 400, i / 3 * -445, 0), Quaternion.identity);
        slotObject.transform.localScale = Vector3.one;

        //슬롯의 정보를 초기화
        ItemSlot itemSlotClass = slotObject.GetComponent<ItemSlot>();
        var ItemInfo = ReadJson._dictBuff[_ItemIdList[i]];
        itemSlotClass._parentStore = this;
        itemSlotClass.InitializeItemSlot(ItemInfo);
    }
    //다음 포션제조로 이동
    private void NextQuest()
    {
        Debug.Log("다음 의뢰로 넘어갑니다");
        CloseStoreUI();
        GameManager.GM.Brewer.GetNextCraft();
    }

    //다음 날로 이동 (의뢰 게시판 이동)
    private void NextDay()
    {
        CloseStoreUI();
    }

    //상점 창 오픈
    public void OpenStoreUI(StoreType type)
    {
        gameObject.SetActive(true);
        InitializeStore(type);
    }

    //상점 창 닫기
    public void CloseStoreUI()
    {
        Debug.Log("상점창 닫습니다.");
        gameObject.SetActive(false);
    }

}
