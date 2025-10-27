using UnityEngine;

public class PlayerGrowth : MonoBehaviour
{
    public int score = 0;
    public int playerLevel = 1;

    private readonly int[] growthThresholds = { 5, 25, 70, 100, 130, 180, 230, 300, 350, 400 };
    private readonly float[] growthSteps = { 1.0f, 2.5f, 4.0f, 5.5f, 6.0f, 6.4f, 6.8f, 7.1f, 7.3f, 7.5f };

    [Header("UI (optional)")]
    public UIManager uiManager;

    public delegate void OnGrowth(int newLevel);
    public event OnGrowth onGrowth;

    public int[] GrowthThresholds => growthThresholds;

    private Vector3 baseScale;
    private float baseY;

    void Start()
    {
        baseScale = transform.localScale;
        baseY = transform.position.y;

        if (uiManager == null)
        {
#if UNITY_2023_1_OR_NEWER
            uiManager = UnityEngine.Object.FindFirstObjectByType<UIManager>();
#else
            uiManager = UnityEngine.Object.FindObjectOfType<UIManager>();
#endif
        }

        if (uiManager != null)
        {
            uiManager.Initialize(growthThresholds);
            uiManager.UpdateScore(score, playerLevel);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debris debris = other.GetComponent<Debris>();
        if (debris != null)
        {
            if (score + 1 >= debris.sizeValue)
            {
                score += debris.sizeValue;
                UnityEngine.Debug.Log("Ate debris! Score: " + score);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayDebrisEat();

                UpdateUI();

                int nextThresholdIndex = playerLevel - 1;
                if (nextThresholdIndex >= 0 && nextThresholdIndex < growthThresholds.Length)
                {
                    if (score >= growthThresholds[nextThresholdIndex])
                        Grow();
                }

                Destroy(other.gameObject);
            }
            else
            {
                int penalty = debris.sizeValue / 2;
                score = Mathf.Max(0, score - penalty);

                UnityEngine.Debug.Log($"Too weak! Lost {penalty} points. Score={score}");

                if (uiManager != null)
                    uiManager.ShowScoreTooLow();

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayDebrisFail();

                UpdateUI();

                int prevThresholdIndex = Mathf.Max(0, playerLevel - 2);
                if (playerLevel > 1 && score < growthThresholds[prevThresholdIndex])
                    Shrink();
            }

        }
    }

    public void Shrink()
    {
        if (playerLevel <= 1) return;

        int newLevel = playerLevel - 1;
        playerLevel = newLevel;

        Vector3 oldScale = transform.localScale;

        Vector3 targetScale = baseScale;
        for (int i = 0; i < newLevel - 1; i++)
        {
            targetScale.x += growthSteps[i];
            targetScale.z += growthSteps[i];
            targetScale.y += growthSteps[i] * 0.1f;
        }

        float deltaY = (targetScale.y - oldScale.y) / 2f;
        transform.position += new Vector3(0, deltaY, 0);

        transform.localScale = targetScale;

        UnityEngine.Debug.Log("Player shrank to level " + playerLevel);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelDown();

        UpdateUI();

        onGrowth?.Invoke(playerLevel);
    }

    void Grow()
    {
        if (playerLevel >= 10) return; // ✅ Prevent level 11

        int newLevel = playerLevel + 1;
        playerLevel = newLevel;

        Vector3 oldScale = transform.localScale;

        Vector3 targetScale = baseScale;
        for (int i = 0; i < newLevel - 1; i++)
        {
            targetScale.x += growthSteps[i];
            targetScale.z += growthSteps[i];
            targetScale.y += growthSteps[i] * 0.1f;
        }

        float deltaY = (targetScale.y - oldScale.y) / 2f;
        transform.position += new Vector3(0, deltaY, 0);

        transform.localScale = targetScale;

        UnityEngine.Debug.Log("Player grew to level " + playerLevel);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelUp();

        UpdateUI();

        onGrowth?.Invoke(playerLevel);
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score, playerLevel);

            // ✅ When player reaches max level, show "MAX LEVEL"
            if (playerLevel >= 10)
                uiManager.ShowMaxLevel();
        }
    }
}
