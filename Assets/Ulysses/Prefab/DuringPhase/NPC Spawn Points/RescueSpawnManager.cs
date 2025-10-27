using UnityEngine;

public class RescueSpawnManager : MonoBehaviour
{
    [Header("Rescue Prefabs (NPCs/Pets in different locations)")]
    public GameObject[] rescuePrefabs; // Assign multiple layout prefabs here

    private GameObject activeRescuePrefab;

    void Start()
    {
        SpawnRandomRescueLayout();
    }

    void SpawnRandomRescueLayout()
    {
        if (rescuePrefabs == null || rescuePrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No rescue prefabs assigned in RescueSpawnManager.");
            return;
        }

        // Randomly pick one rescue layout prefab
        int index = Random.Range(0, rescuePrefabs.Length);

        // Instantiate as a child of this manager to preserve prefab's local transform
        activeRescuePrefab = Instantiate(rescuePrefabs[index], transform);

        Debug.Log($"✅ Spawned Rescue Layout: {rescuePrefabs[index].name}");
    }
}
