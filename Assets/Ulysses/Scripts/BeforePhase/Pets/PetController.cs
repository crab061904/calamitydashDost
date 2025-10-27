using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PetController : MonoBehaviour
{
    [Header("Behavior Settings")]
    public bool startsWandering = false; // ✅ toggle for prefab setup

    private Animator animator;
    private NavMeshAgent agent;
    private Transform followTarget;

    private bool isRescued = false;
    private TimingQTE qteSystem;
    private Animator playerAnimator;
    private PlayerMovement playerController;
    private PetInteract petInteract;
    private PetWander wanderScript;

    [Header("Agent Settings")]
    public float wanderSpeed = 5f;
    public float wanderAngularSpeed = 100f;
    public float wanderAcceleration = 4f;

    public float followSpeed = 12f;
    public float followAngularSpeed = 200f;
    public float followAcceleration = 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        qteSystem = FindObjectOfType<TimingQTE>();
        petInteract = GetComponent<PetInteract>();
        wanderScript = GetComponent<PetWander>();
    }

    private void Start()
    {
        if (wanderScript != null)
            wanderScript.enabled = startsWandering;

        if (animator != null)
        {
            if (startsWandering)
            {
                // 🐾 wandering pets stand up
                animator.SetBool("isSeated", false);
            }
            else
            {
                // 🐾 seated pets stay seated until rescued
                animator.SetBool("isSeated", true);
            }
        }
    }

    // ✅ Called when the player interacts
    public void StartRescueQTEFromPlayer(PetRaycastInteract playerRaycast)
    {
        if (!CanInteract() || isRescued || qteSystem == null) return;

        playerController = playerRaycast.GetComponent<PlayerMovement>();
        playerAnimator = playerRaycast.GetComponent<Animator>();

        StopPetMovement();
        petInteract?.DisablePrompt();

        if (playerAnimator != null)
            playerAnimator.SetBool("isPetting", true);

        if (playerController != null)
            playerController.enabled = false;

        // Start the QTE sequence
        qteSystem.StartQTE(
            onSuccess: () =>
            {
                FollowPlayer(playerController.transform);
                StartCoroutine(LookAtPetCoroutine(playerController.transform, 0.5f));
            },
            onFail: () =>
            {
                SpawnMissEffect();
            },
            onComplete: () =>
            {
                if (playerAnimator != null)
                    playerAnimator.SetBool("isPetting", false);

                if (playerController != null)
                    playerController.enabled = true;
            }
        );
    }

    // ✅ Pet can always be interacted with unless already rescued
    public bool CanInteract()
    {
        return !isRescued;
    }

    private IEnumerator LookAtPetCoroutine(Transform player, float duration)
    {
        Vector3 dir = transform.position - player.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) yield break;

        Quaternion start = player.rotation;
        Quaternion target = Quaternion.LookRotation(dir);

        float t = 0f;
        while (t < duration)
        {
            player.rotation = Quaternion.Slerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        player.rotation = target;
    }

    public bool IsFollowingPlayer()
    {
        return isRescued && followTarget != null;
    }

    private void FollowPlayer(Transform player)
    {
        if (isRescued) return;

        isRescued = true;
        followTarget = player;

        if (wanderScript != null)
            wanderScript.enabled = false;

        SetAgentToFollow();

        if (animator != null)
        {
            animator.SetBool("isSeated", false);
            animator.SetTrigger("isHappy");
        }
    }

    private void Update()
    {
        if (isRescued && followTarget != null && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(followTarget.position);

            if (animator != null)
            {
                float normalizedSpeed = agent.velocity.magnitude / agent.speed;
                animator.SetFloat("Speed", normalizedSpeed);
            }
        }
        else if (wanderScript != null && wanderScript.enabled)
        {
            if (animator != null && agent != null)
                animator.SetFloat("Speed", agent.velocity.magnitude / wanderSpeed);
        }
    }

    private void SetAgentToFollow()
    {
        if (agent != null)
        {
            agent.speed = followSpeed;
            agent.angularSpeed = followAngularSpeed;
            agent.acceleration = followAcceleration;
            agent.isStopped = false;
        }
    }

    private void SetAgentToWander()
    {
        if (agent != null)
        {
            agent.speed = wanderSpeed;
            agent.angularSpeed = wanderAngularSpeed;
            agent.acceleration = wanderAcceleration;
            agent.isStopped = false;
        }
    }

    private void StopPetMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (wanderScript != null)
            wanderScript.enabled = false;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("isSeated", true);
        }
    }

    private void SpawnMissEffect()
    {
        Debug.Log("💥 QTE missed!");
    }
}
