using UnityEngine;
using UnityEngine.SceneManagement;

public class DURING_PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;           // Pause panel
    public GameObject confirmationUI;        // Confirmation popup
    public TMPro.TextMeshProUGUI confirmationText;

    public GameObject resultsUI;             // Assign your Results Panel here

    private System.Action pendingAction;
    private bool isPaused = false;

    [HideInInspector] public bool canPause = true;

    private enum ConfirmationSource { PauseMenu, ResultsPanel }
    private ConfirmationSource currentSource;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        confirmationUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        confirmationUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ========== REQUEST FROM PAUSE MENU ==========

    public void RequestRestartFromPause()
    {
        currentSource = ConfirmationSource.PauseMenu;
        ShowConfirmation("Restart the level?", RestartLevel);
    }

    public void RequestMainMenuFromPause()
    {
        currentSource = ConfirmationSource.PauseMenu;
        ShowConfirmation("Exit to Main Menu?", ExitToMainMenu);
    }

    // ========== REQUEST FROM RESULTS PANEL ==========

    public void RequestRestartFromResults()
    {
        currentSource = ConfirmationSource.ResultsPanel;
        ShowConfirmation("Restart the level?", RestartLevel);
    }

    public void RequestMainMenuFromResults()
    {
        currentSource = ConfirmationSource.ResultsPanel;
        ShowConfirmation("Exit to Main Menu?", ExitToMainMenu);
    }

    // ========== CONFIRMATION SYSTEM ==========

    void ShowConfirmation(string message, System.Action action)
    {
        pauseMenuUI.SetActive(false);
        confirmationUI.SetActive(true);

        if (confirmationText != null)
            confirmationText.text = message;

        pendingAction = action;
    }

    public void ConfirmYes()
    {
        confirmationUI.SetActive(false);
        Time.timeScale = 1f;
        pendingAction?.Invoke();
    }

    public void ConfirmNo()
    {
        confirmationUI.SetActive(false);

        if (currentSource == ConfirmationSource.PauseMenu)
        {
            pauseMenuUI.SetActive(true);
        }
        // If from results panel → do nothing, it stays on results screen
    }

    public void GoToDuringPhase()
    {
        SceneManager.LoadScene("DURING PHASE");
    }


    // ========== FINAL ACTIONS ==========

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ExitToMainMenu()
    {
        // Stop background music if it exists in the scene
        During_BackgroundMusic bgMusic = FindObjectOfType<During_BackgroundMusic>();
        if (bgMusic != null)
        {
            bgMusic.StopMusic(); // optional
        }

        SceneManager.LoadScene("Game Map");
    }

}
