using System;
using UnityEngine;

public class CleanUpNPCWalker : MonoBehaviour
{
    public float speed = 2f;
    public Vector3 squareSize = new Vector3(5, 0, 5);
    public int penaltyAmount = 50;
    public float rotationSpeed = 5f; // Smooth rotation speed

    private Vector3 startPos;
    private int direction = 0; // 0=right, 1=up, 2=left, 3=down

    private Animator animator; // optional, in case you want to control walk animation

    void Start()
    {
        startPos = transform.position;

        // Find Animator in child model
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("Walking", true); // if your Animator has a Walking bool
        }
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position; // or startPos if you want runtime start
        Vector3 topRight = origin + new Vector3(squareSize.x, 0, squareSize.z);
        Vector3 topLeft = origin + new Vector3(0, 0, squareSize.z);
        Vector3 bottomRight = origin + new Vector3(squareSize.x, 0, 0);

        Gizmos.color = Color.green;

        // Draw square
        Gizmos.DrawLine(origin, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, origin);
    }


    void Update()
    {
        MoveInSquare();
    }

    void MoveInSquare()
    {
        // Determine target point based on current direction
        Vector3 target = startPos;

        if (direction == 0) target += new Vector3(squareSize.x, 0, 0);
        else if (direction == 1) target += new Vector3(squareSize.x, 0, squareSize.z);
        else if (direction == 2) target += new Vector3(0, 0, squareSize.z);
        else if (direction == 3) target += new Vector3(0, 0, 0);

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Smoothly rotate toward target
        Vector3 moveDirection = (target - transform.position).normalized;
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if reached target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            direction = (direction + 1) % 4;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerGrowth player = other.GetComponent<PlayerGrowth>();
        if (player != null)
        {
            player.ApplyPenalty(penaltyAmount); // Apply penalty

            // Show warning UI
            if (player.uiManager != null)
            {
                player.uiManager.ShowWarningMessage("-50 AVOID THE CIVILIAN!", Color.red, 0.5f, 1f);
            }

            // Play civilian eat sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                AudioManager.Instance.PlayCivilianEat();
                AudioManager.Instance.sfxSource.pitch = 1f; // reset pitch
            }


            Destroy(gameObject);                // Destroy NPC after collision
        }
    }
}
