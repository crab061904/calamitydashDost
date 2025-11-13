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

    [Header("Carrying Wood")]
    public Transform headCarrySocket; // Empty GameObject on player's head
    private GameObject currentWood;   // Currently carried wood
    private int woodCount = 0;
    public int maxWoodStack = 1;

    [Header("UI")]
    public GameObject pickUpIndicator;  // UI for "Press E to pick up"


    private bool isDriving = false;
    private EvacuationManager evacManager;

    private void Start()
    {
        busArrow.SetActive(false);
        if (busIcon != null) busIcon.SetActive(true);

        evacManager = FindObjectOfType<EvacuationManager>();
        if (evacManager == null)
            Debug.LogWarning("⚠️ EvacuationManager not found in scene!");

        if (pickUpIndicator != null) pickUpIndicator.SetActive(false);
    }

    void Update()
    {
        UpdateUIIndicators();

        // --- BUS INTERACTION ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isDriving)
            {
                // Prevent entering bus if carrying wood
                if (currentWood != null)
                {
                    Debug.Log("🚫 Cannot enter bus while carrying wood!");
                    return;
                }

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

        // --- E KEY HANDLING ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentWood == null)
            {
                TryPickUpWood(); // Pick up from wood stack
            }
            else
            {
                TryPlaceWood(); // Place at house
            }
        }
    }

    // ----------------- WOOD PICKUP -----------------
    private void TryPickUpWood()
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("WoodStack"))
            {
                WoodStack stack = hit.GetComponent<WoodStack>();
                if (stack != null && stack.HasPlanks())
                {
                    GameObject plank = stack.PickUpPlank();
                    if (plank != null)
                    {
                        // Attach to player's head
                        plank.transform.SetParent(headCarrySocket);
                        plank.transform.localPosition = Vector3.zero;
                        plank.transform.localRotation = Quaternion.identity;

                        // Disable collider for safety
                        Collider col = plank.GetComponent<Collider>();
                        if (col) col.enabled = false;

                        currentWood = plank;
                        woodCount++;

                        Debug.Log("🪵 Picked up wood!");
                    }
                }
                break;
            }
        }
    }

    // ----------------- PLACE WOOD -----------------
    private void TryPlaceWood()
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("HouseFix"))
            {
                HouseFix house = hit.GetComponent<HouseFix>();
                if (house != null && !house.IsFullyRepaired())
                {
                    bool placed = house.PlacePlank(currentWood);
                    if (placed)
                    {
                        currentWood = null;
                        woodCount--;
                        Debug.Log("✅ Placed wood on house!");
                    }
                    break;
                }
            }
        }
    }

    // ----------------- INDICATORS -----------------
    private void UpdateUIIndicators()
    {
        bool nearWood = false;
        bool nearHouse = false;

        Collider[] hits = Physics.OverlapSphere(player.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (currentWood == null && hit.CompareTag("WoodStack"))
            {
                WoodStack ws = hit.GetComponent<WoodStack>();
                if (ws != null && ws.HasPlanks())
                    nearWood = true;
            }

            if (currentWood != null && hit.CompareTag("HouseFix"))
            {
                HouseFix hf = hit.GetComponent<HouseFix>();
                if (hf != null && !hf.IsFullyRepaired())
                    nearHouse = true;
            }
        }

        if (pickUpIndicator != null)
            pickUpIndicator.SetActive(nearWood && currentWood == null);
    }

    // ----------------- BUS SYSTEM -----------------
    void EnterBus()
    {
        isDriving = true;

        // Handle rescued pets
        PetController[] allPets = FindObjectsOfType<PetController>();
        foreach (var pet in allPets)
        {
            if (pet != null && pet.IsFollowingPlayer())
            {
                if (evacManager != null)
                    evacManager.AddEvacuatedPet();

                Destroy(pet.gameObject);
            }
        }

        player.SetActive(false);
        bus.GetComponent<BusController>().isPlayerDriving = true;

        playerCamera.SetActive(false);
        busCamera.SetActive(true);

        if (minimapCamera != null)
            minimapCamera.SetTarget(bus.transform, true);

        playerArrow?.SetActive(false);
        busArrow?.SetActive(true);
        busIcon?.SetActive(false);
    }

    void ExitBus()
    {
        isDriving = false;
        Vector3 safeExitPos = exitPoint != null ? exitPoint.position : bus.transform.position + bus.transform.right * 2f;

        if (Physics.CheckSphere(safeExitPos, 0.3f, LayerMask.GetMask("Default", "Building", "Obstacle")))
            safeExitPos = FindSafeExitPosition();

        player.transform.position = safeExitPos;
        player.transform.rotation = exitPoint ? exitPoint.rotation : bus.transform.rotation;

        player.SetActive(true);
        bus.GetComponent<BusController>().isPlayerDriving = false;

        playerCamera.SetActive(true);
        busCamera.SetActive(false);

        if (minimapCamera != null)
            minimapCamera.SetTarget(player.transform, false);

        playerArrow?.SetActive(true);
        busArrow?.SetActive(false);
        busIcon?.SetActive(true);
    }

    private Vector3 FindSafeExitPosition()
    {
        Vector3[] directions =
        {
            exitPoint.right * -1f,
            (exitPoint.right * -1f) + exitPoint.forward,
            (exitPoint.right * -1f) - exitPoint.forward,
            exitPoint.right * 1f,
            exitPoint.forward * -1f
        };

        foreach (var dir in directions)
        {
            Vector3 testPos = exitPoint.position + dir * 1f;
            if (!Physics.CheckSphere(testPos, 0.3f, LayerMask.GetMask("Default", "Building", "Obstacle")))
                return testPos;
        }

        return exitPoint.position + Vector3.up * 1f;
    }

    public bool HasWood()
    {
        return currentWood != null;
    }

    // Public property for easier access
    public GameObject CurrentWood => currentWood;
}