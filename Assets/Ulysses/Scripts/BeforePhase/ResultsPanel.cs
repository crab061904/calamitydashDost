using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultsPanel : MonoBehaviour
{
    public static ResultsPanel Instance; // Singleton

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public Image[] starImages; // [0]=1 star, [1]=2 stars, [2]=3 stars

    public PauseMenu pauseMenu; // Drag your PauseMenu object in Inspector

    [Header("Leaderboard Submission")]
    public TMP_InputField playerNameInput; // assign in Inspector
    public Button submitScoreButton; // assign in Inspector
    public Button openLeaderboardButton; // optional
    private bool scoreSubmitted;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
        else
            Debug.LogError("ResultsPanel: Panel reference is missing!");

        // Hide all stars at the start
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }

        // Default key for Typhoon before phase (separate)
        if (string.IsNullOrEmpty(GameData.gameName))
            GameData.gameName = "Typhoon_Before";
    }

    public void ShowResults(int score, float time)
    {
        panel.SetActive(true);

        // Disable pause menu while results are shown
        if (pauseMenu != null)
            pauseMenu.canPause = false;

        // Show score
        scoreText.text = $"{score}";

        // Show formatted time
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";

        // ✅ Stars based on time
        int stars = 1; // default
        if (time <= 240f)       
            stars = 3;
        else if (time <= 300f) 
            stars = 2;

        // Hide all stars first
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }

        // Show correct star image
        if (stars > 0 && stars <= starImages.Length && starImages[stars - 1] != null)
        {
            starImages[stars - 1].gameObject.SetActive(true);
        }

        // Wire submission UI when results are shown
        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(() => SubmitScore(score));
            submitScoreButton.interactable = playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text);
        }
        if (playerNameInput != null)
        {
            playerNameInput.onValueChanged.RemoveAllListeners();
            playerNameInput.onValueChanged.AddListener(_ =>
            {
                if (submitScoreButton != null)
                    submitScoreButton.interactable = !string.IsNullOrWhiteSpace(playerNameInput.text);
            });
        }
        if (openLeaderboardButton != null)
        {
            openLeaderboardButton.onClick.RemoveAllListeners();
            openLeaderboardButton.onClick.AddListener(OpenLeaderboard);
        }
    }

    private void SubmitScore(int score)
    {
        if (scoreSubmitted) return;
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text)) ? playerNameInput.text.Trim() : "Player";
        UjoeLeaderboardManager.AddEntry("Typhoon_Before", playerName, score);
        scoreSubmitted = true;
        if (submitScoreButton != null) submitScoreButton.interactable = false;
        // Optional: show confirmation in timeText if result label not available
        if (timeText != null) timeText.text += $"\nSaved as: {playerName}";
    }

    private void OpenLeaderboard()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LeaderboardScene");
    }
}
