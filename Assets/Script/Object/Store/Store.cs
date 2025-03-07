using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    //상점 유형
    public enum StoreType
    {
        None,
        Buff,
        Recipe,
    }

    //Component
    [SerializeField]
    private Canvas _canvas;
    
    //UI
    [SerializeField]
    private Text _titleText;
    [SerializeField]
    private Text _nextText;
    [SerializeField]
    private Button _nextButton;

    //구매 확인창 UI
    [Header("Purchase UI")]
    public GameObject _purchaseUI;
    public Button _puchaseCancelButton;
    public Button _purchaseAcceptButton;
    public Text _ConfirmText;

    //아이템 목록
    [SerializeField]
    GameObject _itemListObject;
    //표시할 아이템 ID 리스트
    [SerializeField]
    List<int> _ItemIdList;
   
    //상점 Type
    [SerializeField]
    private StoreType _storeType;

    //아이템(UI) 프리펩
    [SerializeField]
    GameObject _itemSlotPrefab;
    //아이템(UI) 목록
    private List<GameObject> _itemSlotsList;
   
    public StoreType SType
    {
        get { return _storeType; }
    }

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
    }

    //상점 초기화 함수
    public void InitializeStore(StoreType type)
    {
        _nextButton.onClick.RemoveAllListeners();
        _storeType = type;
        _ItemIdList = new List<int>();
        _itemSlotsList = new List<GameObject>();

        int itemCnt = 0;
        //버프 상점
        if (_storeType == StoreType.Buff)
        {
            _titleText.text = "버프 상점";
            _nextText.text = "다음 의뢰";
            _nextButton.onClick.AddListener(NextQuest);

            foreach (var ID in ReadJson._dictBuff.Keys)
                _ItemIdList.Add(ID);

            itemCnt = Mathf.Min(6, _ItemIdList.Count);
            for (int i = 0; i < itemCnt; ++i)
            {
                InitializeItemSlot(i);
            }
        }
        //레시피 상점
        else if (_storeType == StoreType.Recipe)
        {
            _titleText.text = "레시피 상점";
            _nextText.text = "다음 날";
            _nextButton.onClick.AddListener(NextDay);

            //미보유 레시피만 상점에 등장
            foreach (var ID in ReadJson._dictPotion.Keys)
            {
                if(GameManager.GM.PlayInformation.HasRecipe(ID) == false)
                    _ItemIdList.Add(ID);
            }

            itemCnt = Mathf.Min(6, _ItemIdList.Count);
            for (int i = 0; i < itemCnt; ++i)
            {
                InitializeItemSlot(i);
            }
        }

    }

    //상점 판매 목록을 초기화함
    private void InitializeItemSlot(int i)
    {
        Vector3 startPos = new Vector3(-600, 145, 0);

        //아이템 슬롯을 생성하고 위치를 초기화
        var slotObject = Instantiate(_itemSlotPrefab);
        _itemSlotsList.Add(slotObject);
        slotObject.transform.SetParent(_itemListObject.transform);
        slotObject.transform.SetLocalPositionAndRotation(startPos + new Vector3((i%3) * 400, i / 3 * -445, 0), Quaternion.identity);
        slotObject.transform.localScale = Vector3.one;

        //슬롯의 정보를 초기화
        ItemSlot itemSlotClass = slotObject.GetComponent<ItemSlot>();
        itemSlotClass._parentStore = this;
        if(_storeType == StoreType.Buff)
            itemSlotClass.InitializeItemSlot(ReadJson._dictBuff[_ItemIdList[i]]);
        else if (_storeType == StoreType.Recipe)
            itemSlotClass.InitializeItemSlot(ReadJson._dictPotion[_ItemIdList[i]]);
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
        GameManager.GM.CheckRecipt();
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
        foreach(var itemSlot in _itemSlotsList)
        {
            Destroy(itemSlot);
        }
        gameObject.SetActive(false);
    }

}
