using UnityEngine;

public class HoseController : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterParticles;

    [Header("Extinguishing Settings")]
    [SerializeField] private float extinguishRate = 0.5f; // constant extinguish power

    [Header("Water Control")]
    [SerializeField] private float minLifetime = 1f;
    [SerializeField] private float maxLifetime = 15f;
    [SerializeField] private float scrollSensitivity = 1f;

    private ParticleSystem.MainModule mainModule;
    private float currentLifetime;

    // LayerMask to ignore the Car + CameraTrigger layers
    private int layerMask;

    void Start()
    {
        if (waterParticles != null)
        {
            mainModule = waterParticles.main;
            currentLifetime = Mathf.Clamp(mainModule.startLifetime.constant, minLifetime, maxLifetime);
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(currentLifetime, currentLifetime);
        }

        // ✅ Ignore both "Car" and "CameraTrigger" layers
        int carLayer = LayerMask.NameToLayer("Car");
        int triggerLayer = LayerMask.NameToLayer("CameraTrigger");
        layerMask = ~((1 << carLayer) | (1 << triggerLayer));
    }

    void Update()
    {
        // Scroll to adjust water spray length
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentLifetime = Mathf.Clamp(currentLifetime + scroll * scrollSensitivity, minLifetime, maxLifetime);
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(currentLifetime, currentLifetime);
        }

        // Visualize the ray in Scene view
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

        // Left mouse click to spray
        if (Input.GetMouseButton(0))
        {
            if (!waterParticles.isPlaying)
                waterParticles.Play();

            // ✅ Raycast ignoring Car + CameraTrigger layers
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100f, layerMask))
            {
                Debug.Log("Ray hit: " + hit.collider.name);

                if (hit.collider.TryGetComponent(out Fire fire))
                {
                    fire.TryExtinguish(extinguishRate * Time.deltaTime);
                }
            }
        }
        else
        {
            if (waterParticles.isPlaying)
                waterParticles.Stop();
        }
    }
}
