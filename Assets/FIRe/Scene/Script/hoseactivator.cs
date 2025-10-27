using UnityEngine;

public class HoseActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HoseController hoseController;
    [SerializeField] private GameObject player;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool isPlayerNear = false;

    void Start()
    {
        if (hoseController != null)
            hoseController.enabled = false; // Hose ability starts off
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactKey))
        {
            // Toggle hose ON/OFF
            if (hoseController != null)
                hoseController.enabled = !hoseController.enabled;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNear = false;
            // Optional: hoseController.enabled = false; // uncomment if you want it to auto-turn off when walking away
        }
    }
}
