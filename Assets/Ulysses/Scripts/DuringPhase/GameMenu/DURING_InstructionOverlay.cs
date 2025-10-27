using UnityEngine;
using TMPro;
using System.Collections;

public class DURING_InstructionOverlay : MonoBehaviour
{
    public GameObject instructionUI;      // Instruction overlay canvas/panel
    public GameObject countdownUI;        // Countdown canvas/panel (whole panel, not just text)
    public TextMeshProUGUI countdownText; // Countdown text element inside countdown UI

    public float countdownTime = 3f; // Seconds for countdown

    public bool showOnStart = true;  // Toggle this in Inspector if needed

    private System.Action onComplete;

    void Start()
    {
        if (showOnStart)
            ShowInstructions(null); // No callback if not needed

        countdownUI.SetActive(false); // Hide countdown at start
    }

    // Call this to show instructions manually
    public void ShowInstructions(System.Action afterInstructions)
    {
        instructionUI.SetActive(true);
        onComplete = afterInstructions;
        Time.timeScale = 0f; // Freeze gameplay
    }

    // Called by OK button
    public void OnReadyClicked()
    {
        instructionUI.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        countdownUI.SetActive(true);

        float remaining = countdownTime;
        while (remaining > 0)
        {
            countdownText.text = Mathf.Ceil(remaining).ToString();
            yield return new WaitForSecondsRealtime(1f); // Use REAL TIME even when paused
            remaining--;
        }

        countdownUI.SetActive(false);

        Time.timeScale = 1f; // Unpause game AFTER countdown
        onComplete?.Invoke(); // Start the actual phase
    }

}
