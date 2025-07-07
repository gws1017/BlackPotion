using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Image _currentImage;
    [SerializeField] Sprite[] _imageList;
    [SerializeField] Button _leftButton;
    [SerializeField] Button _rightButton;
    [SerializeField] Button _closeButton;

    private int _imageIndex = 0;
    void Start()
    {
        _leftButton.onClick.AddListener(PrevImage);
        _rightButton.onClick.AddListener(NextImage);
        _closeButton.onClick.AddListener(HideUI);
    }
    public void HideUI()
    {
        SoundManager._Instance.PlayClickSound();
        gameObject.SetActive(false);
    }
    public void PrevImage()
    {
        SoundManager._Instance.PlayClickSound();
        _imageIndex--;
        if (_imageIndex < 0) _imageIndex = _imageList.Length - 1;

        _imageIndex = Mathf.Clamp(_imageIndex,0, _imageList.Length-1);
        _currentImage.sprite = _imageList[_imageIndex];
    }
    public void NextImage()
    {
        SoundManager._Instance.PlayClickSound();
        _imageIndex++;
        if (_imageIndex >= _imageList.Length) _imageIndex = 0;

        _imageIndex = Mathf.Clamp(_imageIndex, 0, _imageList.Length-1);
        _currentImage.sprite = _imageList[_imageIndex];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
