using UnityEngine;

public class HoseController : MonoBehaviour
{
    [Header("Water Particles")]
    [SerializeField] private ParticleSystem waterParticles;
    
    [Header("Water Audio")]
    [SerializeField] private AudioSource waterAudioSource;

    [Header("Extinguishing Settings")]
    [SerializeField] private float extinguishRate = 0.5f;

    [Header("Water Control")]
    [SerializeField] private float minLifetime = 1f;
    [SerializeField] private float maxLifetime = 15f;
    [SerializeField] private float scrollSensitivity = 1f;

    private ParticleSystem.MainModule mainModule;
    private float currentLifetime;

    // LayerMask to ignore irrelevant layers (like Car or CameraTrigger)
    private int layerMask;

    void Start()
    {
        if (waterParticles != null)
        {
            mainModule = waterParticles.main;
            currentLifetime = Mathf.Clamp(mainModule.startLifetime.constant, minLifetime, maxLifetime);
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(currentLifetime, currentLifetime);
        }

        // Example: ignore Car and CameraTrigger layers
        int carLayer = LayerMask.NameToLayer("Car");
        int triggerLayer = LayerMask.NameToLayer("CameraTrigger");
        layerMask = ~((1 << carLayer) | (1 << triggerLayer));
    }

    void Update()
    {
        // Adjust water spray length with scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentLifetime = Mathf.Clamp(currentLifetime + scroll * scrollSensitivity, minLifetime, maxLifetime);
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(currentLifetime, currentLifetime);
        }

        // Visualize ray in Scene view (optional)
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);

        if (Input.GetMouseButton(0))
        {
            if (!waterParticles.isPlaying)
            {
                waterParticles.Play();
                
                // Start water audio
                if (waterAudioSource != null && !waterAudioSource.isPlaying)
                {
                    waterAudioSource.Play();
                }
            }

            // Raycast from camera forward
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100f, layerMask))
            {
                // Try extinguish fire if hit
                if (hit.collider.TryGetComponent(out Fire fire))
                {
                    fire.TryExtinguish(extinguishRate * Time.deltaTime);
                }
            }
        }
        else
        {
            if (waterParticles.isPlaying)
            {
                waterParticles.Stop();
                
                // Stop water audio
                if (waterAudioSource != null && waterAudioSource.isPlaying)
                {
                    waterAudioSource.Stop();
                }
            }
        }
    }
}
