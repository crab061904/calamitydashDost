using UnityEngine;
using TMPro;

public class UIPrompt : MonoBehaviour
{
    public static UIPrompt Instance;

    [SerializeField] private TextMeshProUGUI promptText;

    void Awake()
    {
        // Ensure only one instance exists and it's set before other scripts call it
        Instance = this;

        // Hide prompt initially, but don't disable GameObject (so Awake runs)
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    public void ShowPrompt(string message)
    {
        if (promptText == null) return;
        promptText.text = message;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }
}
