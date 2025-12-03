using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardSceneController : MonoBehaviour
{
    [Header("Scene Names")]
    public string resultsSceneName = "ResultsScene"; // Set in Inspector to actual results scene name
    public string mainMenuSceneName = "START MENU";  // Set to your main menu scene name

    [Header("Game Keys")]
    [Tooltip("Optional: Force a starting game key. If empty, uses GameData.gameName or first in gameKeys.")]
    public string overrideGameKey; // Starting key override
    [Tooltip("Keys for each game (set 4 for four buttons)")]
    public string[] gameKeys = new string[4];

    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text[] scoreTexts; // Assign an array of TextMeshPro texts for top entries
    public TMP_Text emptyStateText; // Shown if no scores
    [Header("Dynamic Generation (Optional)")]
    public RectTransform entriesParent; // Assign a UI parent (Vertical Layout Group)
    public GameObject entryPrefab; // Prefab with LeaderboardEntryUI component
    public Color highlightColor = Color.yellow;
    public Color normalColor = Color.white;

    private string currentKey;

    private void Start()
    {
        // Decide starting key
        currentKey = !string.IsNullOrEmpty(overrideGameKey)
            ? overrideGameKey
            : (!string.IsNullOrEmpty(GameData.gameName) ? GameData.gameName : FirstNonEmptyKey());

        RefreshUI();
    }

    private string FirstNonEmptyKey()
    {
        if (gameKeys != null)
        {
            foreach (var k in gameKeys)
            {
                if (!string.IsNullOrEmpty(k)) return k;
            }
        }
        return "Game"; // fallback label
    }

    private void RefreshUI()
    {
        if (string.IsNullOrEmpty(currentKey)) currentKey = FirstNonEmptyKey();

        if (titleText != null)
        {
            titleText.text = $"Leaderboard - {currentKey}";
        }

        var entries = UjoeLeaderboardManager.LoadEntries(currentKey);
        var last = UjoeLeaderboardManager.LoadLastSubmitted(currentKey);

        bool hasAny = entries != null && entries.Count > 0;
        if (emptyStateText != null) emptyStateText.gameObject.SetActive(!hasAny);

        if (!hasAny)
        {
            // Hide legacy texts if present
            if (scoreTexts != null)
            {
                foreach (var t in scoreTexts) if (t != null) t.gameObject.SetActive(false);
            }
            // Clear dynamic children
            if (entriesParent != null)
            {
                for (int i = entriesParent.childCount - 1; i >= 0; i--)
                    Destroy(entriesParent.GetChild(i).gameObject);
            }
            return;
        }

        if (entryPrefab != null && entriesParent != null)
        {
            // Dynamic path
            for (int i = entriesParent.childCount - 1; i >= 0; i--)
                Destroy(entriesParent.GetChild(i).gameObject);

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
                    var text = go.GetComponent<TMP_Text>();
                    if (text != null)
                    {
                        text.text = $"#{i + 1}: {entries[i].name} - {entries[i].score}";
                        text.color = highlight ? highlightColor : normalColor;
                    }
                }
            }
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

    // Button hooks to switch games
    public void ShowGameByKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        currentKey = key;
        RefreshUI();
    }

    public void ShowGameIndex(int index)
    {
        if (gameKeys == null || index < 0 || index >= gameKeys.Length) return;
        if (string.IsNullOrEmpty(gameKeys[index])) return;
        currentKey = gameKeys[index];
        RefreshUI();
    }

    // Convenience wrappers for Unity buttons without parameters
    public void ShowGame0() { ShowGameIndex(0); }
    public void ShowGame1() { ShowGameIndex(1); }
    public void ShowGame2() { ShowGameIndex(2); }
    public void ShowGame3() { ShowGameIndex(3); }

    // Button: Back to Results
    public void BackToResults()
    {
        SceneManager.LoadScene("ResultsScene");
    }

    // Button: Go to Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("START MENU");
    }
}
