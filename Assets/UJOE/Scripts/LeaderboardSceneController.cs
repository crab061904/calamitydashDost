using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardSceneController : MonoBehaviour
{
    [Header("Scene Names")]
    public string resultsSceneName = "ResultsScene"; // Set in Inspector to actual results scene name
    public string mainMenuSceneName = "START MENU";  // Set to your main menu scene name

    [Header("Game Key (optional override)")]
    public string overrideGameKey; // Leave empty to use GameData.gameName

    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text[] scoreTexts; // Assign an array of TextMeshPro texts for top entries
    public TMP_Text emptyStateText; // Shown if no scores

    private void Start()
    {
        string key = string.IsNullOrEmpty(overrideGameKey) ? GameData.gameName : overrideGameKey;
        if (titleText != null)
        {
            titleText.text = $"Leaderboard - {key}";
        }

        List<int> scores = UjoeLeaderboardManager.LoadScores(key);

        if (scores.Count == 0)
        {
            if (emptyStateText != null) emptyStateText.gameObject.SetActive(true);
            if (scoreTexts != null)
            {
                foreach (var t in scoreTexts)
                {
                    if (t != null) t.gameObject.SetActive(false);
                }
            }
            return;
        }
        else if (emptyStateText != null)
        {
            emptyStateText.gameObject.SetActive(false);
        }

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (scoreTexts[i] == null) continue;
            if (i < scores.Count)
            {
                scoreTexts[i].text = $"#{i + 1}: {scores[i]}";
                scoreTexts[i].gameObject.SetActive(true);
            }
            else
            {
                scoreTexts[i].gameObject.SetActive(false);
            }
        }
    }

    // Button: Back to Results
    public void BackToResults()
    {
        SceneManager.LoadScene(resultsSceneName);
    }

    // Button: Go to Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
