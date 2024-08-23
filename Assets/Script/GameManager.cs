using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static Camera _camera;

    private static QuestBoard _board;
    private static PotionBrewer _brewer;

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
}
