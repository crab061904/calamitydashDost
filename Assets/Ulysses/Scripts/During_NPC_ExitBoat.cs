using UnityEngine;
using UnityEngine.AI;

public class During_NPC_ExitBoat : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform safeZoneTarget;
    private bool isGoingToSafeZone = false;

    [Header("NPC Settings")]
    [SerializeField] private float safeZoneThreshold = 1.5f; // distance to consider "arrived"
    [SerializeField] private bool destroyOnArrival = true;   // toggle: destroy or disable NPC

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void GoToSafeZone(Transform target)
    {
        if (agent == null || target == null) return;

        safeZoneTarget = target;
        agent.enabled = true;
        agent.SetDestination(safeZoneTarget.position);

        if (animator != null)
        {
            animator.SetBool("isSeated", false);
            animator.SetBool("isRunning", true);
        }

        isGoingToSafeZone = true;
    }

    void Update()
    {
        if (isGoingToSafeZone && agent != null && agent.enabled && safeZoneTarget != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= safeZoneThreshold)
            {
                ReachSafeZone();
            }
        }
    }

    private void ReachSafeZone()
    {
        isGoingToSafeZone = false;

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
        }

        // ✅ Cleanup: destroy or disable NPC
        if (destroyOnArrival)
        {
            Destroy(gameObject); // completely remove NPC
        }
        else
        {
            gameObject.SetActive(false); // hide but keep in memory
        }
    }
}
