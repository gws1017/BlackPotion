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
        //JSON에서 데이터 읽기
        _questID = Random.Range(1, 1000);
        _questText.text = "제임스";
        _potionQualityValue.text = "( " + _minPotionQuality.ToString() + " ~ " + _maxPotionQuality.ToString() + " )";
        //_potionImage
        CheckQuestGrade();
        _reqPotionQuality = Random.Range(_minPotionQuality, _maxPotionQuality);
    }


    private void CheckQuestGrade()
    {
        if (_questGrade >= 1 && _questGrade <= 3)
        {
            _gradeColor.color = Color.black;
        }
        else if (_questGrade > 3 && _questGrade <= 5)
        {
            _gradeColor.color = Color.magenta;
        }
        else if (_questGrade > 5 && _questGrade <= 7)
        {
            _gradeColor.color = Color.yellow;

        }
        else if (_questGrade > 7 && _questGrade <= 10)
        {
            _gradeColor.color = Color.red;

        }
    }
}
