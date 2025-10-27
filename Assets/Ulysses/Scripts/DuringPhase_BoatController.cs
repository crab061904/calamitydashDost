using UnityEngine;
using UnityEngine.UI; // Needed for Slider

[AddComponentMenu("FusionWater/_Examples/BoatController")]
public class DuringPhase_BoatController : MonoBehaviour
{
    [Header("Boat Settings")]
    public float moveSpeed = 900f;
    public float turnSpeed = 50f; // increased for better responsiveness

    [Range(0.5f, 5)]
    public float accelerationTime = 3f;
    private float accTime = 0;

    [Header("References")]
    public Transform boatMotor;
    public GameObject rescueCanvas;

    private Vector3 startRotation;
    private Rigidbody rb;
    public bool isPlayerDriving = true;

    [Header("Engine Particles")]
    public ParticleSystem engineBurst;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    [Header("Fuel & Boost")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float boostMultiplier = 2f;
    public float fuelDrainRate = 10f;
    private bool isBoosting = false;

    [Header("UI")]
    public Slider fuelBar;

    [Header("Audio")]
    public AudioSource engineAudio;
    public float basePitch = 1f;
    public float boostPitch = 1.5f;
    public float idleVolume = 0.2f;
    public float moveVolume = 0.6f;

    [Header("Optional Speed Clamp")]
    public float maxSpeed = 20f;

    [Header("Movement Threshold")]
    public float minMoveSpeedForTurning = 0.5f; // Minimum speed required to turn

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startRotation = boatMotor.localEulerAngles;

        if (engineBurst != null)
        {
            mainModule = engineBurst.main;
            emissionModule = engineBurst.emission;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        if (rescueCanvas != null)
            rescueCanvas.SetActive(false);

        if (fuelBar != null)
        {
            fuelBar.maxValue = maxFuel;
            fuelBar.value = currentFuel;
        }

        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.playOnAwake = false;
            engineAudio.volume = 0f;
            engineAudio.Play();
        }
    }

    void FixedUpdate()
    {
        if (!isPlayerDriving) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // --- Acceleration logic (linear ramp) ---
        if (Mathf.Abs(moveInput) < 0.01f)
            accTime = 0;
        else
            accTime += Time.fixedDeltaTime;

        float accelerationFactor = Mathf.Clamp01(accTime / accelerationTime);

        // --- Boost logic ---
        float effectiveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && currentFuel > 0f)
        {
            isBoosting = true;
            effectiveSpeed *= boostMultiplier;
            currentFuel -= fuelDrainRate * Time.fixedDeltaTime;
            if (currentFuel < 0f) currentFuel = 0f;
        }
        else
        {
            isBoosting = false;
        }

        // --- Movement ---
        Vector3 customForward = Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.forward;
        Vector3 moveForce = customForward * moveInput * effectiveSpeed * accelerationFactor;
        rb.AddForce(moveForce, ForceMode.Force);

        // --- Clamp maximum speed ---
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // --- FIXED: Only allow turning when boat is moving ---
        bool isMoving = rb.linearVelocity.magnitude > minMoveSpeedForTurning;

        if (Mathf.Abs(turnInput) > 0.01f && isMoving)
        {
            float targetYaw = rb.rotation.eulerAngles.y + turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion targetRotation = Quaternion.Euler(rb.rotation.eulerAngles.x, targetYaw, rb.rotation.eulerAngles.z);

            // Smooth rotation
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 8f));
        }

        // --- Visual motor rotation (graphics only) ---
        if (boatMotor != null)
        {
            boatMotor.localEulerAngles = startRotation;
            // Only rotate motor visually if boat is moving
            if (isMoving)
            {
                boatMotor.Rotate(Vector3.up, turnInput * turnSpeed * 0.5f, Space.Self);
            }
        }

        // --- Update particles ---
        if (engineBurst != null)
        {
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                float boostFactor = isBoosting ? 1.5f : 1f;
                mainModule.startSpeed = Mathf.Lerp(1f, 5f, Mathf.Abs(moveInput)) * boostFactor;
                emissionModule.rateOverTime = Mathf.Lerp(10, 160, Mathf.Abs(moveInput)) * boostFactor;
                mainModule.startSize = Mathf.Lerp(0.2f, 0.6f, accelerationFactor) * boostFactor;
            }
            else
            {
                emissionModule.rateOverTime = 0;
            }
        }

        // --- Update fuel UI ---
        if (fuelBar != null)
            fuelBar.value = currentFuel;

        // --- Update engine audio ---
        if (engineAudio != null)
        {
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, moveVolume, 5f * Time.deltaTime);
                float targetPitch = isBoosting ? basePitch * 0.85f : basePitch + (Mathf.Abs(moveInput) * 0.2f);
                engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, 5f * Time.deltaTime);
            }
            else
            {
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, idleVolume, 5f * Time.deltaTime);
                engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, basePitch, 5f * Time.deltaTime);
            }
        }
    }

    public void AddFuel(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
        if (fuelBar != null)
            fuelBar.value = currentFuel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rescueCanvas != null)
                rescueCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rescueCanvas != null)
                rescueCanvas.SetActive(false);
        }
    }
}