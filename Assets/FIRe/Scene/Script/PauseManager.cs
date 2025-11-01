using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI")]
    public GameObject pauseMenu;

    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    public bool pauseTime = true;
    public bool pauseAudio = true;

    [Header("Scripts to Pause")]
    public MonoBehaviour[] scriptsToPause;

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
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

        foreach (var script in scriptsToPause)
        {
            if (script != null) script.enabled = false;
        }

        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("[PauseManager] Game paused, audio stopped");
    }

    public void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);

        if (pauseTime) Time.timeScale = 1f;

        if (pauseAudio) AudioListener.pause = false;

        foreach (var script in scriptsToPause)
        {
            if (script != null) script.enabled = true;
        }

        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("[PauseManager] Game resumed, audio resumed");
    }

    public void QuitToMenu(string menuSceneName)
    {
        // Reset time and audio before quitting
        if (pauseTime) Time.timeScale = 1f;
        if (pauseAudio) AudioListener.pause = false;

        Debug.Log("[PauseManager] Quitting to menu, audio resumed");

        SceneManager.LoadScene(menuSceneName);
    }
}
