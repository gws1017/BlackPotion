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
    private Text _nextText;
    [SerializeField]
    private Button _nextButton;

    //������ ���
    [SerializeField]
    GameObject _itemListObject;
    //ǥ���� ������ ID ����Ʈ
    [SerializeField]
    List<int> _ItemIdList;
    [SerializeField]
    GameObject _itemSlotPrefab;

    //���� Ȯ��â UI
    [Header("Purchase UI")]
    public GameObject _purchaseUI;
    public Button _puchaseCancelButton;
    public Button _purchaseAcceptButton;
    public Text _ConfirmText;

    private List<GameObject> _itemSlotsList;
   
    public StoreType SType
    {
        get { return _storeType; }
    }

    void Start()
    {
        _canvas.worldCamera = GameManager.GM.MainCamera;
    }

    public void InitializeStore(StoreType type)
    {
        //_nextButton.enabled = true;
        _nextButton.onClick.RemoveAllListeners();
        _storeType = type;
        _ItemIdList = new List<int>();
        _itemSlotsList = new List<GameObject>();
        int itemCnt = 0;
        if (_storeType == StoreType.Buff)
        {
            _titleText.text = "���� ����";
            _nextText.text = "���� �Ƿ�";
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
            _titleText.text = "������ ����";
            _nextText.text = "���� ��";
            _nextButton.onClick.AddListener(NextDay);

            //�̺��� �����Ǹ� ������ ����
            foreach (var ID in ReadJson._dictPotion.Keys)
            {
                if(GameManager.GM._playInfo.HasRecipe(ID) == false)
                    _ItemIdList.Add(ID);
            }

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

        //������ ������ �����ϰ� ��ġ�� �ʱ�ȭ
        var slotObject = Instantiate(_itemSlotPrefab);
        _itemSlotsList.Add(slotObject);
        slotObject.transform.SetParent(_itemListObject.transform);
        slotObject.transform.SetLocalPositionAndRotation(startPos + new Vector3((i%3) * 400, i / 3 * -445, 0), Quaternion.identity);
        slotObject.transform.localScale = Vector3.one;

        //������ ������ �ʱ�ȭ
        ItemSlot itemSlotClass = slotObject.GetComponent<ItemSlot>();
        itemSlotClass._parentStore = this;
        if(_storeType == StoreType.Buff)
            itemSlotClass.InitializeItemSlot(ReadJson._dictBuff[_ItemIdList[i]]);
        else if (_storeType == StoreType.Recipe)
            itemSlotClass.InitializeItemSlot(ReadJson._dictPotion[_ItemIdList[i]]);
    }
    //���� ���������� �̵�
    private void NextQuest()
    {
        Debug.Log("���� �Ƿڷ� �Ѿ�ϴ�");
        //_nextButton.enabled = false;
        CloseStoreUI();
        GameManager.GM.Brewer.GetNextCraft();
    }

    //���� ���� �̵� (�Ƿ� �Խ��� �̵�)
    private void NextDay()
    {
        CloseStoreUI();
        GameManager.GM.CheckRecipt();
    }

    //���� â ����
    public void OpenStoreUI(StoreType type)
    {
        gameObject.SetActive(true);
        InitializeStore(type);
    }

    //���� â �ݱ�
    public void CloseStoreUI()
    {
        Debug.Log("����â �ݽ��ϴ�.");
        foreach(var itemSlot in _itemSlotsList)
        {
            Destroy(itemSlot);
        }
        gameObject.SetActive(false);
    }

}
