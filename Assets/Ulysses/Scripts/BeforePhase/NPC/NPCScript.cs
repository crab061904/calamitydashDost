using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 20f;

    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f; // If not moving for 2 sec, consider stuck

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Snap NPC to nearest NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);

            // ✅ Restrict to Sidewalk area only
            int sidewalkArea = NavMesh.GetAreaFromName("Sidewalk"); // safer than assuming index 4
            if (sidewalkArea < 0)
            {
                Debug.LogError("NavMesh area 'Sidewalk' not found! Check Navigation → Areas tab.");
                return;
            }

            agent.areaMask = 1 << sidewalkArea; // only allow movement on Sidewalk

            SetNewDestination();
        }
        else
        {
            Debug.LogError($"{name} could not find any nearby NavMesh!");
            enabled = false;
        }
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // ✅ Move to next destination when reached
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            SetNewDestination();

        // ✅ Detect stuck NPCs
        if (speed < 0.1f)
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
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
        }
    }

    void ForceUnstuck()
    {
        agent.ResetPath();

        Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
        Vector3 newPos = transform.position + offset;

        if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 5f, agent.areaMask))
        {
            agent.Warp(hit.position);
            SetNewDestination();
        }
        else
        {
            SetNewDestination();
        }
    }

    public void Init(float customRoamRadius = -1f)
    {
        if (customRoamRadius > 0f)
            roamRadius = customRoamRadius;

        SetNewDestination();
    }
}
