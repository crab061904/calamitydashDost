using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManagerFire : MonoBehaviour
{
    public static GameManagerFire Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [Header("Player Components")]
    public GameObject player;
    public MonoBehaviour movementScript;
    public HoseController hoseController;
    public CameraChange cameraChange;
    public MonoBehaviour cameraLookScript;

    private Fire[] fires;
    private int totalFires;
    private int extinguishedFires;
    private float timer;
    private bool allFiresOut = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        fires = Object.FindObjectsByType<Fire>(FindObjectsSortMode.None);
        totalFires = fires.Length;
        extinguishedFires = 0;

        Debug.Log($"[GameManagerFire] Found {totalFires} total fires in scene.");

        if (timerText != null)
            timerText.text = "0.0s";
        if (scoreText != null)
            scoreText.text = $"0/{totalFires}";
    }

    void Update()
    {
        if (allFiresOut) return;

        timer += Time.deltaTime;

        if (timerText != null)
            timerText.text = $"{timer:F1}s";

        if (scoreText != null)
            scoreText.text = $"{extinguishedFires}/{totalFires}";

        // DEBUG KEY — press T to force finish
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[DEBUG] T pressed — forcing results.");
            OnAllFiresExtinguished();
        }
    }

    public void FireExtinguished()
    {
        extinguishedFires++;
        Debug.Log($"[GameManagerFire] Fire extinguished! Count: {extinguishedFires}/{totalFires}");

        if (extinguishedFires >= totalFires && !allFiresOut)
        {
            allFiresOut = true;
            Debug.Log("[GameManagerFire] ✅ All fires extinguished — calling OnAllFiresExtinguished()");
            OnAllFiresExtinguished();
        }
    }

    private void OnAllFiresExtinguished()
    {
        Debug.Log("[GameManagerFire] 🔥 OnAllFiresExtinguished() triggered");

        int stars = CalculateStars();

        // Save data
        GameDataFire.objective = "Extinguish All Fires";
        GameDataFire.firesExtinguished = extinguishedFires;
        GameDataFire.totalFires = totalFires;
        GameDataFire.elapsedTime = timer;
        GameDataFire.finalScore = extinguishedFires * 100;
        GameDataFire.starsEarned = stars;

        Debug.Log($"[GameManagerFire] Data Saved — Stars: {stars}, Time: {timer:F1}s");

        // Disable gameplay scripts
        if (movementScript) movementScript.enabled = false;
        if (hoseController) hoseController.enabled = false;
        if (cameraChange) cameraChange.enabled = false;
        if (cameraLookScript) cameraLookScript.enabled = false;

        // Unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Change scene fast (0.1s delay)
        StartCoroutine(LoadResultsSceneAfterDelay(0.1f));
    }

    private IEnumerator LoadResultsSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene("ResultsSceneFire");
    }

    private int CalculateStars()
    {
        if (timer <= 120f) return 3;
        if (timer <= 180f) return 2;
        if (timer <= 240f) return 1;
        return 0;
    }
}
