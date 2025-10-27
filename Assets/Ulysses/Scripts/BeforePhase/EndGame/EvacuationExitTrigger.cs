using UnityEngine;

public class EvacuationExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bus"))
        {
            EvacuationManager manager = FindObjectOfType<EvacuationManager>();
            if (manager != null && manager.ReadyForEvacExit)
            {
                ShowResultsImmediately(manager);
            }
        }
    }

    private void ShowResultsImmediately(EvacuationManager manager)
    {
        Time.timeScale = 0f; // Full pause instantly
        ResultsPanel.Instance.ShowResults(manager.PendingScore, manager.GetTimer());
    }
}
