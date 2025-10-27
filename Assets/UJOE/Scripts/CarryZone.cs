using UnityEngine;

public class CarryZone : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Joe_PlayerMovement player = other.GetComponentInParent<Joe_PlayerMovement>();
        if (player != null)
        {
            // Player picks up a box
            player.AddCarry(); // visually add to player hands

            if (gameManager != null)
            {
                gameManager.AddBox(); // update boxesHeld + score
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // optional: do nothing for pickup
    }
}
