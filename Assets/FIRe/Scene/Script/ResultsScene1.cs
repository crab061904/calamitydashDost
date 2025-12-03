using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsSceneFire : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI firesExtinguishedText;
    public TextMeshProUGUI timeTakenText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI achievementsText;

    [Header("Star Rating Images")]
    public GameObject star1; // 1 star image
    public GameObject star2; // 2 stars image
    public GameObject star3; // 3 stars image
    public GameObject emptyStar; // Empty silhouette image

    [Header("Optional Result Text")]
    public TextMeshProUGUI resultText;

    [Header("Buttons")]
    public Button retryButton;
    public Button nextButton;
    public Button mainMenuButton;

    [Header("Leaderboard Submission")]
    public TMP_InputField playerNameInput; // assign in Inspector
    public Button submitScoreButton; // assign in Inspector
    public Button openLeaderboardButton; // optional: assign to open leaderboard
    private bool scoreSubmitted;

    private void Start()
    {
        // Display all the results on the screen
        ShowResults();

        // Ensure the correct leaderboard key is set
        if (string.IsNullOrEmpty(GameData.gameName))
            GameData.gameName = "Fire";

        // Hook up submission
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

    public void ShowResults()
    {
        if (objectiveText != null)
            objectiveText.text = GameDataFire.objective;

        if (firesExtinguishedText != null)
            firesExtinguishedText.text = $"Fires Extinguished: {GameDataFire.firesExtinguished}/{GameDataFire.totalFires}";

        if (timeTakenText != null)
            timeTakenText.text = $"Time Taken: {GameDataFire.elapsedTime:F1}s";

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {GameDataFire.finalScore}";

        // --- MODIFIED SECTION START ---
        // Sets the achievement text based on the number of stars earned.
        if (achievementsText != null)
        {
            switch (GameDataFire.starsEarned)
            {
                case 3:
                    achievementsText.text = "Outstanding Performance!";
                    break;
                case 2:
                    achievementsText.text = "Great Job!";
                    break;
                case 1:
                    achievementsText.text = "Good Effort!";
                    break;
                default: // This covers 0 stars
                    achievementsText.text = "Better Luck Next Time!";
                    break;
            }
        }
        // --- MODIFIED SECTION END ---

        // Deactivate all star images first to reset the state
        if (emptyStar != null) emptyStar.SetActive(false);
        if (star1 != null) star1.SetActive(false);
        if (star2 != null) star2.SetActive(false);
        if (star3 != null) star3.SetActive(false);

        // Activate the correct star image based on score
        switch (GameDataFire.starsEarned)
        {
            case 3:
                if (star3 != null) star3.SetActive(true);
                break;
            case 2:
                if (star2 != null) star2.SetActive(true);
                break;
            case 1:
                if (star1 != null) star1.SetActive(true);
                break;
            default: // 0 stars
                if (emptyStar != null) emptyStar.SetActive(true);
                break;
        }

        if (resultText != null)
            resultText.text = $"Stars Earned: {GameDataFire.starsEarned}/3";
    }

    // --- Button Actions ---
    // These functions must be linked in the Inspector.

    public void RetryGame()
    {
        Debug.Log("Retry Game clicked");
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("finalfire");
    }

    public void NextMiniGame()
    {
        Debug.Log("Next MiniGame clicked");
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("CleanupPhase");
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Return to Main Menu clicked");
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("START MENU");
    }

    // --- Leaderboard hooks ---
    public void SubmitScore()
    {
        if (scoreSubmitted) return;
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
            ? playerNameInput.text.Trim()
            : "Player";

        // Save name + score under the current game key
        UjoeLeaderboardManager.AddEntry(GameData.gameName, playerName, GameDataFire.finalScore);

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