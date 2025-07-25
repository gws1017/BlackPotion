using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeObject : MonoBehaviour
{
    public int _recipeID;
    public Text _recipeName;
    public Text[] _materiallNames;
    public Text[] _ratioCategoryNames;
    public Text[] _ratioValue;
    public GameObject[] _ratioBox;
    public Button _selectButton;
    public Animator _animator;
    public Outline _outline;

    private bool _isPlayAnim = false;
    public void Initialize(int recipeID)
    {
        _recipeID = recipeID;
        var recipeInfo = ReadJson._dictPotion[recipeID];
        _recipeName.text = recipeInfo.potionName;

        //위치 초기화
        _outline.enabled = false;

        int materialIdx = 0;
        foreach(int materialRatio in recipeInfo.materialRatioList)
        {
            if(materialRatio != 0 && _ratioValue[materialIdx])
            {
                _ratioValue[materialIdx++].text = materialRatio.ToString();
            }
        }

        materialIdx = 0;
        foreach (int materialID in recipeInfo.ingredientIdList)
        {
            if (materialID != 0)
            {
                var materialInfo = ReadJson._dictMaterial[materialID];
                if(_materiallNames[materialIdx])
                    _materiallNames[materialIdx].text = $"{materialIdx+1}. {materialInfo.materialName}";
                if(_ratioCategoryNames[materialIdx])
                    _ratioCategoryNames[materialIdx++].text = materialInfo.materialName;
            }
        }

        for(int i = materialIdx; i < _ratioBox.Length; ++i)
        {
            if (_materiallNames[i])
                _materiallNames[i].gameObject.SetActive(false);
            if (_ratioBox[i])
                _ratioBox[i].SetActive(false);
        }

    }

    public void ToggleHighLight()
    {
        _outline.enabled = !_outline.enabled;
        GameManager.GM.Board._selectRecipeObject = this;
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Recipe1);

    }

    public void PlayRecipeAnim()
    {
        if (_isPlayAnim) return;

        _animator.SetTrigger("Slide");
        SoundManager._Instance.PlaySFXAtObject(gameObject, SFXType.Recipe2);
        _isPlayAnim = true;
    }

    public void RecipeAnimEnd()
    {
        _isPlayAnim = false;

        GameManager.GM.Board.HideSelectRecipeUI();
    }
}
