using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FuelFloat : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.2f;   // How high it bobs
    public float frequency = 1f;     // Speed of bobbing

    [Header("Pickup Settings")]
    public float pickupScaleMultiplier = 1.3f; // Grow when picked
    public float pickupDuration = 0.3f;        // Time to scale/fade
    public AudioClip pickupSound;              // Pickup sound
    public float pickupSoundVolume = 1.5f;     // Louder than default (can go >1 if needed)

    private Vector3 startPos;
    private Renderer rend;
    private bool isPicked = false;

    private void Start()
    {
        startPos = transform.position;
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (isPicked) return;

        // Smooth bobbing using sine wave
        float yOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2) * amplitude;

        // Optional: slight horizontal sway
        float xOffset = Mathf.Sin(Time.time * frequency * Mathf.PI) * amplitude * 0.2f;
        float zOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 1.3f) * amplitude * 0.2f;

        transform.position = startPos + new Vector3(xOffset, yOffset, zOffset);
    }

    public void PickUp()
    {
        if (isPicked) return;
        isPicked = true;

        // 🔊 Play pickup sound for every object
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupSoundVolume);
        }

        // Start scale & fade coroutine
        StartCoroutine(PickupAnimation());
    }

    private IEnumerator PickupAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * pickupScaleMultiplier;

        float elapsed = 0f;
        Color originalColor = rend.material.color;

        while (elapsed < pickupDuration)
        {
            float t = elapsed / pickupDuration;
            // Scale up
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            // Fade out
            Color c = originalColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            rend.material.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully invisible
        Color finalColor = originalColor;
        finalColor.a = 0f;
        rend.material.color = finalColor;

        // Destroy object after animation
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            PickUp();

            // Add fuel to player (requires your boat controller reference)
            DuringPhase_BoatController boat = other.GetComponent<DuringPhase_BoatController>();
            if (boat != null)
            {
                boat.AddFuel(25f); // Example: +25 fuel
            }
        }
    }
}
