using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgMusicSource;
    public AudioSource sfxSource;

    [Header("Sound Clips")]
    public AudioClip pickupClip;
    public AudioClip deliverClip;

    private void Start()
    {
        if (bgMusicSource != null && !bgMusicSource.isPlaying)
        {
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }
    }

    public void PlayPickupSound()
    {
        if (pickupClip != null && sfxSource != null)
            sfxSource.PlayOneShot(pickupClip);
    }

    public void PlayDeliverSound()
    {
        if (deliverClip != null && sfxSource != null)
            sfxSource.PlayOneShot(deliverClip);
    }
}
