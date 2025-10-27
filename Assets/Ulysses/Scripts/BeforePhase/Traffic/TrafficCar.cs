using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    [Header("Path Settings")]
    public WayPointPath path;   // Assign in inspector
    private Transform[] waypoints;
    private int currentIndex = 0;

    [Header("Movement Settings")]
    public float minSpeed = 3f;   // Minimum random speed
    public float maxSpeed = 8f;   // Maximum random speed
    private float speed;          // Actual speed for this car

    [Header("Obstacle Detection")]
    public float stopDistance = 8f;     // How far ahead to check
    public float detectRadius = 1.2f;   // Width of detection (like bumper)
    public LayerMask obstacleLayers;    // Define layers (e.g., "Car", optional "Player")

    void Start()
    {
        if (path != null)
            waypoints = path.GetPoints();

        // Randomize speed per car
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Draw debug ray every frame
        Debug.DrawRay(transform.position + Vector3.up * 3.5f, transform.forward * stopDistance, Color.red);

        // Only move forward if no obstacle is ahead
        if (!IsObstacleAhead())
        {
            MoveForward();
        }

        // Waypoint check should happen regardless of movement
        CheckWaypointProgress();
    }

    private void MoveForward()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void CheckWaypointProgress()
    {
        if (currentIndex >= waypoints.Length - 1)
        {
            // Close to last waypoint → destroy car
            float distanceToEnd = Vector3.Distance(transform.position, waypoints[waypoints.Length - 1].position);
            if (distanceToEnd < 2f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Progress to next waypoint if close
            if (Vector3.Distance(transform.position, waypoints[currentIndex].position) < 2f)
            {
                currentIndex++;
                // Snap forward to next waypoint’s direction
                Vector3 dir = (waypoints[currentIndex].position - transform.position).normalized;
                transform.forward = dir;
            }
        }
    }

    private bool IsObstacleAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // 1️⃣ SphereCast ahead (normal bumper check)
        if (Physics.SphereCast(origin, detectRadius, transform.forward, out RaycastHit hit, stopDistance, obstacleLayers))
        {
            if (hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        // 2️⃣ Extra safety: OverlapSphere directly in front (to catch close objects)
        Vector3 closeCheckPos = origin + transform.forward * 1.5f; // right in front of bumper
        Collider[] closeHits = Physics.OverlapSphere(closeCheckPos, detectRadius, obstacleLayers);
        foreach (var c in closeHits)
        {
            if (c.gameObject != gameObject)
            {
                Debug.Log($"{name} too close to {c.name} - full stop");
                return true;
            }
        }

        return false;
    }



    private void OnDrawGizmosSelected()
    {
        // Visualize detection
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, origin + transform.forward * stopDistance);
        Gizmos.DrawWireSphere(origin + transform.forward * stopDistance, detectRadius);
    }
}
