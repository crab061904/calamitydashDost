using UnityEngine;

public class BuildingZone : MonoBehaviour
{
    private GameManager gameManager;
    private bool delivered = false;

    [Header("Popup Settings")]
    public GameObject popupUI; // assign a small UI canvas or world-space text prefab

    [Header("Effects")]
    public ParticleSystem areaStarEffect; // assign Area_star_ellow in Inspector

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Hide popup at start
        if (popupUI != null)
            popupUI.SetActive(false);

        // Play the particle system at start (if assigned)
        if (areaStarEffect != null)
            areaStarEffect.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        Joe_PlayerMovement player = other.GetComponentInParent<Joe_PlayerMovement>();

        if (player != null && player.isCarrying)
        {
            if (!delivered)
            {
                // First time delivery ✅
                player.RemoveCarry();
                gameManager?.DeliverBox();
                delivered = true;

                // Disable particle system after delivery
                if (areaStarEffect != null)
                {
                    areaStarEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }

                Debug.Log($"{gameObject.name} delivery completed!");
            }
            else
            {
                // Already delivered ⚠️ → show popup
                if (popupUI != null)
                {
                    popupUI.SetActive(true);
                    CancelInvoke(nameof(HidePopup));
                    Invoke(nameof(HidePopup), 2f); // auto-hide after 2 seconds
                }
                Debug.Log($"{gameObject.name} has already been delivered to!");
            }
        }
    }

    private void HidePopup()
    {
        if (popupUI != null)
            popupUI.SetActive(false);
    }
}
