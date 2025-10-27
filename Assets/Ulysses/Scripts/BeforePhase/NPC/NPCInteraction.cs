using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCInteraction : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Transform doorLeft;
    private Transform doorRight;
    private Animator animator;
    private Transform busTargetPoint;

    private EvacuationManager evacuationManager;
    private BusController busController;

    [Header("Settings")]
    [SerializeField] private float interactDistance = 10f;

    private bool isEvacuating = false;
    public bool IsEvacuating => isEvacuating;

    private float evacStartTime;
    private float maxEvacTime = 30f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 🔹 Find required references
        player = GameObject.FindWithTag("Player")?.transform;
        doorLeft = GameObject.FindWithTag("DoorLeft")?.transform;
        doorRight = GameObject.FindWithTag("DoorRight")?.transform;
        busController = GameObject.FindWithTag("Bus")?.GetComponent<BusController>();
        evacuationManager = FindObjectOfType<EvacuationManager>();

        // 🔹 Error reporting
        if (player == null) Debug.LogError($"{name}: No 'Player' found!");
        if (doorLeft == null) Debug.LogError($"{name}: No 'DoorLeft' found!");
        if (doorRight == null) Debug.LogError($"{name}: No 'DoorRight' found!");
        if (busController == null) Debug.LogError($"{name}: No 'BusController' found!");
        if (evacuationManager == null) Debug.LogError($"{name}: No 'EvacuationManager' found!");

        // ❌ Removed: agent.areaMask = 1 << 4;
        // This was forcing NPCs to sidewalk-only and breaking movement.
        // Let NPCScript handle normal NavMesh roaming.
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        if (!isEvacuating)
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (distanceToPlayer <= interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                StartEvacuate();
            }
        }
        else
        {
            if (Time.time - evacStartTime >= maxEvacTime)
            {
                Debug.Log($"{name}: Forced into bus after timeout.");
                EnterBus();
            }
        }
    }

    void StartEvacuate()
    {
        if (isEvacuating || agent == null || !agent.isOnNavMesh) return;

        if (animator != null)
            animator.SetBool("IsRunning", true);

        if (busController != null)
            busController.isWaitingForNPCs = true;

        isEvacuating = true;
        evacStartTime = Time.time;
        agent.speed = 4f;

        // ✅ Use default NavMesh area (Walkable)
        agent.areaMask = NavMesh.AllAreas; // allows walking anywhere on baked navmesh

        // ✅ Choose closest bus door
        if (doorLeft == null || doorRight == null)
        {
            Debug.LogError($"{name}: Missing bus doors!");
            return;
        }

        float distL = Vector3.Distance(transform.position, doorLeft.position);
        float distR = Vector3.Distance(transform.position, doorRight.position);
        busTargetPoint = (distL < distR) ? doorLeft : doorRight;

        // ✅ Safely move to the door
        if (agent.isOnNavMesh)
        {
            bool success = agent.SetDestination(busTargetPoint.position);
            Debug.Log($"{name}: Evacuating → moving to {busTargetPoint.name}, success={success}");
        }
    }


    void EnterBus()
    {
        if (evacuationManager != null)
            evacuationManager.AddEvacuatedNPC();

        if (agent != null)
            agent.enabled = false;

        if (busController != null)
            busController.isWaitingForNPCs = false;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEvacuating) return;

        if (other.CompareTag("DoorLeft") || other.CompareTag("DoorRight"))
        {
            Debug.Log($"{name}: Entering bus via {other.tag}");
            EnterBus();
        }
    }
}
