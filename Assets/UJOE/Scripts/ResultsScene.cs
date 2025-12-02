using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ResultsScene : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI boxesDeliveredText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI achievementsText;
    [Header("Name Submission")]
    public TMP_InputField playerNameInput; // Assign in Inspector
    public Button submitScoreButton; // Assign in Inspector
    private bool scoreSubmitted;


    [Header("Star References")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    [Header("Result Text (Optional)")]
    public TextMeshProUGUI resultText;
    private void Start()
    {
         // Hide all stars first
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        // Then enable stars based on the player's performance
        if (GameData.starCount >= 1) star1.SetActive(true);
        if (GameData.starCount >= 2) star2.SetActive(true);
        if (GameData.starCount >= 3) star3.SetActive(true);
         if (resultText != null)
        {
            resultText.text = $"Stars Earned: {GameData.starCount}/3";
        }
        ShowResults();
        // Wait for player to submit name; do not auto-add score.
        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(SubmitScore);
        }
    }

    private void ShowResults()
    {
        if (objectiveText != null)
            objectiveText.text = GameData.objective;

        if (boxesDeliveredText != null)
            boxesDeliveredText.text = $"Boxes Delivered: {GameData.boxesDelivered}/{GameData.boxesGoal}";

        if (timeRemainingText != null)
            timeRemainingText.text = $"Time Remaining: {GameData.timeRemaining:00}s";

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {GameData.score}";

        if (achievementsText != null)
        {
            if (GameData.boxesDelivered == GameData.boxesGoal)
                achievementsText.text = "Achievement: Perfect Delivery!";
            else if (GameData.boxesDelivered > 0)
                achievementsText.text = "Partial Success!";
            else
                achievementsText.text = "Try Again!";
        }
        // 🌟 Star Display Logic
        if (star1 != null) star1.SetActive(GameData.starCount >= 1);
        if (star2 != null) star2.SetActive(GameData.starCount >= 2);
        if (star3 != null) star3.SetActive(GameData.starCount >= 3);
    }

    // 🌀 Button Actions
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Joe_Rescuing"); // Replace with your current gameplay scene name
    }

    public void NextMiniGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CleanupPhase"); // Replace with your next mini-game scene name
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("START MENU"); // Replace with your main menu scene name
    }

    // 📊 Open Leaderboard Scene
    public void ShowLeaderboard()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LeaderboardScene"); // Create a scene named "LeaderboardScene" and add LeaderboardSceneController
    }

    // 📝 Called by Submit button
    public void SubmitScore()
    {
        if (scoreSubmitted) return; // Prevent duplicate submissions
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text)) ? playerNameInput.text.Trim() : "Player";
        UjoeLeaderboardManager.AddEntry(GameData.gameName, playerName, GameData.score);
        scoreSubmitted = true;
        if (submitScoreButton != null) submitScoreButton.interactable = false;
        // Optionally give feedback
        if (resultText != null) resultText.text = $"Saved as: {playerName}";
    }
}
