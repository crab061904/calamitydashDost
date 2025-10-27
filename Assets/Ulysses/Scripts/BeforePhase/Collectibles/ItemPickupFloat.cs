using System.Collections;
using DG.Tweening.Core.Easing;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ItemPickupFloat : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.2f;   // How high it bobs
    public float frequency = 1f;     // Speed of bobbing

    [Header("Pickup Settings")]
    public float pickupScaleMultiplier = 1.3f;
    public float pickupDuration = 0.3f;
    public AudioClip pickupSound;

    private Vector3 startPos;
    private Renderer rend;
    private bool isPicked = false;

    private SupplyZone supplyZone;

    private void Start()
    {
        startPos = transform.position;
        rend = GetComponent<Renderer>();
        supplyZone = GetComponentInParent<SupplyZone>();
    }

    private void Update()
    {
        if (isPicked) return;

        // Smooth bobbing motion
        float yOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2) * amplitude;
        float xOffset = Mathf.Sin(Time.time * frequency * Mathf.PI) * amplitude * 0.2f;
        float zOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 1.3f) * amplitude * 0.2f;

        transform.position = startPos + new Vector3(xOffset, yOffset, zOffset);
    }

    public void PickUp()
    {
        if (isPicked) return;
        isPicked = true;

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

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
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            Color c = originalColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            rend.material.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ✅ Tell SupplyZone this item is gone
        if (supplyZone != null)
        {
            supplyZone.OnSupplyCollected();
        }

        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isPicked) return; // 🚩 Prevents double triggering

        if (other.CompareTag("Player"))
        {
            PickUp();

            EvacuationManager evac = FindObjectOfType<EvacuationManager>();
            if (evac != null)
            {
                if (CompareTag("Food"))
                    evac.AddFood(1);
                else if (CompareTag("Water"))
                    evac.AddWater(1);
                else if (CompareTag("Medkit"))
                    evac.AddMedkit(1);
            }
        }
    }


}
