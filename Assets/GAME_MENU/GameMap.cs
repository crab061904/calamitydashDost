using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameMap : MonoBehaviour
{
    [Header("Loading Screen UI")]
    public GameObject loadingScreen;   // Assign your Loading Canvas
    public Slider loadingBar;          // Assign your Slider

    // --- Town A ---
    public void LoadTyphoonLevel()
    {
        Debug.Log("Town A Button Clicked");
        StartCoroutine(LoadSceneAsync("WITHMAPEvactuation_Typhoon 1"));
    }

    // --- Town B (Fire Scene) ---
    public void LoadFireLevel()
    {
        Debug.Log("Town B Button Clicked");
        StartCoroutine(LoadSceneAsync("finalfire"));
    }
    public void LoadReliefLevel()
    {
        Debug.Log("Town C Button Clicked");
        StartCoroutine(LoadSceneAsync("Joe_Rescuing"));
    }
    public void LoadCleanupLevel()
    {
        Debug.Log("Town D Button Clicked");
        StartCoroutine(LoadSceneAsync("CleanupPhase"));
    }


    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
