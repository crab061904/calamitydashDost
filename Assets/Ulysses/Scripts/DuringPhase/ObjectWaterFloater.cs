using UnityEngine;

public class ObjectWaterFloater : MonoBehaviour
{
    [Header("Water Settings")]
    public WaterVolume waterVolume; // Changed to WaterVolume reference

    [Header("Floating Settings")]
    public float floatHeight = 2f;       // Distance above water surface
    public float bounceDamp = 0.05f;     // How quickly it settles

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Try to find water volume if not assigned
        if (waterVolume == null)
        {
            waterVolume = FindObjectOfType<WaterVolume>();
            if (waterVolume == null)
            {
                Debug.LogError("No WaterVolume found in scene!");
            }
        }
    }

    void FixedUpdate()
    {
        if (waterVolume == null) return;

        // Get water height from WaterVolume script
        float waterHeight = waterVolume.GetHeight(transform.position);

        // Difference between object height and water
        float difference = (waterHeight + floatHeight) - transform.position.y;

        // Apply buoyancy force
        Vector3 uplift = Vector3.up * difference * (1f - rb.linearVelocity.y * bounceDamp);
        rb.AddForce(uplift, ForceMode.Acceleration);
    }
}