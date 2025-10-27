using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Joe_PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Animation Settings")]
    public float animationDampTime = 0.1f;

    [Header("Carry Settings")]
    public GameObject[] carryObjects; // Assign 3 box objects in Inspector
    private int carryCount = 0;

    [Header("References")]
    public GameManager gameManager; // Drag your GameManager here
    public CharacterController controller;
    public Animator animator;

    [Header("Turn Settings")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    // Carry state
    public bool isCarrying { get; private set; } = false;

    private Vector3 moveDirection;

    private void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();

        // Ensure all carry objects are disabled at start
        foreach (GameObject obj in carryObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void HandleMovement()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
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
        animator.SetBool("isCarrying", isCarrying);
    }

    // Pick up box
    public void AddCarry()
    {
        if (carryCount < carryObjects.Length)
        {
            carryObjects[carryCount].SetActive(true);
            carryCount++;
            UpdateCarryState();

            if (gameManager != null)
            {
                gameManager.boxesHeld = carryCount;
                gameManager.UpdateUI();
            }
        }
        else
        {
            Debug.Log("Already carrying max boxes!");
        }
    }

    // Drop/deliver box
    public void RemoveCarry()
    {
        if (carryCount > 0)
        {
            carryCount--;
            carryObjects[carryCount].SetActive(false);
            UpdateCarryState();

            if (gameManager != null)
            {
                gameManager.boxesHeld = carryCount;
                gameManager.UpdateUI();
            }
        }
        else
        {
            Debug.Log("No boxes to drop!");
        }
    }

    private void UpdateCarryState()
    {
        isCarrying = carryCount > 0;
        Debug.Log("Player is now " + (isCarrying ? "carrying boxes." : "not carrying boxes."));
    }
}
