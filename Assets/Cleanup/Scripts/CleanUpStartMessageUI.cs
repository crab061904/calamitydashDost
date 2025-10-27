using UnityEngine;
using TMPro;

public class CleanUpStartMessageUI : MonoBehaviour
{
    public float displayTime = 3f;   // How long the message stays fully visible
    public float fadeDuration = 2f;  // How long it takes to fade out

    private CanvasGroup canvasGroup;
    private float timer = 0f;
    private bool fading = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f; // fully visible at start
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!fading && timer >= displayTime)
        {
            fading = true;
            timer = 0f; // reset timer for fade duration
        }

        if (fading)
        {
            float t = timer / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            if (t >= 1f)
            {
                // fully faded, destroy it
                Destroy(gameObject);
            }
        }
    }
}
