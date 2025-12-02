using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // singleton

    [Header("Audio Sources (assign)")]
    public AudioSource bgMusicSource; // looped background music source
    public AudioSource sfxSource;     // one-shot SFX source

    [Header("Audio Clips (assign)")]
    public AudioClip bgMusic;
    public AudioClip debrisEatSound;
    public AudioClip levelUpSound;
    public AudioClip levelDownSound;
    public AudioClip debrisFailSound;
    public AudioClip civilianEatSound;


    void Awake()
    {
        // singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // start BGM if assigned
        if (bgMusicSource != null && bgMusic != null)
        {
            bgMusicSource.clip = bgMusic;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }
    }

    // public helper methods to play sounds
    public void PlayDebrisEat()
    {
        if (sfxSource != null && debrisEatSound != null)
            sfxSource.PlayOneShot(debrisEatSound);
    }

    public void PlayLevelUp()
    {
        if (sfxSource != null && levelUpSound != null)
            sfxSource.PlayOneShot(levelUpSound);
    }

    public void PlayLevelDown()
    {
        if (sfxSource != null && levelDownSound != null)
            sfxSource.PlayOneShot(levelDownSound);
    }
    public void PlayDebrisFail()
    {
        if (sfxSource != null && debrisFailSound != null)
            sfxSource.PlayOneShot(debrisFailSound);
    }
    public void PlayCivilianEat()
    {
        if (sfxSource != null && civilianEatSound != null)
            sfxSource.PlayOneShot(civilianEatSound);
    }

}
