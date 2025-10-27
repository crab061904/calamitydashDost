using UnityEngine;
using System.Collections;

public class DebrisSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] debrisPrefabs;       // array of debris prefabs
    [Tooltip("How many debris per second. 0.2 = 1 every 5 seconds")]
    public float spawnRate = 0.3f;           // less frequent
    public float debrisSpeed = 14f;          // speed of debris
    public float verticalVariation = 1.5f;   // random up/down movement
    public float lifetime = 8f;              // destroy after X seconds
    [Range(0f, 1f)] public float perfectHitChance = 0.3f; // chance debris targets boat perfectly
    public float maxOffset = 2f;             // max random offset from boat

    [Header("Spawn Area")]
    public Transform topRightCorner;         // used for X and Z position only
    public float minY = 3f;
    public float maxY = 15f;

    private Coroutine spawnCoroutine;
    private Transform boat;                  // reference to boat

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boat = other.transform;
            if (spawnCoroutine == null)
                spawnCoroutine = StartCoroutine(SpawnDebris());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            boat = null;
        }
    }

    private IEnumerator SpawnDebris()
    {
        while (true)
        {
            float spawnX = topRightCorner.position.x;
            float spawnY = Random.Range(minY, maxY);
            float z = topRightCorner.position.z;

            Vector3 spawnPos = new Vector3(spawnX, spawnY, z);

            // Pick a random prefab from the array
            GameObject selectedPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Length)];

            GameObject debris = Instantiate(selectedPrefab, spawnPos, Random.rotation);
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            rb.useGravity = false;

            if (rb != null && boat != null)
            {
                // Start with target position
                Vector3 targetPos = boat.position;

                // Apply random offset if not perfect hit
                if (Random.value > perfectHitChance)
                {
                    targetPos.x += Random.Range(-maxOffset, maxOffset);
                    targetPos.y += Random.Range(-maxOffset, maxOffset);
                }

                // Optional: clamp target Y to make sure it stays below spawn height
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

                // Compute direction toward target
                Vector3 direction = (targetPos - spawnPos).normalized;

                rb.linearVelocity = direction * debrisSpeed;
                rb.angularVelocity = Random.insideUnitSphere * 3f;
            }

            Destroy(debris, lifetime);

            yield return new WaitForSeconds(1f / spawnRate);
        }
    }
}
