using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : MonoBehaviour
{
    public static ResultsPanel Instance; // Singleton

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public Image[] starImages; // [0]=1 star, [1]=2 stars, [2]=3 stars

    public PauseMenu pauseMenu; // Drag your PauseMenu object in Inspector

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
        else
            Debug.LogError("ResultsPanel: Panel reference is missing!");

        // Hide all stars at the start
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }
    }

    public void ShowResults(int score, float time)
    {
        panel.SetActive(true);

        // Disable pause menu while results are shown
        if (pauseMenu != null)
            pauseMenu.canPause = false;

        // Show score
        scoreText.text = $"{score}";

        // Show formatted time
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";

        // ✅ Stars based on time
        int stars = 1; // default
        if (time <= 240f)       
            stars = 3;
        else if (time <= 300f) 
            stars = 2;

        // Hide all stars first
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }

        // Show correct star image
        if (stars > 0 && stars <= starImages.Length && starImages[stars - 1] != null)
        {
            starImages[stars - 1].gameObject.SetActive(true);
        }
    }
}
