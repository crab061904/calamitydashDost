using UnityEngine;

public class SupplyZoneManager : MonoBehaviour
{
    [Header("Supply Zone Prefabs")]
    [Tooltip("Different supply zone layouts as prefabs")]
    public GameObject[] supplyZonePrefabs;

    private GameObject activeSupplyPrefab;

    void Start()
    {
        SpawnRandomSupplyZone();
    }

    void SpawnRandomSupplyZone()
    {
        if (supplyZonePrefabs == null || supplyZonePrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No Supply Zone prefabs assigned in SupplyZoneManager.");
            return;
        }

        // Pick one prefab randomly
        int index = Random.Range(0, supplyZonePrefabs.Length);
        GameObject prefab = supplyZonePrefabs[index];

        // ✅ Instantiate at prefab’s original position & rotation
        activeSupplyPrefab = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);

        // ✅ Optional: parent to this manager (keeps prefab's world transform)
        activeSupplyPrefab.transform.SetParent(transform, true);

        Debug.Log($"✅ Spawned Supply Zone prefab: {prefab.name} at its authored position.");
    }
}
