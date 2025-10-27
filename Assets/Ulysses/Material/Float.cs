using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Float : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigidBody;

    [Tooltip("Floater points under the boat/hull. Spread them out!")]
    public Transform[] floaters;

    [Header("Buoyancy Settings")]
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;

    [Header("Water Damping")]
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void Awake()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        floaterCount = floaters.Length;
    }

    private void FixedUpdate()
    {
        foreach (Transform floater in floaters)
        {
            // get water height at this floater�s (x, z)
            float waterHeight = WaveManager.instance.GetWaveHeight(floater.position.x, floater.position.z);

            if (floater.position.y < waterHeight)
            {
                // how submerged is this floater
                float displacementMultiplier =
                    Mathf.Clamp01((waterHeight - floater.position.y) / depthBeforeSubmerged) * displacementAmount;

                // buoyancy force
                rigidBody.AddForceAtPosition(
                    new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f),
                    floater.position,
                    ForceMode.Acceleration
                );

                // add water drag
                rigidBody.AddForce(displacementMultiplier * -rigidBody.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }
}
