using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI boxesHeldText;
    public TextMeshProUGUI boxesNeededText;
    public TextMeshProUGUI deliveryMessage;

    [Header("Game Settings")]
    public float gameTime = 120f; // 2 minutes
    private float currentTime;

    [Header("Gameplay Data")]
    public int score = 0;
    public int boxesHeld = 0;
    public int boxesNeeded = 5;
    public int maxBoxes = 3;
    public int pointsPerBox = 10;
    private int totalBoxesToDeliver;

    [Header("Delivery Message")]
    public float deliveryMessageDuration = 1f;

    [Header("Visual Effects")]
    public CanvasGroup fadePanel; // Optional fade effect
    public float fadeDuration = 1.5f;

    [Header("Audio")]
    public GameAudioManager audioManager; // link to AudioManager in scene

    private bool gameEnded = false;

    private void Start()
    {
        currentTime = gameTime;
        totalBoxesToDeliver = boxesNeeded;
        UpdateUI();

        if (deliveryMessage != null)
            deliveryMessage.gameObject.SetActive(false);

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        //if (audioManager != null)
            //audioManager.PlayBGM(); // Start background music
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(currentTime);

        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (boxesHeldText != null)
            boxesHeldText.text = "Boxes: " + boxesHeld;

        if (boxesNeededText != null)
            boxesNeededText.text = "Needed: " + boxesNeeded;
    }

    public void AddBox()
    {
        if (boxesHeld < maxBoxes)
        {
            boxesHeld++;
            UpdateUI();

            if (audioManager != null)
                audioManager.PlayPickupSound(); // play pickup SFX
        }
        else
        {
            Debug.Log("Cannot pick up more boxes! Max reached.");
        }
    }

    public void DeliverBox()
    {
        if (boxesHeld > 0 && boxesNeeded > 0)
        {
            boxesHeld--;
            boxesNeeded--;
            score += pointsPerBox;
            UpdateUI();

            if (deliveryMessage != null)
                StartCoroutine(ShowDeliveryMessage("Box Delivered!"));

            if (audioManager != null)
                audioManager.PlayDeliverSound(); // play delivery SFX

            if (boxesNeeded <= 0)
                StartCoroutine(EndGameAfterDelay(deliveryMessageDuration));
        }
        else
        {
            Debug.Log("No boxes to deliver or all boxes delivered!");
        }
    }

    private System.Collections.IEnumerator ShowDeliveryMessage(string message)
    {
        deliveryMessage.text = message;
        deliveryMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(deliveryMessageDuration);
        deliveryMessage.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndGame();
    }

    private void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        int boxesDelivered = totalBoxesToDeliver - boxesNeeded;

        // ✅ Save data before switching scenes
        GameData.score = score;
        GameData.boxesDelivered = boxesDelivered;
        GameData.boxesGoal = totalBoxesToDeliver;
        GameData.timeRemaining = Mathf.CeilToInt(currentTime);
        GameData.objective = boxesNeeded <= 0 ? "Objective: Completed" : "Objective: Incomplete";

        // ⭐ Determine star count
        if (boxesDelivered <= 0)
        {
            GameData.starCount = (score > 0) ? 1 : 0; // picked boxes = 1, none = 0
        }
        else if (boxesDelivered < totalBoxesToDeliver)
        {
            GameData.starCount = 2;
        }
        else
        {
            GameData.starCount = 3;
        }

        // ✅ Smooth fade before scene switch
        if (fadePanel != null)
        {
            StartCoroutine(FadeAndLoadResults());
        }
        else
        {
            SceneManager.LoadScene("ResultsScene");
        }
    }

    private System.Collections.IEnumerator FadeAndLoadResults()
    {
        float elapsed = 0f;
        fadePanel.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("ResultsScene");
    }
}
