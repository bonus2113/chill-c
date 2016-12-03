using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip ambientLoop;
    [SerializeField]
    private AudioClip themeIntro;
    [SerializeField]
    private AudioClip themeLoop;


    [SerializeField]
    private AudioClip plop;
    [SerializeField]
    private AudioClip rune_activate;
    [SerializeField]
    private AudioClip jump_start;
    [SerializeField]
    private AudioClip jump_land;
    [SerializeField]
    private List<AudioClip> footSteps;

    private AudioSource m_ambientLoop = null;
    private AudioSource m_theme = null;
    private AudioSource m_ThemeIntro = null;
    private bool m_loopingPlaying = false;


    private float footstepTimer = 0f;

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
        PLOP,
        FOOTSTEP,
        RUNE,
        JUMP_START,
        JUMP_LAND
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

        this.m_ambientLoop = this.gameObject.AddComponent<AudioSource>();
        this.m_ambientLoop.clip = ambientLoop;
        this.m_ambientLoop.loop = true;
        this.m_ambientLoop.volume = 0.2f;

        this.m_ThemeIntro = this.gameObject.AddComponent<AudioSource>();
        this.m_ThemeIntro.clip = themeIntro;
        this.m_ThemeIntro.volume = 0.6f;

        this.m_theme = this.gameObject.AddComponent<AudioSource>();
        this.m_theme.clip = themeLoop;
        this.m_theme.volume = 0.6f;
        this.m_theme.loop = true;

        DontDestroyOnLoad(transform.gameObject);
        StartBackgroundMusic();
    }

    void Update()
    {
        footstepTimer -= Time.deltaTime;
        if(Player.Instance.isRunning & footstepTimer < 0f)
        {
            PlaySound(Sounds.FOOTSTEP, 0.28f);
            footstepTimer = 0.25f;
        }
    }

    public void StartBackgroundMusic()
    {
        if (m_loopingPlaying)
            return;

        m_loopingPlaying = true;
        this.m_ambientLoop.Play();
        this.m_ThemeIntro.Play();
        this.m_theme.PlayDelayed(themeIntro.length);
    }



    public void StopBackgroundMusic()
    {
        SoundManager.Instance.m_ambientLoop.Stop();
        SoundManager.Instance.m_theme.Stop();
    }

    private AudioClip GetClipFromEnum(Sounds sound)
    {
        AudioClip outClip = null;

        switch (sound)
        {
            case Sounds.PLOP:
                outClip = plop;
                break;
            case Sounds.FOOTSTEP:
                outClip = footSteps[Random.Range(0, footSteps.Count)];
                break;
            case Sounds.RUNE:
                outClip = rune_activate;
                break;
            case Sounds.JUMP_START:
                outClip = jump_start;
                break;
            case Sounds.JUMP_LAND:
                outClip = jump_land;
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
