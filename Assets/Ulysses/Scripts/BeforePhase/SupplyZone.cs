using System.Collections;
using UnityEngine;

public class SupplyZone : MonoBehaviour
{
    private int remainingSupplies;
    private bool isDisappearing = false;

    [Header("Disappear Settings")]
    public float disappearDuration = 0.6f; // seconds
    public float shrinkScale = 0.3f;       // how small before vanish

    [Header("Sound Settings")]
    public AudioClip collectSound;   // assign in inspector
    public float soundVolume = 1f;   // volume scale (0–1)

    void Start()
    {
        // Count children with ItemPickupFloat
        remainingSupplies = GetComponentsInChildren<ItemPickupFloat>().Length;
    }

    public void OnSupplyCollected()
    {
        if (isDisappearing) return;

        remainingSupplies--;

        // 🔊 Play sound every time a supply is collected
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        if (remainingSupplies <= 0)
        {
            StartCoroutine(DisappearAnimation());
        }
    }


    private IEnumerator DisappearAnimation()
    {
        isDisappearing = true;

        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * shrinkScale;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] startColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].material.color;
        }

        while (elapsed < disappearDuration)
        {
            float t = elapsed / disappearDuration;

            // Scale down
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Fade out
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    Color c = startColors[i];
                    c.a = Mathf.Lerp(1f, 0f, t);
                    renderers[i].material.color = c;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
