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
    [Header("Dynamic Generation (Optional)")]
    public Transform entriesParent; // Assign a vertical layout group parent
    public GameObject entryPrefab; // Prefab with LeaderboardEntryUI component
    public Color highlightColor = Color.yellow;
    public Color normalColor = Color.white;

    private void Start()
    {
        string key = string.IsNullOrEmpty(overrideGameKey) ? GameData.gameName : overrideGameKey;
        if (titleText != null)
        {
            titleText.text = $"Leaderboard - {key}";
        }

        var entries = UjoeLeaderboardManager.LoadEntries(key);
        var last = UjoeLeaderboardManager.LoadLastSubmitted(key);

        if (entries.Count == 0)
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

        // Dynamic path if prefab & parent provided
        if (entryPrefab != null && entriesParent != null)
        {
            // Clear previous children (editor-friendly)
            for (int i = entriesParent.childCount - 1; i >= 0; i--)
            {
                Destroy(entriesParent.GetChild(i).gameObject);
            }
            for (int i = 0; i < entries.Count; i++)
            {
                var go = Instantiate(entryPrefab, entriesParent);
                var ui = go.GetComponent<LeaderboardEntryUI>();
                bool highlight = last != null && last.name == entries[i].name && last.score == entries[i].score;
                if (ui != null)
                {
                    ui.Setup(i + 1, entries[i].name, entries[i].score, highlight ? highlightColor : normalColor);
                }
                else
                {
                    // Fallback if prefab missing component
                    var text = go.GetComponent<TMP_Text>();
                    if (text != null)
                    {
                        text.text = $"#{i + 1}: {entries[i].name} - {entries[i].score}";
                        text.color = highlight ? highlightColor : normalColor;
                    }
                }
            }
            // Hide legacy array entries if any
            if (scoreTexts != null)
            {
                foreach (var t in scoreTexts) if (t != null) t.gameObject.SetActive(false);
            }
        }
        else
        {
            // Legacy array path
            for (int i = 0; i < scoreTexts.Length; i++)
            {
                if (scoreTexts[i] == null) continue;
                if (i < entries.Count)
                {
                    var e = entries[i];
                    bool highlight = last != null && last.name == e.name && last.score == e.score;
                    scoreTexts[i].text = $"#{i + 1}: {e.name} - {e.score}";
                    scoreTexts[i].color = highlight ? highlightColor : normalColor;
                    scoreTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    scoreTexts[i].gameObject.SetActive(false);
                }
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
