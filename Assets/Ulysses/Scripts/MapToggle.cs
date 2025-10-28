using UnityEngine;

public class MapToggle : MonoBehaviour
{
    [Header("Map UI References")]
    [Tooltip("The small minimap UI (shown during gameplay).")]
    public GameObject miniMapUI;

    [Tooltip("The fullscreen map UI (hidden at start).")]
    public GameObject fullMapUI;

    private bool isFullMapOpen = false;

    void Start()
    {
        // Make sure full map starts hidden
        if (fullMapUI != null)
            fullMapUI.SetActive(false);

        // Minimap should be visible at the start
        if (miniMapUI != null)
            miniMapUI.SetActive(true);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        isFullMapOpen = !isFullMapOpen;

        if (miniMapUI != null)
            miniMapUI.SetActive(!isFullMapOpen);

        if (fullMapUI != null)
            fullMapUI.SetActive(isFullMapOpen);
    }
}
