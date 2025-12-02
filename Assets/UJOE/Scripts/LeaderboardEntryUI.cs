using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void Setup(int rank, string playerName, int score, Color color)
    {
        if (rankText != null) rankText.text = "#" + rank;
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
        // Apply color
        if (rankText != null) rankText.color = color;
        if (nameText != null) nameText.color = color;
        if (scoreText != null) scoreText.color = color;
    }
}
