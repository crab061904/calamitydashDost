using System.Collections.Generic;
using UnityEngine;
using System;

// Leaderboard manager supporting player names + scores
public static class UjoeLeaderboardManager
{
    private const int maxEntries = 5;

    [Serializable]
    public class LeaderboardEntry
    {
        public string name;
        public int score;
    }

    [Serializable]
    private class EntryListWrapper
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    private static string GetJsonKey(string gameKey) => $"{gameKey}_Entries";

    // New method: add name + score
    public static void AddEntry(string gameKey, string playerName, int newScore)
    {
        var list = LoadEntries(gameKey);
        list.Add(new LeaderboardEntry { name = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName, score = newScore });
        // Sort descending by score
        list.Sort((a, b) => b.score.CompareTo(a.score));
        // Trim
        if (list.Count > maxEntries) list.RemoveRange(maxEntries, list.Count - maxEntries);
        SaveEntries(gameKey, list);
        // Store last submitted entry for highlight
        PlayerPrefs.SetString(GetLastNameKey(gameKey), playerName);
        PlayerPrefs.SetInt(GetLastScoreKey(gameKey), newScore);
        PlayerPrefs.Save();
    }

    // Backwards compatibility wrapper (kept so existing calls compile)
    public static void AddScore(string gameKey, int newScore)
    {
        AddEntry(gameKey, "Player", newScore);
    }

    public static List<LeaderboardEntry> LoadEntries(string gameKey)
    {
        string key = GetJsonKey(gameKey);
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<EntryListWrapper>(json);
                    if (wrapper != null && wrapper.entries != null) return wrapper.entries;
                }
                catch { }
            }
        }
        // Migration from old int-only storage if exists
        int count = PlayerPrefs.GetInt($"{gameKey}_Count", 0);
        var migrated = new List<LeaderboardEntry>();
        for (int i = 0; i < count; i++)
        {
            int score = PlayerPrefs.GetInt($"{gameKey}_{i}", 0);
            migrated.Add(new LeaderboardEntry { name = "Player", score = score });
        }
        if (migrated.Count > 0)
        {
            SaveEntries(gameKey, migrated);
        }
        return migrated;
    }

    private static void SaveEntries(string gameKey, List<LeaderboardEntry> entries)
    {
        var wrapper = new EntryListWrapper { entries = entries };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(GetJsonKey(gameKey), json);
        PlayerPrefs.Save();
    }

    private static string GetLastNameKey(string gameKey) => $"{gameKey}_LastName";
    private static string GetLastScoreKey(string gameKey) => $"{gameKey}_LastScore";

    public static LeaderboardEntry LoadLastSubmitted(string gameKey)
    {
        if (!PlayerPrefs.HasKey(GetLastScoreKey(gameKey))) return null;
        string name = PlayerPrefs.GetString(GetLastNameKey(gameKey), "Player");
        int score = PlayerPrefs.GetInt(GetLastScoreKey(gameKey), 0);
        return new LeaderboardEntry { name = name, score = score };
    }
}
