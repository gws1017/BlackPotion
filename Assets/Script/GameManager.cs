using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static Camera _camera;

    private static QuestBoard _board;
    private static PotionBrewer _brewer;

    //플레이 기록을 저장하는 클래스를 만들어서 추가하자
    //의뢰 누적성공횟수, 플레이타임, 포션제조 일차(한 게임에 몇일차까지 살아남았나) 등
    //최대 생존기간?

    [SerializeField]
    private Button _questStartButton;

    //Getter Setter
    public static Camera MainCamera
    {
        get
        {
            if(_camera == null)
            {
                _camera = GameObject.Find("Pixel Perfect Camera").GetComponent<Camera>();
            }
            return _camera;
        }
    }

    public static QuestBoard Board
    {
        get
        {
            if (_board == null)
            {
                _board = GameObject.Find("QuestBoard").GetComponent<QuestBoard>();
            }
            return _board;
        }
    }

    public static PotionBrewer Brewer
    {
        get
        {
            if (_brewer == null)
            {
                _brewer = GameObject.Find("PotionBrewer").GetComponent<PotionBrewer>();
            }
            return _brewer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _questStartButton.onClick.AddListener(QuestStart);
    }

    private void QuestStart()
    {
        if (_board.CurrrentAcceptQuestCnt > 0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
            Brewer.UpdateQuestInfo();
        }
        else
        {
            Debug.Log("의뢰를 1개이상 수주하셔야합니다.");
        }
    }

    public static void ShowCraftReceipt()
    {
        //CraftReceipt를 GameManger아래에두고 UpdateReceipt를 호출하고싶었으나
        //이 함수에서 호출시 CraftReceipt에 static을 붙이지않으면 static함수에서
        //호출할 수 없다 , CraftReceipt에대한 static 접근이 필요하지않은데 붙이는건
        //아니되므로, 이 함수를 호출하는 potionBrwer에 CraftReceipt를 두고 호출한다
        _camera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
