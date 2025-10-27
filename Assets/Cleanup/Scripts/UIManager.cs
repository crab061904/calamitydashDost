using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References (assign in Inspector)")]
    public TextMeshProUGUI scoreText;    // Score display
    public TextMeshProUGUI levelText;    // Level display
    public Slider progressBar;           // Progress bar display
    public TextMeshProUGUI maxLevelText; // ✅ Shown when max level is reached
    public TextMeshProUGUI warningText;  // 🔴 Shown when score is too low

    [Header("Timer Reference (assign in Inspector)")]
    public TextMeshProUGUI timerText;    // Timer display

    private int[] growthThresholds;
    private Coroutine warningCoroutine;  // Used to handle fade timing

    // Timer variables
    private float timeRemaining = 60f;   // default 60 seconds
    private bool timerRunning = false;

    // Initialize thresholds and score
    public void Initialize(int[] thresholds)
    {
        growthThresholds = thresholds;
        UpdateScore(0, 1);

        if (maxLevelText != null)
            maxLevelText.gameObject.SetActive(false); // Hide at start

        if (warningText != null)
            warningText.gameObject.SetActive(false); // Hide at start
    }

    // Update score, level, and progress bar
    public void UpdateScore(int score, int level)
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (levelText != null) levelText.text = "Level: " + level;

        // ✅ Handle max level display
        if (level > growthThresholds.Length)
        {
            if (progressBar != null) progressBar.gameObject.SetActive(false);
            if (maxLevelText != null)
            {
                maxLevelText.gameObject.SetActive(true);
                maxLevelText.text = "MAX LEVEL";
            }
            return;
        }

        // ✅ Normal level progress
        if (progressBar != null)
        {
            if (level - 1 < growthThresholds.Length)
            {
                int prev = (level - 2 >= 0) ? growthThresholds[level - 2] : 0;
                int next = growthThresholds[level - 1];
                progressBar.gameObject.SetActive(true);
                progressBar.minValue = prev;
                progressBar.maxValue = next;
                progressBar.value = score;
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
        }

        // Hide “MAX LEVEL” text when not at max
        if (maxLevelText != null)
            maxLevelText.gameObject.SetActive(false);
    }

    // ✅ Called by PlayerGrowth when max level is reached
    public void ShowMaxLevel()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        if (maxLevelText != null)
        {
            maxLevelText.gameObject.SetActive(true);
            maxLevelText.text = "MAX LEVEL";
        }
    }

    // Start the timer with a specific duration
    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        timerRunning = true;
        UpdateTimerUI(timeRemaining);
    }

    private void Start()
    {
        timeRemaining = 60f;
        timerRunning = true;
        UpdateTimerUI(timeRemaining);
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                timerRunning = false;
                OnTimerEnd();
            }

            UpdateTimerUI(timeRemaining);
        }
    }

    // Update the TMP timer text in MM:SS format
    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Called when timer reaches 0
    private void OnTimerEnd()
    {
        UnityEngine.Debug.Log("Time's up!");

        // Store results from PlayerGrowth
        if (FindObjectOfType<PlayerGrowth>() != null)
        {
            PlayerGrowth player = FindObjectOfType<PlayerGrowth>();
            CleanUpGameResults.score = player.score;
            CleanUpGameResults.playerLevel = player.playerLevel;

            // Assign stars based on score
            if (player.score > 400) CleanUpGameResults.starCount = 3;
            else if (player.score > 300) CleanUpGameResults.starCount = 2;
            else if (player.score > 230) CleanUpGameResults.starCount = 1;
            else CleanUpGameResults.starCount = 0;
        }
        UnityEngine.Debug.Log("Redirecting to results scene...");
        // Load the results scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Meds_ResultsScene"); // Replace with your results scene name
    }


    // 🔴 NEW FEATURE: Show red warning text when score too low
    public void ShowScoreTooLow()
    {
        if (warningText == null) return;

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(ShowScoreTooLowCoroutine());
    }

    private IEnumerator ShowScoreTooLowCoroutine()
    {
        warningText.text = "- SCORE TOO LOW";
        warningText.color = new Color(1f, 0f, 0f, 1f); // full red, fully visible
        warningText.gameObject.SetActive(true);

        // Wait 0.5s fully visible, then fade out over 1s
        yield return new WaitForSeconds(0.5f);

        float duration = 1f;
        float elapsed = 0f;
        Color startColor = warningText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            warningText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        warningText.gameObject.SetActive(false);
        warningCoroutine = null;
    }
}
