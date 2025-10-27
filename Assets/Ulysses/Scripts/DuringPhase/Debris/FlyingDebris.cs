using UnityEngine;

public class FlyingDebris : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            // Apply damage or knockback here
            Destroy(gameObject);
        }
    }
}
