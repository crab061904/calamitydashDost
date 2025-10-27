using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenu; // Assign your pause panel in inspector

    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape; // Key to toggle pause
    public bool pauseTime = true;             // Freeze game time when paused
    public bool pauseAudio = true;            // Pause all audio when paused

    [Header("Scripts to Pause")]
    public MonoBehaviour[] scriptsToPause;    // Any player/enemy/movement scripts

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);

        if (pauseTime) Time.timeScale = 0f;
        if (pauseAudio) AudioListener.pause = true;

        // Disable all scripts
        foreach (var script in scriptsToPause)
        {
            if (script != null) script.enabled = false;
        }

        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);

        if (pauseTime) Time.timeScale = 1f;
        if (pauseAudio) AudioListener.pause = false;

        // Enable all scripts
        foreach (var script in scriptsToPause)
        {
            if (script != null) script.enabled = true;
        }

        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitToMenu(string menuSceneName)
    {
        // Reset time and audio before quitting
        if (pauseTime) Time.timeScale = 1f;
        if (pauseAudio) AudioListener.pause = false;

        SceneManager.LoadScene(menuSceneName);
    }
}
