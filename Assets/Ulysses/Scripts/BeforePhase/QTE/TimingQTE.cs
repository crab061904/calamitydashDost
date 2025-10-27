using UnityEngine;
using UnityEngine.UI;
using System;

public class TimingQTE : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider timingBar;
    public RectTransform safeZone;

    [Header("Settings")]
    public float speed = 2f;
    public float safeZoneWidth = 0.3f; // 30% of the slider width

    private bool movingRight = true;
    private bool isActive = false;

    private Action onSuccess;
    private Action onFail;
    private Action onComplete;


    private bool safeZoneSet = false;  // Track if safe zone has been positioned

    void Start()
    {
        if (timingBar != null)
        {
            timingBar.value = 0f;
            timingBar.gameObject.SetActive(false);
        }

        if (safeZone != null)
            safeZone.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive || timingBar == null) return;

        MoveBar();

        if (Input.GetKeyDown(KeyCode.E))
            CheckResult();
    }

    void MoveBar()
    {
        if (movingRight)
            timingBar.value += Time.deltaTime * speed;
        else
            timingBar.value -= Time.deltaTime * speed;

        if (timingBar.value >= timingBar.maxValue)
            movingRight = false;
        else if (timingBar.value <= timingBar.minValue)
            movingRight = true;
    }

    void CheckResult()
    {
        float leftEdge = safeZone.anchorMin.x;
        float rightEdge = safeZone.anchorMax.x;
        float barNorm = timingBar.value / timingBar.maxValue;

        if (barNorm >= leftEdge && barNorm <= rightEdge)
        {
            Debug.Log("✅ QTE Success!");
            onSuccess?.Invoke();
            EndQTE();
        }
        else
        {
            Debug.Log("❌ QTE Failed!");
            onFail?.Invoke();
            // Bar continues moving instead of resetting
        }
    }

    public void StartQTE(Action onSuccess, Action onFail, Action onComplete)
    {
        this.onSuccess = onSuccess;
        this.onFail = onFail;
        this.onComplete = onComplete;

        isActive = true;

        if (timingBar != null)
            timingBar.gameObject.SetActive(true);

        if (safeZone != null)
        {
            safeZone.gameObject.SetActive(true);

            // Only set the safe zone randomly the first time
            if (!safeZoneSet)
            {
                float min = 0f;
                float max = 1f - safeZoneWidth;
                float randomStart = UnityEngine.Random.Range(min, max);
                safeZone.anchorMin = new Vector2(randomStart, safeZone.anchorMin.y);
                safeZone.anchorMax = new Vector2(randomStart + safeZoneWidth, safeZone.anchorMax.y);

                safeZoneSet = true; // Mark as set
            }
        }
    }

    private void EndQTE()
    {
        isActive = false;

        if (timingBar != null)
            timingBar.gameObject.SetActive(false);

        if (safeZone != null)
            safeZone.gameObject.SetActive(false);

        safeZoneSet = false; // Reset for next QTE

        onComplete?.Invoke();

        onSuccess = null;
        onFail = null;
        onComplete = null;
    }

}
