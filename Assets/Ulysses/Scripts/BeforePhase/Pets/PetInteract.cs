using UnityEngine;

public class PetInteract : MonoBehaviour
{
    private Canvas promptCanvas;

    void Awake()
    {
        promptCanvas = GetComponentInChildren<Canvas>();
        if (promptCanvas != null)
            promptCanvas.enabled = false;
    }

    public void EnablePrompt()
    {
        if (promptCanvas != null)
            promptCanvas.enabled = true;
    }

    public void DisablePrompt()
    {
        if (promptCanvas != null)
            promptCanvas.enabled = false;
    }

    // Optional: If you want to check if the pet can always interact, just return true
    public bool CanInteract()
    {
        return true;
    }
}
