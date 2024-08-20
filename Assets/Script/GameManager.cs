using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static Camera _camera;

    [SerializeField]
    private QuestBoard _board;

    [SerializeField]
    private Button QuestStartButton;

    // Start is called before the first frame update
    void Start()
    {
        QuestStartButton.onClick.AddListener(QuestStart);
    }

    private void QuestStart()
    {
        if(_board.CurrrentAcceptQuestCnt>0)
        {
            _camera.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            Debug.Log("의뢰를 1개이상 수주하셔야합니다.");
        }
    }
}
