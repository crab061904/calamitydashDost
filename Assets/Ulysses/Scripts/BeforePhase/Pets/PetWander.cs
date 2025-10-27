using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PetWander : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 50f;

    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f; // If not moving for 2 sec, consider stuck

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetNewDestination();
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
        }

        // ✅ STUCK DETECTION
        if (speed < 0.1f) // barely moving
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer > stuckThreshold)
        {
            Debug.Log($"{name} seems stuck! Forcing new destination.");
            ForceUnstuck();
            stuckTimer = 0f;
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius + transform.position;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void ForceUnstuck()
    {
        agent.ResetPath(); // Clear previous path

        // Try a very close new position first
        Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
        Vector3 newPos = transform.position + offset;

        if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);   // Snap to valid point
            SetNewDestination();        // Continue roaming
        }
        else
        {
            SetNewDestination(); // Fallback
        }
    }

    public void Init(float customRoamRadius = -1f)
    {
        if (customRoamRadius > 0f)
            roamRadius = customRoamRadius;

        SetNewDestination();
    }
}
