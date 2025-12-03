using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CleanupResultsUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalLevelText;
    public TextMeshProUGUI resultText;
    
    [Header("Leaderboard Submission")]
    public TMP_InputField playerNameInput; // assign in Inspector
    public Button submitScoreButton; // assign in Inspector
    public Button openLeaderboardButton; // optional
    private bool scoreSubmitted;

    [Header("Star References")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject noStarVisual; // optional: show this when 0 stars

    private void Start()
    {
        // Show final score and level
        if (finalScoreText != null)
            finalScoreText.text = $"Score: {CleanUpGameResults.score}";
        if (finalLevelText != null)
            finalLevelText.text = $"Level: {CleanUpGameResults.playerLevel}";

        // Activate stars based on star count
        if (star1 != null) star1.SetActive(CleanUpGameResults.starCount >= 1);
        if (star2 != null) star2.SetActive(CleanUpGameResults.starCount >= 2);
        if (star3 != null) star3.SetActive(CleanUpGameResults.starCount >= 3);

        // Handle 0 stars
        if (noStarVisual != null)
            noStarVisual.SetActive(CleanUpGameResults.starCount == 0);

        // Optional result text
        if (resultText != null)
        {
            switch (CleanUpGameResults.starCount)
            {
                case 3: resultText.text = "Excellent!"; break;
                case 2: resultText.text = "Good Job!"; break;
                case 1: resultText.text = "Keep Practicing!"; break;
                default: resultText.text = "Try Again!"; break;
            }
        }

        // Ensure leaderboard key is set for Cleanup
        if (string.IsNullOrEmpty(GameData.gameName))
            GameData.gameName = "Recovery"; // or "Cleanup" if you prefer; match Leaderboard gameKeys[3]

        // Wire submit and input behavior
        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(SubmitScore);
        }
        if (playerNameInput != null && submitScoreButton != null)
        {
            submitScoreButton.interactable = !string.IsNullOrWhiteSpace(playerNameInput.text);
            playerNameInput.onValueChanged.RemoveAllListeners();
            playerNameInput.onValueChanged.AddListener(_ => UpdateSubmitInteractable());
        }
        if (openLeaderboardButton != null)
        {
            openLeaderboardButton.onClick.RemoveAllListeners();
            openLeaderboardButton.onClick.AddListener(OpenLeaderboard);
        }
    }

    // Button Actions
    public void RetryGame()
    {
        SceneManager.LoadScene("CleanupPhase"); // replace with your gameplay scene
    }

    public void NextGame()
    {
        SceneManager.LoadScene("Joe_Rescuing"); // replace with your gameplay scene
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("START MENU");
    }

    // Leaderboard hooks
    public void SubmitScore()
    {
        if (scoreSubmitted) return;
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
            ? playerNameInput.text.Trim()
            : "Player";

        UjoeLeaderboardManager.AddEntry(GameData.gameName, playerName, CleanUpGameResults.score);

        scoreSubmitted = true;
        if (submitScoreButton != null) submitScoreButton.interactable = false;
        if (resultText != null) resultText.text = $"Saved as: {playerName}";
    }

    private void UpdateSubmitInteractable()
    {
        if (submitScoreButton == null || playerNameInput == null) return;
        submitScoreButton.interactable = !string.IsNullOrWhiteSpace(playerNameInput.text);
    }

    public void OpenLeaderboard()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LeaderboardScene");
    }
}
