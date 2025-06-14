using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum BGMType
{
    None = -1,
    Title = 0,
    InGame = 1,
    Ambient = 2
}

public enum SFXType
{
    None = -1, 
    Click = 0,
    Money,
    Item,
    Recipe1,
    Recipe2,
    Add,
    Craft,
    Coin,
    Stamp,
    Succ,
    Fail,
    Writing
}

public class SoundManager : MonoBehaviour
{

    public static SoundManager _Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _bgmAmbientSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    [Tooltip("0=Title, 1=InGame, 2=Ambient")]
    [SerializeField] private AudioClip[] _bgmClips;

    [Tooltip("0=click , 1=money")]
    [SerializeField] private AudioClip[] _sfxClips;

    private Dictionary<string, AudioSource> _sfxChannelMap = new Dictionary<string, AudioSource>();

    [Header("Default Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float _bgmVolume = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _sfxVolume = 0.3f;

    private BGMType _currentBGM = BGMType.None;

    public BGMType CurrentBGM { get => _currentBGM; set => _currentBGM = value; }

    public float BGMVolume 
    { 
        get => _bgmVolume; 
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            if (_bgmSource != null)
                _bgmSource.volume = value;
        }
    }
    public float SFXVolume 
    { 
        get => _sfxVolume;  
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            if (_sfxSource != null)
                _sfxSource.volume = value;

            var Sources = _sfxChannelMap.Values;
            if (Sources.Count <= 0) return;
            foreach (var Source in Sources)
            {
                Source.volume = value;
            }
        }
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (_bgmAmbientSource == null)
        {
            _bgmAmbientSource = gameObject.AddComponent<AudioSource>();
            _bgmAmbientSource.playOnAwake = false;
            _bgmAmbientSource.loop = true;
            _bgmAmbientSource.spatialBlend = 0f;
            _bgmAmbientSource.volume = 0.1f;
        }
        if (_bgmSource == null)
        {
            // 없으면 자동으로 생성
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0f; 
        }

        _bgmSource.volume = _bgmVolume;
        //_sfxSource.volume = _sfxVolume;
    }

    public void PlayBGM()
    {
        if (_currentBGM == BGMType.None)
        {
            Debug.LogWarning("[SoundManager] PlayBGM: 현재 설정된 BGMType이 None이거나 유효하지 않습니다.");
            return;
        }

        AudioClip clipToPlay = _bgmClips[(int)_currentBGM];
        if (clipToPlay == null)
        {
            Debug.LogError($"[SoundManager] PlayBGM: BGMType {_currentBGM}에 해당하는 AudioClip이 할당되지 않았습니다.");
            return;
        }

        AudioClip ambientClip = _bgmClips[(int)BGMType.Ambient];
        if (ambientClip == null)
        {
            Debug.LogWarning($"[SoundManager] PlayBGM: BGMType {BGMType.Ambient}에 해당하는 AudioClip이 할당되지 않았습니다.");
        }
        else
        {
            if (_bgmAmbientSource.isPlaying)
                _bgmAmbientSource.Stop();

            _bgmAmbientSource.clip = ambientClip;
            _bgmAmbientSource.loop = true;
            _bgmAmbientSource.Play();
        }

        if (_bgmSource.isPlaying)
            _bgmSource.Stop();

        _bgmSource.clip = clipToPlay;
        _bgmSource.volume = _bgmVolume;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void PlayClickSound()
    {
        if (_sfxClips == null || (int)SFXType.Click >= _sfxClips.Length || _sfxClips[(int)SFXType.Click] == null)
        {
            Debug.LogError($"[SoundManager] PlaySFX2D: SFXType {(int)SFXType.Click}에 해당하는 AudioClip이 없습니다.");
            return;
        }

        if (_sfxSource == null)
        {
            // 없으면 자동으로 생성
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            _sfxSource.spatialBlend = 0f;
        }

        _sfxSource.clip = _sfxClips[(int)SFXType.Click];
        _sfxSource.volume = _sfxVolume;
        _sfxSource.loop = false;
        _sfxSource.Play();

    }
    public void PlaySFX2D(SFXType type)
    {
        if (type <= SFXType.None)
        {
            Debug.LogError($"[SoundManager] PlaySFX2D: 유효하지 않은 SFXType ({type})입니다.");
            return;
        }

        if (_sfxClips == null || (int)type >= _sfxClips.Length || _sfxClips[(int)type] == null)
        {
            Debug.LogError($"[SoundManager] PlaySFX2D: SFXType {(int)type}에 해당하는 AudioClip이 없습니다.");
            return;
        }

        if (_sfxSource == null)
        {
            // 없으면 자동으로 생성
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            _sfxSource.spatialBlend = 0f;
        }

        _sfxSource.clip = _sfxClips[(int)type];
        _sfxSource.volume = _sfxVolume;
        _sfxSource.loop = false;
        _sfxSource.Play();
    }

    public void AttachSFXChannel(GameObject attachObject, SFXType type)
    {
        if (attachObject == null)
        {
            Debug.LogError("[SoundManager] AttachSFXChannel: attachObject null입니다.");
            return;
        }

        string key = attachObject.name;
        if (_sfxChannelMap.ContainsKey(key) && _sfxChannelMap[key] != null)
        {
            Debug.LogWarning($"[SoundManager] AttachSFXChannel: 이미 '{key}'에 AudioSource가 등록되어 있습니다.");
            PlaySFX2D(type);
            return;
        }

        AudioSource newChannel = attachObject.AddComponent<AudioSource>();
        newChannel.playOnAwake = false;
        newChannel.volume = _sfxVolume;

        // 타입에 해당하는 기본 클립이 있으면 세팅
        if (type > SFXType.None )
        {
            if (_sfxClips != null && (int)type < _sfxClips.Length && _sfxClips[(int)type] != null)
            {
                newChannel.clip = _sfxClips[(int)type];
            }
        }

        _sfxChannelMap[key] = newChannel;
    }

    public void PlaySFXAtObject(GameObject playObject, SFXType type, AudioClip overrideClip = null)
    {
        // AudioClip을 먼저 결정
        AudioClip clipToPlay = overrideClip;
        if (clipToPlay == null)
        {
            if (type <= SFXType.None )
            {
                Debug.LogError($"[SoundManager] PlaySFXAtLocation: 유효하지 않은 SFXType ({type})입니다.");
                return;
            }
            if (_sfxClips == null || (int)type >= _sfxClips.Length || _sfxClips[(int)type] == null)
            {
                Debug.LogError($"[SoundManager] PlaySFXAtLocation: SFXType {(int)type}에 해당하는 AudioClip이 없습니다.");
                return;
            }
            clipToPlay = _sfxClips[(int)type];
        }

        if (playObject == null)
        {
            PlaySFX2D(type);
            return;
        }

        // playActor에 이미 채널(AudioSource)이 있다면 재생, 없으면 AttachSFXChannel로 생성
        string key = playObject.name;
        if (!_sfxChannelMap.ContainsKey(key) || _sfxChannelMap[key] == null)
        {
            AttachSFXChannel(playObject, type);
        }

        AudioSource channel = _sfxChannelMap[key];
        if (channel == null)
        {
            Debug.LogError($"[SoundManager] PlaySFXAtLocation: '{key}'에 제대로 된 AudioSource가 없습니다.");
            return;
        }

        channel.clip = clipToPlay;
        channel.volume = _sfxVolume;
        channel.loop = false;
        channel.Play();
    }
}