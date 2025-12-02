using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsScene : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI boxesDeliveredText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI achievementsText;


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
        // Add score to leaderboard
        UjoeLeaderboardManager.AddScore(GameData.gameName, GameData.score);
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
}
