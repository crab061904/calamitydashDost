using UnityEngine;
using System.Collections;

public class WindZone : MonoBehaviour
{
    public GameObject debrisPrefab;
    public Transform spawnPoint; // empty GameObject above player
    public float spawnRate = 0.5f; // less frequent debris
    public float debrisSpeed = 8f;
    public float destroyDistance = 20f; // how far past player debris gets destroyed

    private bool boatInside = false;
    private Transform boat;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInside = true;
            boat = other.transform;
            StartCoroutine(SpawnDebris());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInside = false;
        }
    }

    private IEnumerator SpawnDebris()
    {
        while (boatInside)
        {
            // Spawn at the defined spawn point, slightly randomized in X/Z
            Vector3 spawnPos = spawnPoint.position + new Vector3(
                Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)
            );

            GameObject debris = Instantiate(debrisPrefab, spawnPos, Random.rotation);
            Rigidbody rb = debris.GetComponent<Rigidbody>();

            // Direction from spawn point to boat
            Vector3 direction = (boat.position - spawnPos).normalized;

            // Apply velocity directly toward player
            rb.linearVelocity = direction * debrisSpeed;

            // Optional tumbling
            rb.angularVelocity = Random.insideUnitSphere * 5f;

            // Destroy debris after it passes beyond the player
            StartCoroutine(DestroyAfterDistance(debris, spawnPos, direction));

            yield return new WaitForSeconds(1f / spawnRate);
        }
    }

    private IEnumerator DestroyAfterDistance(GameObject debris, Vector3 spawnPos, Vector3 direction)
    {
        while (debris != null)
        {
            if (Vector3.Distance(spawnPos, debris.transform.position) > destroyDistance)
            {
                Destroy(debris);
                yield break;
            }
            yield return null;
        }
    }
}
