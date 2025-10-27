using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonRPGMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float cameraPitchLimit = 80f;

    [Header("Animation")]
    public Animator animator; // Assign your Animator here

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");  // A/D
        float vertical = Input.GetAxis("Vertical");      // W/S

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        bool isMoving = move.magnitude > 0.1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        // --- Gravity Fix ---
        if (controller.isGrounded)
        {
            // Small downward force to keep grounded (prevents sinking)
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            // Jump
            if (Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        // --- Animation ---
        if (animator != null)
        {
            if (isMoving)
            {
                // Blend between walk/run
                float animSpeed = isRunning ? 1f : 0.5f;
                animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            }
        }
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Horizontal rotation (rotate player)
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (camera look up/down)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraPitchLimit, cameraPitchLimit);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
