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
            // ������ �ڵ����� ����
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
            Debug.LogWarning("[SoundManager] PlayBGM: ���� ������ BGMType�� None�̰ų� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        AudioClip clipToPlay = _bgmClips[(int)_currentBGM];
        if (clipToPlay == null)
        {
            Debug.LogError($"[SoundManager] PlayBGM: BGMType {_currentBGM}�� �ش��ϴ� AudioClip�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        AudioClip ambientClip = _bgmClips[(int)BGMType.Ambient];
        if (ambientClip == null)
        {
            Debug.LogWarning($"[SoundManager] PlayBGM: BGMType {BGMType.Ambient}�� �ش��ϴ� AudioClip�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.LogError($"[SoundManager] PlaySFX2D: SFXType {(int)SFXType.Click}�� �ش��ϴ� AudioClip�� �����ϴ�.");
            return;
        }

        if (_sfxSource == null)
        {
            // ������ �ڵ����� ����
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
            Debug.LogError($"[SoundManager] PlaySFX2D: ��ȿ���� ���� SFXType ({type})�Դϴ�.");
            return;
        }

        if (_sfxClips == null || (int)type >= _sfxClips.Length || _sfxClips[(int)type] == null)
        {
            Debug.LogError($"[SoundManager] PlaySFX2D: SFXType {(int)type}�� �ش��ϴ� AudioClip�� �����ϴ�.");
            return;
        }

        if (_sfxSource == null)
        {
            // ������ �ڵ����� ����
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
            Debug.LogError("[SoundManager] AttachSFXChannel: attachObject null�Դϴ�.");
            return;
        }

        string key = attachObject.name;
        if (_sfxChannelMap.ContainsKey(key) && _sfxChannelMap[key] != null)
        {
            Debug.LogWarning($"[SoundManager] AttachSFXChannel: �̹� '{key}'�� AudioSource�� ��ϵǾ� �ֽ��ϴ�.");
            PlaySFX2D(type);
            return;
        }

        AudioSource newChannel = attachObject.AddComponent<AudioSource>();
        newChannel.playOnAwake = false;
        newChannel.volume = _sfxVolume;

        // Ÿ�Կ� �ش��ϴ� �⺻ Ŭ���� ������ ����
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
        // AudioClip�� ���� ����
        AudioClip clipToPlay = overrideClip;
        if (clipToPlay == null)
        {
            if (type <= SFXType.None )
            {
                Debug.LogError($"[SoundManager] PlaySFXAtLocation: ��ȿ���� ���� SFXType ({type})�Դϴ�.");
                return;
            }
            if (_sfxClips == null || (int)type >= _sfxClips.Length || _sfxClips[(int)type] == null)
            {
                Debug.LogError($"[SoundManager] PlaySFXAtLocation: SFXType {(int)type}�� �ش��ϴ� AudioClip�� �����ϴ�.");
                return;
            }
            clipToPlay = _sfxClips[(int)type];
        }

        if (playObject == null)
        {
            PlaySFX2D(type);
            return;
        }

        // playActor�� �̹� ä��(AudioSource)�� �ִٸ� ���, ������ AttachSFXChannel�� ����
        string key = playObject.name;
        if (!_sfxChannelMap.ContainsKey(key) || _sfxChannelMap[key] == null)
        {
            AttachSFXChannel(playObject, type);
        }

        AudioSource channel = _sfxChannelMap[key];
        if (channel == null)
        {
            Debug.LogError($"[SoundManager] PlaySFXAtLocation: '{key}'�� ����� �� AudioSource�� �����ϴ�.");
            return;
        }

        channel.clip = clipToPlay;
        channel.volume = _sfxVolume;
        channel.loop = false;
        channel.Play();
    }
}