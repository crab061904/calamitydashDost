using UnityEngine;

public class DropOffTrigger : MonoBehaviour
{
    private RescueManager rescueManager;
    private bool boatInRange = false;

    [Header("UI")]
    [SerializeField] private Canvas dropOffCanvas; // assign in Inspector

    void Start()
    {
        rescueManager = FindObjectOfType<RescueManager>();
        if (rescueManager == null)
            Debug.LogError("DropOffTrigger: No RescueManager found in scene!");

        if (dropOffCanvas != null)
            dropOffCanvas.enabled = false; // hide at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInRange = true;
            UpdateCanvasVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInRange = false;
            if (dropOffCanvas != null)
                dropOffCanvas.enabled = false; // hide when leaving
        }
    }

    void Update()
    {
        if (boatInRange && Input.GetKeyDown(KeyCode.E))
        {
            // ✅ Drop off both NPCs and Pets
            rescueManager.DropOffNPCs();
            rescueManager.DropOffPets();
            UpdateCanvasVisibility(); // update after drop-off
        }
    }

    private void UpdateCanvasVisibility()
    {
        if (dropOffCanvas != null)
        {
            // ✅ Only show if boat is in range AND has NPCs or pets inside
            dropOffCanvas.enabled = boatInRange &&
                                   (rescueManager.GetInsideBoatCount() > 0 || rescueManager.GetInsideBoatPetsCount() > 0);
        }
    }
}
