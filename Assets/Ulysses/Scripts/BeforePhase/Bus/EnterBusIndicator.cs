using UnityEngine;

public class NPCEnteringBusIndicator : MonoBehaviour
{
    public Canvas canvasEnter;           // Canvas for "Enter Bus" indicator
    public Canvas canvasWaiting;         // Canvas for "Waiting for NPCs" indicator

    public Transform player;             // Player transform for distance check
    public float showDistance = 6f;     // Distance at which canvas shows
    public BusController busController;
    void Start()
    {
        if (canvasEnter != null)
            canvasEnter.enabled = false;
        if (canvasWaiting != null)
            canvasWaiting.enabled = false;
    }

    void Update()
    {
        if (canvasEnter == null || canvasWaiting == null || player == null || busController == null)
            return;

        // Reset both canvases
        canvasEnter.enabled = false;
        canvasWaiting.enabled = false;

        float distance = Vector3.Distance(player.position, busController.transform.position);

        // Show waiting canvas if bus is waiting for NPCs
        if (busController.isWaitingForNPCs)
        {
            canvasWaiting.enabled = true;
            return; // No need to check enter canvas
        }

        // Show enter canvas only if player is near and not driving
        if (!busController.isPlayerDriving && distance <= showDistance)
        {
            canvasEnter.enabled = true;
        }
    }

}
