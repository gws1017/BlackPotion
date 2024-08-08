using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [SerializeField]
    private TextMesh _questText;
    [SerializeField]
    private TextMesh _potionQualityValue;

    [SerializeField]
    private Image _gradeColor;
    [SerializeField]
    private Image _potionImage;

    [ReadOnly,SerializeField]
    private int _questID;
    [SerializeField]
    private int _minPotionQuality;
    [SerializeField]
    private int _maxPotionQuality;
    [ReadOnly,SerializeField]
    private int _reqPotionQuality;
    [SerializeField]
    private int _questGrade;

    public int QuestID
    {
        get
        {
            return _questID;
        }
        set
        {
            _questID = value;
        }
    }
    public string QuestText
    {
        get
        {
            return _questText.text;
        }
        set
        {
            _questText.text = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //ID�� ������ �������� �ο�
        _questID = Random.Range(3001, 3004);
        InitilizeData();
        
    }
    private void InitilizeData()
    {
        var questInfo = ReadJson._dictQuest[_questID];
        _questText.text = questInfo.questText;
        _questGrade = questInfo.questGrade;

        //DB�� �ּ� �ִ� ���� ǰ������ ����
        _potionQualityValue.text = "( " + _minPotionQuality.ToString() + " ~ " + _maxPotionQuality.ToString() + " )";
        CheckQuestGrade();
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);
        //_potionImage
        //����ID�� ����ؼ� ������ ����

    }


    private void CheckQuestGrade()
    {
        if (_questGrade == 1)
        {
            _gradeColor.color = Color.black;
        }
        else if (_questGrade == 2 )
        {
            _gradeColor.color = Color.magenta;
        }
        else if (_questGrade == 3)
        {
            _gradeColor.color = Color.yellow;

        }
        else if (_questGrade == 4)
        {
            _gradeColor.color = Color.red;

        }
    }
}
