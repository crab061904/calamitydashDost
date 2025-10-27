using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RescueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI rescuedCountText;
    public TextMeshProUGUI insideBoatText;
    public TextMeshProUGUI timerText;

    [Header("Pet UI")]
    public TextMeshProUGUI rescuedPetsText;
    public TextMeshProUGUI insideBoatPetsText;

    [Header("Game Settings")]
    public float totalTime = 300f;
    public int maxNPCs = 6;
    public int maxPets = 2;

    [Header("Drop-off Settings")]
    public Transform boatExitPoint;
    public Transform safeZoneTarget;

    [SerializeField] private TextMeshProUGUI boatFullMessage;

    [Header("End Screen UI")]
    public GameObject endResultUI;

    private List<GameObject> npcsInsideBoat = new List<GameObject>();
    private List<GameObject> petsInsideBoat = new List<GameObject>();
    private int rescuedCount = 0;
    private int rescuedPetsCount = 0;
    private float currentTime;
    private bool gameFinished = false;

    void Start()
    {
        currentTime = totalTime;
        UpdateUI();
        UpdateTimerUI();

        if (boatFullMessage != null)
            boatFullMessage.gameObject.SetActive(false);

        if (endResultUI != null)
            endResultUI.SetActive(false);
    }

    void Update()
    {
        if (gameFinished) return;

        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f) currentTime = 0f;
            UpdateTimerUI();
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        gameFinished = true;
        Debug.Log("⏳ Time's up! Game Over.");

        if (endResultUI != null)
            endResultUI.SetActive(true);

        // Call the result panel with counts
        DURING_ResultsPanel.Instance.ShowResults(rescuedCount, rescuedPetsCount);

        Time.timeScale = 0f;
    }


    #region NPC Management

    public void AddToBoat(GameObject npc)
    {
        if (npcsInsideBoat.Count < maxNPCs)
        {
            npcsInsideBoat.Add(npc);
            UpdateUI();
        }
        else
        {
            ShowBoatFullUI("NPC seats are full!");
        }
    }

    public int GetInsideBoatCount() => npcsInsideBoat.Count;

    public void DropOffNPCs()
    {
        if (npcsInsideBoat.Count == 0) return;

        int count = npcsInsideBoat.Count;

        foreach (GameObject npc in npcsInsideBoat)
        {
            npc.SetActive(true);
            npc.transform.position = boatExitPoint.position;
            npc.transform.SetParent(null);

            During_NPC_ExitBoat exitScript = npc.GetComponent<During_NPC_ExitBoat>();
            if (exitScript != null)
                exitScript.GoToSafeZone(safeZoneTarget);
        }

        rescuedCount += count;
        npcsInsideBoat.Clear();

        RescueBoatSeatsManager boatSeats = FindObjectOfType<RescueBoatSeatsManager>();
        if (boatSeats != null) boatSeats.FreeNPCSeats();

        UpdateUI();
        Debug.Log($"{count} NPCs dropped off and sent to safe zone!");
    }

    #endregion

    #region Pet Management

    public void AddPetToBoat(GameObject pet)
    {
        if (petsInsideBoat.Count < maxPets)
        {
            petsInsideBoat.Add(pet);
            UpdateUI();
        }
        else
        {
            ShowBoatFullUI("Pet seats are full!");
        }
    }

    public int GetInsideBoatPetsCount() => petsInsideBoat.Count;

    public void DropOffPets()
    {
        if (petsInsideBoat.Count == 0) return;

        int count = petsInsideBoat.Count;

        foreach (GameObject pet in petsInsideBoat)
        {
            pet.SetActive(true);
            pet.transform.position = boatExitPoint.position;
            pet.transform.SetParent(null);

            Pet_ExitBoat exitScript = pet.GetComponent<Pet_ExitBoat>();
            if (exitScript != null)
                exitScript.GoToSafeZone(safeZoneTarget);
        }

        rescuedPetsCount += count;
        petsInsideBoat.Clear();

        RescueBoatSeatsManager boatSeats = FindObjectOfType<RescueBoatSeatsManager>();
        if (boatSeats != null) boatSeats.FreePetSeats();

        UpdateUI();
        Debug.Log($"{count} Pets dropped off and sent to safe zone!");
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        if (rescuedCountText != null)
            rescuedCountText.text = $"Rescued: {rescuedCount}";
        if (insideBoatText != null)
            insideBoatText.text = $"Inside Boat: {npcsInsideBoat.Count}";
        if (rescuedPetsText != null)
            rescuedPetsText.text = $"Rescued Pets: {rescuedPetsCount}";
        if (insideBoatPetsText != null)
            insideBoatPetsText.text = $"Pets in Boat: {petsInsideBoat.Count}";
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void ShowBoatFullUI(string message)
    {
        if (boatFullMessage != null)
            StartCoroutine(ShowBoatFullMessageRoutine(message));
    }

    private IEnumerator ShowBoatFullMessageRoutine(string message)
    {
        boatFullMessage.text = message;
        boatFullMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        boatFullMessage.gameObject.SetActive(false);
    }

    #endregion
}   // ✅ FINAL CLOSING BRACE
