using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Animation Settings")]
    [SerializeField] private float animationDampTime = 0.1f;

    private Vector3 moveDirection;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ✅ Prevent fast diagonal movement
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void HandleMovement()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // ✅ Move smoothly
            Vector3 targetPosition = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            // ------------------------------------
            // THE FIX: Enforce Y-axis rotation only
            // ------------------------------------

            // 1. Calculate the raw target rotation (this is the one that might have X/Z rotation)
            Quaternion rawTargetRotation = Quaternion.LookRotation(moveDirection);

            // 2. Extract the Euler angles, specifically the Y-rotation (Yaw)
            Vector3 eulerAngles = rawTargetRotation.eulerAngles;

            // 3. Create a new, clean rotation using ONLY the Y-rotation. 
            //    X and Z are explicitly set to 0 (no pitch or roll).
            Quaternion flatTargetRotation = Quaternion.Euler(0f, eulerAngles.y, 0f);

            // 4. Apply the smooth rotation using the flat quaternion
            rb.rotation = Quaternion.Slerp(rb.rotation, flatTargetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleAnimation()
    {
        float targetSpeed = 0f;

        if (moveDirection.magnitude >= 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            targetSpeed = isRunning ? 1f : 0.5f; // ✅ 0 = idle, 0.5 = walk, 1 = run
        }

        animator.SetFloat("Speed", targetSpeed, animationDampTime, Time.deltaTime);
    }
}
