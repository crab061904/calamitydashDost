using System.Collections.Generic;
using UnityEngine;

// Local fallback leaderboard manager to avoid cross-folder compile issues
public static class UjoeLeaderboardManager
{
    private const int maxEntries = 5;

    public static void AddScore(string gameKey, int newScore)
    {
        List<int> scores = LoadScores(gameKey);
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a));
        if (scores.Count > maxEntries)
            scores.RemoveRange(maxEntries, scores.Count - maxEntries);

        for (int i = 0; i < scores.Count; i++)
            PlayerPrefs.SetInt($"{gameKey}_{i}", scores[i]);

        PlayerPrefs.SetInt($"{gameKey}_Count", scores.Count);
        PlayerPrefs.Save();
    }

    public static List<int> LoadScores(string gameKey)
    {
        List<int> scores = new List<int>();
        int count = PlayerPrefs.GetInt($"{gameKey}_Count", 0);
        for (int i = 0; i < count; i++)
        {
            int score = PlayerPrefs.GetInt($"{gameKey}_{i}", 0);
            scores.Add(score);
        }
        return scores;
    }
}
