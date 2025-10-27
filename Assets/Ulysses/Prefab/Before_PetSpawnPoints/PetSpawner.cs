using UnityEngine;

public class PetSpawner : MonoBehaviour
{
    [Header("Pet Location Prefabs")]
    public GameObject[] petPrefabs; // Your prefabs that already contain pets positioned in different locations

    private GameObject activePrefab;

    void Start()
    {
        SpawnRandomPetSet();
    }

    void SpawnRandomPetSet()
    {
        if (petPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No pet prefabs assigned in PetPrefabManager.");
            return;
        }

        // Pick one prefab randomly
        int index = Random.Range(0, petPrefabs.Length);
        GameObject prefab = petPrefabs[index];

        // ✅ Spawn at prefab's original position & rotation
        activePrefab = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);

        Debug.Log($"✅ Spawned pet prefab: {prefab.name} at {prefab.transform.position}");
    }
}
