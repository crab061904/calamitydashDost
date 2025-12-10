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
        // Listen for scene changes to stop music after gameplay ends
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // If a Results scene or Main Menu loads, stop or fade out the BGM
        string name = scene.name ?? string.Empty;
        if (name.ToLower().Contains("results") || name == "START MENU")
        {
            StopBackgroundMusic(true);
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

    // Stop or fade out background music. If fade = true will fade over 0.8s, otherwise stop immediately.
    public void StopBackgroundMusic(bool fade = false)
    {
        if (bgMusicSource == null) return;
        if (fade)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutMusic(0.8f));
        }
        else
        {
            bgMusicSource.Stop();
        }
    }

    // Immediately stops everything (BGM and SFX)
    public void StopAllAudioImmediate()
    {
        if (bgMusicSource != null) bgMusicSource.Stop();
        if (sfxSource != null) sfxSource.Stop();
    }

    private System.Collections.IEnumerator FadeOutMusic(float duration)
    {
        if (bgMusicSource == null) yield break;
        float startVol = bgMusicSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgMusicSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        bgMusicSource.Stop();
        bgMusicSource.volume = startVol; // reset for next play
    }

}
