using UnityEngine;

public class HiddenNPCSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject npcPrefab;
    public Transform[] spawnPoints;
    public Camera mainCamera;
    public EvacuationManager evacuationManager; // Reference to manager

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (evacuationManager == null)
            evacuationManager = FindObjectOfType<EvacuationManager>();
    }

    // Spawn NPCs randomly at hidden spawn points
    public int SpawnNPCs(int count)
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < count && attempts < 50)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (!IsVisibleFrom(spawnPoints[i].position))
                {
                    GameObject npc = Instantiate(npcPrefab, spawnPoints[i].position, Quaternion.identity);
                    spawned++;
                    break; // Only spawn one per attempt
                }
            }
            attempts++;
        }

        return spawned;
    }

    bool IsVisibleFrom(Vector3 worldPosition)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Bounds bounds = new Bounds(worldPosition, Vector3.one * 1f);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}
