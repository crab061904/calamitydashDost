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

        // Freeze ALL rotations and use only manual rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Also ensure we're not using physics for rotation
        rb.freezeRotation = true;
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);
        if (input.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = input.normalized;
    }

    private void HandleMovement()
    {
        if (moveDirection != Vector3.zero)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // Move smoothly
            Vector3 targetPosition = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            // Rotate only when we have valid movement - use transform.rotation instead of rb.rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        // When moveDirection is zero, no rotation occurs
    }

    private void HandleAnimation()
    {
        float targetSpeed = 0f;

        if (moveDirection.magnitude >= 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            targetSpeed = isRunning ? 1f : 0.5f;
        }

        animator.SetFloat("Speed", targetSpeed, animationDampTime, Time.deltaTime);
    }
}