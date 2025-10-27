using UnityEngine;

public class NPCPrefabManager : MonoBehaviour
{
    [Header("NPC Location Prefabs")]
    public GameObject[] npcPrefabs; // Your 5+ prefabs with NPCs in different locations

    private GameObject activePrefab;

    void Start()
    {
        SpawnRandomNPCSet();
    }

    void SpawnRandomNPCSet()
    {
        if (npcPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No NPC prefabs assigned in NPCPrefabManager.");
            return;
        }

        // Pick one prefab randomly
        int index = Random.Range(0, npcPrefabs.Length);

        // Instead of forcing to (0,0,0), use prefab's own position & rotation
        Vector3 spawnPos = npcPrefabs[index].transform.position;
        Quaternion spawnRot = npcPrefabs[index].transform.rotation;

        activePrefab = Instantiate(npcPrefabs[index], spawnPos, spawnRot);

        Debug.Log($"✅ Spawned NPC prefab: {npcPrefabs[index].name} at {spawnPos}");
    }
}
