using UnityEngine;
using UnityEngine.AI;

public class Dog_Seat : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private RescueManager rescueManager;
    private RescueBoatSeatsManager boatSeatsManager;

    private bool isRescued = false;
    public bool IsRescued => isRescued;
    private bool boatInRange = false;

    [Header("UI")]
    [SerializeField] private Canvas rescueCanvas;

    // 🔹 Auto-detected minimap icon
    private GameObject minimapIcon;

    [Header("Floating Indicator")]
    private GameObject floatingIndicatorInstance; // 🐾 clone per pet
    [SerializeField] private float indicatorOffsetY = 1.7f; // how high above the pet

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("isSeated", false);

        rescueManager = FindObjectOfType<RescueManager>();
        if (rescueManager == null)
            Debug.LogError("Dog_Seat: No RescueManager found in scene!");

        if (rescueCanvas != null)
            rescueCanvas.enabled = false;

        // 🔍 Find child with layer "MinimapONLY"
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("MinimapONLY"))
            {
                minimapIcon = child.gameObject;
                break;
            }
        }

        // 🔍 Find the indicator prefab in the scene by tag
        GameObject indicatorPrefab = GameObject.FindGameObjectWithTag("During_Pet_Indicator");
        if (indicatorPrefab != null)
        {
            // Instantiate a clone and parent it to this pet
            floatingIndicatorInstance = Instantiate(indicatorPrefab, transform);

            // Position slightly above pet
            floatingIndicatorInstance.transform.localPosition = new Vector3(0, indicatorOffsetY, 0);

            // Ensure active (in case prefab is disabled in scene)
            floatingIndicatorInstance.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"⚠️ No GameObject with tag 'During_Pet_Indicator' found for {name}");
        }
    }

    void Update()
    {
        if (isRescued) return;

        if (boatInRange && Input.GetKeyDown(KeyCode.E))
        {
            TrySitInBoat();
        }
    }

    void TrySitInBoat()
    {
        if (boatSeatsManager == null || rescueManager == null) return;

        Transform freeSeat = boatSeatsManager.GetFreePetSeat();
        if (freeSeat != null)
        {
            if (agent != null) agent.enabled = false;

            transform.position = freeSeat.position;
            transform.rotation = freeSeat.rotation;
            transform.SetParent(freeSeat);

            if (animator != null)
                animator.SetBool("isSeated", true);

            isRescued = true;

            if (rescueCanvas != null)
                rescueCanvas.enabled = false;

            if (minimapIcon != null)
                minimapIcon.SetActive(false);

            if (floatingIndicatorInstance != null)
                floatingIndicatorInstance.SetActive(false);

            rescueManager.AddPetToBoat(gameObject);
        }
        else
        {
            rescueManager.ShowBoatFullUI("Pet seats are full!");
            Debug.Log("🚫 No free pet seats available!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInRange = true;
            boatSeatsManager = other.GetComponent<RescueBoatSeatsManager>();

            if (!isRescued)
            {
                if (rescueCanvas != null)
                    rescueCanvas.enabled = true;

                if (floatingIndicatorInstance != null)
                    floatingIndicatorInstance.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInRange = false;
            boatSeatsManager = null;

            if (!isRescued)
            {
                if (rescueCanvas != null)
                    rescueCanvas.enabled = false;

                if (floatingIndicatorInstance != null)
                    floatingIndicatorInstance.SetActive(true);
            }
        }
    }
}
