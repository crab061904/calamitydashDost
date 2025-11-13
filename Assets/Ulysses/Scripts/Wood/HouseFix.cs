using UnityEngine;

public class HouseFix : MonoBehaviour
{
    [Header("Wood Plank Placeholders")]
    public GameObject[] woodPlanks;  // Assign in order in Inspector

    [Header("UI")]
    public GameObject placeIndicator; // "Press E to Place" text, world-space or canvas object

    private int currentPlankIndex = 0;
    private PlayerInteraction playerInteraction;
    private bool playerInside = false;

    [Header("Minimap")]
    public GameObject minimapNeedFix;     // parent for wood counter
    public GameObject minimapFinishFix;   // parent for check icon

    [Header("Wood Counter")]
    public GameObject woodCounter;        // child object inside minimapNeedFix
    public UnityEngine.UI.Image woodImage; // optional, can remain static
    public TMPro.TextMeshProUGUI counterText;

    [Header("Check Icon")]
    public GameObject checkIcon;          // child object inside minimapFinishFix

    [Header("Evacuation Manager")]
    public EvacuationManager evacuationManager;  // assign in Inspector

    private void Start()
    {
        // Hide all planks at start
        foreach (var plank in woodPlanks)
            if (plank) plank.SetActive(false);

        // Hide UI
        if (placeIndicator)
            placeIndicator.SetActive(false);

        // Minimap setup
        if (minimapNeedFix) minimapNeedFix.SetActive(true);
        if (minimapFinishFix) minimapFinishFix.SetActive(false);

        // Initialize counter
        if (counterText != null)
            counterText.text = $"{currentPlankIndex}/{woodPlanks.Length}";

        if (checkIcon)
            checkIcon.SetActive(false);

        if (evacuationManager == null)
            evacuationManager = FindObjectOfType<EvacuationManager>();
    }

    private void Update()
    {
        // Fixed: Check if player is inside and has wood directly
        if (playerInside && playerInteraction != null)
        {
            bool hasWood = playerInteraction.CurrentWood != null;
            bool canPlace = hasWood && !IsFullyRepaired();

            if (placeIndicator)
                placeIndicator.SetActive(canPlace);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // Grab PlayerInteraction from GameManager (or anywhere in scene)
            if (playerInteraction == null)
                playerInteraction = FindObjectOfType<PlayerInteraction>();

            Debug.Log("Player entered house zone");

            // Update UI immediately
            if (playerInteraction != null && placeIndicator != null)
            {
                bool hasWood = playerInteraction.CurrentWood != null;
                bool canPlace = hasWood && !IsFullyRepaired();
                placeIndicator.SetActive(canPlace);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (placeIndicator)
                placeIndicator.SetActive(false);

            Debug.Log("Player left house zone");
        }
    }

    public bool PlacePlank(GameObject playerWood)
    {
        if (currentPlankIndex >= woodPlanks.Length || playerWood == null)
            return false;

        woodPlanks[currentPlankIndex].SetActive(true);
        currentPlankIndex++;

        Destroy(playerWood);

        UpdateMinimapUI();

        if (placeIndicator) placeIndicator.SetActive(false);

        return true;
    }

    private void UpdateMinimapUI()
    {
        if (IsFullyRepaired())
        {
            if (minimapNeedFix) minimapNeedFix.SetActive(false);
            if (minimapFinishFix) minimapFinishFix.SetActive(true);
            if (woodCounter) woodCounter.SetActive(false);
            if (checkIcon) checkIcon.SetActive(true);

            // Notify evacuation manager that this house is fixed
            if (evacuationManager != null)
            {
                evacuationManager.AddFixedHouse();
                // To prevent double-counting, only call once
                evacuationManager = null;
            }
        }
        else
        {
            if (minimapNeedFix) minimapNeedFix.SetActive(true);
            if (minimapFinishFix) minimapFinishFix.SetActive(false);

            if (counterText != null)
                counterText.text = $"{currentPlankIndex}/{woodPlanks.Length}";
        }
    }

    public bool IsFullyRepaired()
    {
        return currentPlankIndex >= woodPlanks.Length;
    }
}