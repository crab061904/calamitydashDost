using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
public class CleanUpPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float boostMultiplier = 2f;
    public float rotationSpeed = 10f; // 🔄 how fast the player rotates

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private PlayerGrowth growth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        growth = GetComponent<PlayerGrowth>();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            moveInput.x = (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0);
            moveInput.y = (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0);
            isBoosting = Keyboard.current.leftShiftKey.isPressed;
        }

        // Handle boost draining over time
        if (isBoosting && growth != null && growth.score > 0)
        {
            boostTimer += Time.deltaTime;
            if (boostTimer >= 1f)
            {
                int drain = GetBoostDrain(growth.playerLevel);
                growth.score = Mathf.Max(0, growth.score - drain);
                growth.uiManager?.UpdateScore(growth.score, growth.playerLevel);
                Debug.Log($"Boost drain: -{drain}, Score={growth.score}");

                int prevThresholdIndex = Mathf.Max(0, growth.playerLevel - 2);
                if (growth.playerLevel > 1 && growth.score < growth.GrowthThresholds[prevThresholdIndex])
                    growth.Shrink();

                boostTimer = 0f;
            }
        }
        else
        {
            boostTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        float speed = isBoosting ? moveSpeed * boostMultiplier : moveSpeed;
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized * speed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // 🔄 Rotate player to face movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    int GetBoostDrain(int level)
    {
        if (level == 1) return 1;
        if (level == 2 || level == 3) return 2;
        return 3;
    }
}
