using UnityEngine;

public class SpeedBoostLayout : MonoBehaviour
{
    [Header("Speed Boost Prefabs")]
    [Tooltip("Different speed boost layouts as prefabs")]
    public GameObject[] speedBoostPrefabs;

    private GameObject activeBoostPrefab;

    void Start()
    {
        SpawnRandomBoostLayout();
    }

    void SpawnRandomBoostLayout()
    {
        if (speedBoostPrefabs == null || speedBoostPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No Speed Boost prefabs assigned in SpeedBoostManager.");
            return;
        }

        // Pick one prefab randomly
        int index = Random.Range(0, speedBoostPrefabs.Length);

        // ✅ Instantiate the prefab
        activeBoostPrefab = Instantiate(speedBoostPrefabs[index]);

        // ✅ Parent it to the manager while keeping prefab’s local transform
        activeBoostPrefab.transform.SetParent(transform, false);

        Debug.Log($"✅ Spawned Speed Boost prefab: {speedBoostPrefabs[index].name} as child of {gameObject.name}");
    }
}
