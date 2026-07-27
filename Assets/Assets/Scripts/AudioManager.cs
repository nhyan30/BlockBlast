using UnityEngine;

public enum SoundType
{
    Place,
    Clear,
    GameOver,
    Flip, // Used for settings slider test
    ButtonClick
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip placeClip;
    public AudioClip clearClip;
    public AudioClip gameOverClip;
    public AudioClip buttonClickClip;
    public AudioClip flipClip;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep audio playing between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void Play(SoundType type)
    {
        if (sfxSource == null) return;

        AudioClip clip = null;
        switch (type)
        {
            case SoundType.Place: clip = placeClip; break;
            case SoundType.Clear: clip = clearClip; break;
            case SoundType.GameOver: clip = gameOverClip; break;
            case SoundType.ButtonClick: clip = buttonClickClip; break;
            case SoundType.Flip: clip = flipClip; break;
        }

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null) musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}