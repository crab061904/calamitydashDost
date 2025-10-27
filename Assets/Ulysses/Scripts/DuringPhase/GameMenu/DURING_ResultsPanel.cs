using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DURING_ResultsPanel : MonoBehaviour
{
    public static DURING_ResultsPanel Instance;

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;     // Shows total score
    public Image[] starImages;            // 3 separate images: [0]=1 star, [1]=2 stars, [2]=3 stars

    public DURING_PauseMenu pauseMenu;

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);

        // Hide all stars at the start
        foreach (var img in starImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }
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
    }
}
