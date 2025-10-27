using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController_DURING : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 1000f;   // Thrust power
    public float turnTorque = 500f;   // Turning power

    private Rigidbody rb;

    [HideInInspector] public bool isPlayerDriving = true;
    [HideInInspector] public bool isWaitingForNPCs = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.isKinematic = false;
        rb.linearDamping = 1f;      // add drag for water resistance
        rb.angularDamping = 2f;
    }

    void FixedUpdate()
    {
        if (!isPlayerDriving || isWaitingForNPCs) return;

        float moveInput = Input.GetAxis("Vertical");   // W/S or ↑↓
        float turnInput = Input.GetAxis("Horizontal"); // A/D or ←→

        // Apply forward/backward thrust
        rb.AddForce(transform.forward * moveInput * moveForce * Time.fixedDeltaTime);

        // Apply turning torque
        rb.AddTorque(Vector3.up * turnInput * turnTorque * Time.fixedDeltaTime);
    }
}
