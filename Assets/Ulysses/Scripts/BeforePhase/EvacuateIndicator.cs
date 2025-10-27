using UnityEngine;

public class EvacuateIndicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float showDistance = 5f;   // Distance at which the canvas shows

    private Transform player;
    private Canvas canvas;
    private NPCInteraction npcInteraction;

    void Start()
    {
        // Cache canvas
        canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.enabled = false; // Hide by default

        // Find NPC interaction on parent
        npcInteraction = GetComponentInParent<NPCInteraction>();

        // Find player automatically by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("EvacuateIndicator: No object with tag 'Player' found in scene!");
    }

    void Update()
    {
        if (player == null || npcInteraction == null)
            return;

        // Hide indicator if NPC is already evacuating
        if (npcInteraction.IsEvacuating)
        {
            if (canvas != null)
                canvas.enabled = false;
            return;
        }

        // Show or hide based on player distance
        float distance = Vector3.Distance(player.position, transform.position);
        if (canvas != null)
            canvas.enabled = distance <= showDistance;
    }
}
