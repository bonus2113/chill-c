using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    [SerializeField]
    private AudioClip Background;
    [SerializeField]
    private List<AudioClip> SplashSounds;
    [SerializeField]
    private AudioClip Bang;
    [SerializeField]
    private AudioClip Ready;
    [SerializeField]
    private AudioClip PlayerConnected;
    [SerializeField]
    private AudioClip BoardBoing;
    [SerializeField]
    private List<AudioClip> HitSounds;
    [SerializeField]
    private List<AudioClip> WoodSounds;

    private AudioSource m_BackGroundMusicSource = null;
    private bool b_BackgroundPlaying = false;

    public static SoundManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SoundManager();
            }
            return m_Instance;
        }
    }
    private static SoundManager m_Instance = null;

    public enum Sounds
    {
        SPLOOSH,
        BANG,
        READY,
        PLAYERCONNECTED,
        BOARDBOING,
        HIT,
        WOOD
    };

    void Awake()
    {
        if (m_Instance)
        {
            Debug.LogWarning("Sound manager already exists.");
            Destroy(gameObject);
            return;
        }
        
        m_Instance = this;

        this.m_BackGroundMusicSource = this.gameObject.AddComponent<AudioSource>();
        this.m_BackGroundMusicSource.clip = Background;
        this.m_BackGroundMusicSource.loop = true;

        DontDestroyOnLoad(transform.gameObject);
    }

    public void StartBackgroundMusic()
    {
        if (b_BackgroundPlaying)
            return;

        b_BackgroundPlaying = true;
        SoundManager.Instance.m_BackGroundMusicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        SoundManager.Instance.m_BackGroundMusicSource.Stop();
    }

    private AudioClip GetClipFromEnum(Sounds sound)
    {
        AudioClip outClip = null;

        switch (sound)
        {
            case Sounds.BANG:
                outClip = Bang;
                break;
            case Sounds.SPLOOSH:
                outClip = SplashSounds[Random.Range(0, SplashSounds.Count)]; ;
                Debug.Log("Returned Splash: " + outClip.name);
                break;
            case Sounds.PLAYERCONNECTED:
                outClip = PlayerConnected;
                break;
            case Sounds.READY:
                outClip = Ready;
                break;
            case Sounds.BOARDBOING:
                outClip = BoardBoing;
                break;
            case Sounds.HIT:
                outClip = HitSounds[Random.Range(0, HitSounds.Count)];
                break;
            case Sounds.WOOD:
                outClip = WoodSounds[Random.Range(0, WoodSounds.Count)];
                break;
            default:
                break;
        };

        return outClip;
    }

    public void PlaySound(Sounds sound, float volume = 1.0f)
    {
        AudioSource.PlayClipAtPoint(GetClipFromEnum(sound), UnityEngine.Camera.main.transform.position, volume);
    }
}
