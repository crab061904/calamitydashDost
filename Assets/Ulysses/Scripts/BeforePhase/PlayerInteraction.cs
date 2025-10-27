using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject bus;
    public GameObject playerCamera;
    public GameObject busCamera;
    public Transform exitPoint;
    public LimitCamera minimapCamera;

    [Header("Markers")]
    public GameObject playerArrow;
    public GameObject busArrow;
    public GameObject busIcon;

    private bool isDriving = false;

    private EvacuationManager evacManager;

    private void Start()
    {
        busArrow.SetActive(false);
        if (busIcon != null) busIcon.SetActive(true);

        evacManager = FindObjectOfType<EvacuationManager>();
        if (evacManager == null)
            Debug.LogWarning("⚠️ EvacuationManager not found in scene!");
    }

    void Update()
    {
        // --- BUS INTERACTION ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isDriving)
            {
                float distance = Vector3.Distance(player.transform.position, bus.transform.position);
                if (distance <= 7f)
                {
                    EnterBus();
                }
            }
            else
            {
                ExitBus();
            }
        }
    }

    // ----------------- BUS -----------------
    void EnterBus()
    {
        isDriving = true;

        // --- Handle rescued pets ---
        PetController[] allPets = FindObjectsOfType<PetController>();
        foreach (var pet in allPets)
        {
            if (pet != null && pet.IsFollowingPlayer()) // ✅ only rescued/following pets
            {
                // Increment evacuation manager
                EvacuationManager manager = FindObjectOfType<EvacuationManager>();
                if (manager != null)
                {
                    manager.AddEvacuatedPet();
                }

                Destroy(pet.gameObject); // ✅ remove pet from scene
            }
        }

        // --- Usual bus logic ---
        player.SetActive(false);
        bus.GetComponent<BusController>().isPlayerDriving = true;

        playerCamera.SetActive(false);
        busCamera.SetActive(true);

        if (minimapCamera != null)
            minimapCamera.SetTarget(bus.transform, true);

        if (playerArrow != null) playerArrow.SetActive(false);
        if (busArrow != null) busArrow.SetActive(true);
        if (busIcon != null) busIcon.SetActive(false);
    }

    void ExitBus()
    {
        isDriving = false;

        Vector3 safeExitPos = exitPoint != null ? exitPoint.position : bus.transform.position + bus.transform.right * 2f;

        // ✅ SAFETY CHECK: if exit point is inside obstruction, find alternative
        if (Physics.CheckSphere(safeExitPos, 0.3f, LayerMask.GetMask("Default", "Building", "Obstacle")))
        {
            safeExitPos = FindSafeExitPosition();
        }

        player.transform.position = safeExitPos;
        player.transform.rotation = exitPoint ? exitPoint.rotation : bus.transform.rotation;

        player.SetActive(true);
        bus.GetComponent<BusController>().isPlayerDriving = false;

        playerCamera.SetActive(true);
        busCamera.SetActive(false);

        if (minimapCamera != null)
            minimapCamera.SetTarget(player.transform, false);

        if (playerArrow != null) playerArrow.SetActive(true);
        if (busArrow != null) busArrow.SetActive(false);
        if (busIcon != null) busIcon.SetActive(true);
    }


    // ✅ NEW HELPER FUNCTION
    private Vector3 FindSafeExitPosition()
    {
        Vector3[] directions =
        {
        exitPoint.right * -1f,       // Left (main exit)
        (exitPoint.right * -1f) + exitPoint.forward,  // Front-left
        (exitPoint.right * -1f) - exitPoint.forward,  // Back-left
        exitPoint.right * 1f,        // Right fallback
        exitPoint.forward * -1f      // Behind bus
    };

        foreach (var dir in directions)
        {
            Vector3 testPos = exitPoint.position + dir * 1f;

            if (!Physics.CheckSphere(testPos, 0.3f, LayerMask.GetMask("Default", "Building", "Obstacle")))
            {
                return testPos;
            }
        }

        return exitPoint.position + Vector3.up * 1f; // Worst-case fallback (spawn slightly up)
    }

    public void DisableControls()
    {
        enabled = false;  // This will stop Update() from running, disabling input
    }

    public void EnableControls()
    {
        enabled = true;
    }


    private void DestroyFollowingPets()
    {
        if (evacManager == null) return;

        // Find all pets in scene
        PetController[] allPets = FindObjectsOfType<PetController>();
        foreach (var pet in allPets)
        {
            // Check if the pet is following the player
            if (pet.isActiveAndEnabled && pet.transform != null && pet.transform.parent == null)
            {
                // Check if the pet's followTarget is the player
                var followField = pet.GetType().GetField("followTarget", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Transform followTarget = (Transform)followField?.GetValue(pet);

                if (followTarget == player.transform)
                {
                    evacManager.AddEvacuatedPet(); // Increment counter
                    Destroy(pet.gameObject);       // Remove from scene
                }
            }
        }
    }
}
