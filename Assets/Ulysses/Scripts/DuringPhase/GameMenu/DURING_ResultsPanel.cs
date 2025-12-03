using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DURING_ResultsPanel : MonoBehaviour
{
    public static DURING_ResultsPanel Instance;

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;     // Shows total score
    public Image[] starImages;            // 3 separate images: [0]=1 star, [1]=2 stars, [2]=3 stars

    public DURING_PauseMenu pauseMenu;

    [Header("Leaderboard Submission")]
    public TMP_InputField playerNameInput; // assign in Inspector
    public Button submitScoreButton; // assign in Inspector
    public Button openLeaderboardButton; // optional
    private bool scoreSubmitted;

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);

        // Hide all stars at the start
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }

        // Default key for Typhoon during phase (separate)
        if (string.IsNullOrEmpty(GameData.gameName))
            GameData.gameName = "Typhoon_During";
    }

    public void ShowResults(int rescuedNPCs, int rescuedPets)
    {
        panel.SetActive(true);

        if (pauseMenu != null)
            pauseMenu.canPause = false;

        // Calculate score
        int totalScore = (rescuedNPCs * 10) + (rescuedPets * 20);

        // Show score only
        scoreText.text = $"{totalScore}";

        // Determine number of stars
        int stars = 0;
        if (totalScore >= 100) stars = 3;
        else if (totalScore >= 60) stars = 2;
        else stars = 1;

        // Hide all star images first
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }

        // ✅ Only enable star image if valid
        if (stars > 0 && stars <= starImages.Length && starImages[stars - 1] != null)
        {
            starImages[stars - 1].gameObject.SetActive(true);
        }

        // Wire submission (reuse computed totalScore)
        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(() => SubmitScore(totalScore));
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
        UjoeLeaderboardManager.AddEntry("Typhoon_During", playerName, score);
        scoreSubmitted = true;
        if (submitScoreButton != null) submitScoreButton.interactable = false;
        if (scoreText != null) scoreText.text += $"\nSaved as: {playerName}";
    }

    private void OpenLeaderboard()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LeaderboardScene");
    }
}
