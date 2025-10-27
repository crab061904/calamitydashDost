using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class During_BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Assign the music clip to play on start")]
    public AudioClip backgroundMusic;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
