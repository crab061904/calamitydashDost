using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BusController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxMoveSpeed = 10f;
    public float accelerationRate = 10f;
    public float decelerationRate = 16f;
    public float turnSpeed = 50f;

    private float currentSpeed = 0f;
    private Rigidbody rb;

    [HideInInspector] public bool isPlayerDriving = false;
    [HideInInspector] public bool isWaitingForNPCs = false;

    [Header("Boost Settings")]
    public float boostMultiplier = 1.4f;
    public float boostAcceleration = 20f;

    [Header("Collision Penalty UI")]
    public TextMeshProUGUI penaltyText;
    private Coroutine penaltyRoutine;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    // 🔊 Audio
    [Header("Engine Audio")]
    public AudioSource engineAudio;
    public float idleVolume = 0.05f;  // softer idle
    public float moveVolume = 0.2f;  // softer when moving

    public float basePitch = 1f;
    public float boostPitch = 1f;

    private float frontLeftRotationX = 0f;
    private float frontRightRotationX = 0f;
    private float rearLeftRotationX = 0f;
    private float rearRightRotationX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        penaltyText.gameObject.SetActive(false);

        // setup engine sound
        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.playOnAwake = false;
            engineAudio.volume = 0f;
            engineAudio.Play();
        }
    }

    void Update()
    {
        if (!isPlayerDriving || isWaitingForNPCs)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.isKinematic = true;
            currentSpeed = 0f;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = false;
        }

        // 🔊 Lower engine volume when not driving
        if (engineAudio != null)
        {
            if (!isPlayerDriving || isWaitingForNPCs)
            {
                // Fade to very low volume (bus parked)
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, 0.05f, 3f * Time.deltaTime);
                engineAudio.pitch = basePitch; // keep pitch constant
            }
        }

    }

    void FixedUpdate()
    {
        if (!isPlayerDriving || isWaitingForNPCs) return;
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        float effectiveMaxSpeed = isBoosting ? maxMoveSpeed * boostMultiplier : maxMoveSpeed;
        float effectiveAcceleration = isBoosting ? accelerationRate * 1.5f : accelerationRate;

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            float targetSpeed = moveInput * effectiveMaxSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, effectiveAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, decelerationRate * Time.fixedDeltaTime);
        }

        Vector3 move = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        float rotationMultiplier = Mathf.Abs(currentSpeed) / effectiveMaxSpeed;
        float turn = turnInput
                     * Mathf.Sign(currentSpeed)
                     * rotationMultiplier
                     * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        RotateWheelMeshes(currentSpeed, turnInput);

        // 🔊 Update engine audio
        // 🔊 Update engine audio (no pitch shifting)
        if (engineAudio != null)
        {
            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                // Louder when moving
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, moveVolume, 5f * Time.deltaTime);
            }
            else
            {
                // Quieter when idle
                engineAudio.volume = Mathf.Lerp(engineAudio.volume, idleVolume, 5f * Time.deltaTime);
            }

            // Keep pitch constant
            engineAudio.pitch = basePitch;
        }

    }

    void RotateWheelMeshes(float currentMoveSpeed, float turnInput)
    {
        float wheelRotationSpeed = 360f;
        float steerAngle = turnInput * 30f;

        if (Mathf.Abs(currentMoveSpeed) > 0.01f)
        {
            float rollAmount = Mathf.Sign(currentMoveSpeed) * wheelRotationSpeed * Time.fixedDeltaTime;
            frontLeftRotationX += rollAmount;
            frontRightRotationX += rollAmount;
            rearLeftRotationX += rollAmount;
            rearRightRotationX += rollAmount;
        }

        if (frontLeftMesh)
            frontLeftMesh.localRotation = Quaternion.Euler(frontLeftRotationX, steerAngle, 0f);
        if (frontRightMesh)
            frontRightMesh.localRotation = Quaternion.Euler(frontRightRotationX, steerAngle, 0f);
        if (rearLeftMesh)
            rearLeftMesh.localRotation = Quaternion.Euler(rearLeftRotationX, 0f, 0f);
        if (rearRightMesh)
            rearRightMesh.localRotation = Quaternion.Euler(rearRightRotationX, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        EvacuationManager evac = FindObjectOfType<EvacuationManager>();
        if (evac == null) return;

        if (collision.gameObject.CompareTag("Car"))
        {
            evac.DeductScore(10);
            ShowWorldPenalty("Hit Car -10");
        }
        else if (collision.gameObject.CompareTag("Building"))
        {
            evac.DeductScore(15);
            ShowWorldPenalty("Crashed Building -15");
        }
        else if (collision.gameObject.CompareTag("Pet"))
        {
            evac.DeductScore(30);
            ShowWorldPenalty("Pet Injured! -30");
        }
        else if (collision.gameObject.CompareTag("NPC_Before"))
        {
            evac.DeductScore(50);
            ShowWorldPenalty("Civilian Hit! -50");
        }
    }

    private void ShowWorldPenalty(string message)
    {
        if (penaltyText == null) return;

        if (penaltyRoutine != null) StopCoroutine(penaltyRoutine);
        penaltyRoutine = StartCoroutine(PenaltyWorldRoutine(message));
    }

    private IEnumerator PenaltyWorldRoutine(string message)
    {
        penaltyText.text = message;
        penaltyText.gameObject.SetActive(true);

        if (Camera.main != null)
        {
            penaltyText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        yield return new WaitForSeconds(1.5f);
        penaltyText.gameObject.SetActive(false);
    }
}
