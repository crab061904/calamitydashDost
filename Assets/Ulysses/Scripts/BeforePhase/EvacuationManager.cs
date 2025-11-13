using UnityEngine;
using TMPro;
using System.Collections;

public class EvacuationManager : MonoBehaviour
{
    [Header("Evacuation UI")]
    public TextMeshProUGUI npcEvacText;
    public TextMeshProUGUI petEvacText;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Supplies UI")]
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI medkitText;

    [Header("House Fixing")]
    public TextMeshProUGUI houseText;

    [Header("Objectives Needed")]
    public int foodNeeded = 3;
    public int waterNeeded = 3;
    public int medkitsNeeded = 2;
    public int npcsNeeded = 6;
    public int petsNeeded = 4;
    public int housesToFix = 2;

    private int npcEvacCount = 0;
    private int petEvacCount = 0;
    private int foodCollected = 0;
    private int waterCollected = 0;
    private int medkitCollected = 0;
    private int housesFixed = 0;


    private float timer = 0f;
    private bool gameFinished = false;

    [Header("Evacuation Path")]
    public GameObject evacArrow; // Whole canvas (disabled at start)

    [Header("Evacuation Target")]
    public Transform evacExitPoint; // Assign your ExitTrigger object here


    // ✅ Score System
    public int CurrentScore { get; private set; } = 0; // Live score
    public int PendingScore { get; private set; }      // Final score
    public int PendingStars { get; private set; }
    public bool ReadyForEvacExit { get; private set; } = false;

    public float GetTimer() => timer;

    [Header("Completion Check Icons")]
    public GameObject npcCheckIcon;
    public GameObject petCheckIcon;
    public GameObject foodCheckIcon;
    public GameObject waterCheckIcon;
    public GameObject medkitCheckIcon;
    public GameObject houseCheckIcon;


    [Header("Warnings UI")]
    public GameObject wrongStreetOverlay;


    void Start()
    {
        wrongStreetOverlay?.SetActive(false);

        evacArrow?.SetActive(false);
        npcCheckIcon?.SetActive(false);
        petCheckIcon?.SetActive(false);
        foodCheckIcon?.SetActive(false);
        waterCheckIcon?.SetActive(false);
        medkitCheckIcon?.SetActive(false);
        houseCheckIcon?.SetActive(false);
    }

    void Update()
    {
        if (!gameFinished)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void AddEvacuatedNPC() { npcEvacCount++; CurrentScore += 10; UpdateUI(); CheckCompletion(); }
    public void AddEvacuatedPet() { petEvacCount++; CurrentScore += 15; UpdateUI(); CheckCompletion(); }
    public void AddFood(int amount) { foodCollected += amount; CurrentScore += 5 * amount; UpdateUI(); CheckCompletion(); }
    public void AddWater(int amount) { waterCollected += amount; CurrentScore += 5 * amount; UpdateUI(); CheckCompletion(); }
    public void AddMedkit(int amount) { medkitCollected += amount; CurrentScore += 8 * amount; UpdateUI(); CheckCompletion(); }

    public void AddFixedHouse()
    {
        housesFixed++;
        CurrentScore += 12;   // give points for fixing

        if (houseCheckIcon != null && housesFixed >= housesToFix)
            houseCheckIcon.SetActive(true);

        UpdateUI();
        CheckCompletion();
    }


    private void UpdateUI()
    {
        npcEvacText.text = $"{npcEvacCount}/{npcsNeeded}";
        petEvacText.text = $"{petEvacCount}/{petsNeeded}";
        foodText.text = $"{foodCollected}/{foodNeeded}";
        waterText.text = $"{waterCollected}/{waterNeeded}";
        medkitText.text = $"{medkitCollected}/{medkitsNeeded}";
        houseText.text = $"{housesFixed}/{housesToFix}";

        // House fix UI
        if (houseCheckIcon != null)
            houseCheckIcon.SetActive(housesFixed >= housesToFix);

        // Existing check icons
        if (npcEvacCount >= npcsNeeded) npcCheckIcon?.SetActive(true);
        if (petEvacCount >= petsNeeded) petCheckIcon?.SetActive(true);
        if (foodCollected >= foodNeeded) foodCheckIcon?.SetActive(true);
        if (waterCollected >= waterNeeded) waterCheckIcon?.SetActive(true);
        if (medkitCollected >= medkitsNeeded) medkitCheckIcon?.SetActive(true);
    }


    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void CheckCompletion()
    {
        if (npcEvacCount >= npcsNeeded &&
            petEvacCount >= petsNeeded &&
            foodCollected >= foodNeeded &&
            waterCollected >= waterNeeded &&
            medkitCollected >= medkitsNeeded &&
            housesFixed >= housesToFix)   // include houses
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        gameFinished = true;

        PendingScore = CurrentScore;

        // Adjust star thresholds: 5 min = 300s
        PendingStars = (timer <= 300f) ? 3 : (timer <= 360f) ? 2 : 1;

        ReadyForEvacExit = true;
        evacArrow?.SetActive(true);
    }


    // ✅ Optional score deduction from outside scripts
    public void DeductScore(int amount)
    {
        CurrentScore = Mathf.Max(0, CurrentScore - amount);
    }


    public void ShowWrongStreetOverlay(float duration = 3f)
    {
        if (wrongStreetOverlay == null) return;

        wrongStreetOverlay.SetActive(true);
        StartCoroutine(HideWrongStreetOverlay(duration));
    }

    private IEnumerator HideWrongStreetOverlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (wrongStreetOverlay != null)
            wrongStreetOverlay.SetActive(false);
    }

}