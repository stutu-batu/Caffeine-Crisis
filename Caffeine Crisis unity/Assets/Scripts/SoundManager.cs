using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip bgmClip;

    [Header("Audio Clips (SFX)")]
    public AudioClip buttonClickClip;
    public AudioClip iceMachineClip;
    public AudioClip coffeeMachineClip;
    public AudioClip creamClip;
    public AudioClip pumpClip;
    public AudioClip hotWaterClip;
    public AudioClip pourLiquidClip;
    public AudioClip coldWaterClip;
    public AudioClip bellRingClip;
    public AudioClip moneyPayClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // Herhangi bir sesi anında çalar
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}