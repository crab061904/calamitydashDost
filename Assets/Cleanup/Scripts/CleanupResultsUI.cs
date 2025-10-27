using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CleanupResultsUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalLevelText;
    public TextMeshProUGUI resultText;

    [Header("Star References")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject noStarVisual; // optional: show this when 0 stars

    private void Start()
    {
        // Show final score and level
        if (finalScoreText != null)
            finalScoreText.text = $"Score: {CleanUpGameResults.score}";
        if (finalLevelText != null)
            finalLevelText.text = $"Level: {CleanUpGameResults.playerLevel}";

        // Activate stars based on star count
        if (star1 != null) star1.SetActive(CleanUpGameResults.starCount >= 1);
        if (star2 != null) star2.SetActive(CleanUpGameResults.starCount >= 2);
        if (star3 != null) star3.SetActive(CleanUpGameResults.starCount >= 3);

        // Handle 0 stars
        if (noStarVisual != null)
            noStarVisual.SetActive(CleanUpGameResults.starCount == 0);

        // Optional result text
        if (resultText != null)
        {
            switch (CleanUpGameResults.starCount)
            {
                case 3: resultText.text = "Excellent!"; break;
                case 2: resultText.text = "Good Job!"; break;
                case 1: resultText.text = "Keep Practicing!"; break;
                default: resultText.text = "Try Again!"; break;
            }
        }
    }

    // Button Actions
    public void RetryGame()
    {
        SceneManager.LoadScene("CleanupPhase"); // replace with your gameplay scene
    }

    public void NextGame()
    {
        SceneManager.LoadScene("Joe_Rescuing"); // replace with your gameplay scene
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("START MENU");
    }
}
