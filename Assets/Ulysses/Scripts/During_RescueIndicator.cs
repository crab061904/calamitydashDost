using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class During_RescueIndicator : MonoBehaviour
{
    private Canvas canvas;
    private During_NPC_Seat npcInteraction;

    private void Awake()
    {
        // Cache the child Canvas
        canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.enabled = false; // hide by default

        // Get parent NPC interaction
        npcInteraction = GetComponentInParent<During_NPC_Seat>();
        if (npcInteraction == null)
            Debug.LogError("During_RescueIndicator: No During_NPC_Seat found in parent!");

        // Setup trigger collider
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        // Optional: set radius in Inspector
    }

    private void OnTriggerEnter(Collider other)
    {
        if (npcInteraction == null || npcInteraction.IsRescued)
            return;

        if (other.CompareTag("Boat") && canvas != null)
            canvas.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat") && canvas != null)
            canvas.enabled = false;
    }

    private void Update()
    {
        // If rescued, permanently hide and disable trigger
        if (npcInteraction != null && npcInteraction.IsRescued)
        {
            if (canvas != null)
                canvas.enabled = false;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
    }
}
