using UnityEngine;

public class NPC_Spawner_DURING : MonoBehaviour
{
    [Header("References")]
    public GameObject npcPrefab;
    public Transform[] spawnPoints;
    public Camera mainCamera;

    [Header("Spawn Settings")]
    public int maxNPCs = 6;
    public float spawnInterval = 5f;

    private float timer = 0f;
    private int currentNPCCount = 0;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // auto assign
    }

    void Update()
    {
        if (currentNPCCount >= maxNPCs) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            TrySpawnNPC();
            timer = 0f;
        }
    }

    void TrySpawnNPC()
    {
        if (spawnPoints.Length == 0 || npcPrefab == null) return;

        // Shuffle spawn points to pick a random invisible one
        int startIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int index = (startIndex + i) % spawnPoints.Length;
            Transform spawnPoint = spawnPoints[index];

            // ✅ Check if spawn point is visible
            if (!IsVisibleFrom(spawnPoint.position))
            {
                Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
                currentNPCCount++;
                break; // spawn only ONE
            }
        }
    }

    // Check if a world position is inside the camera view
    bool IsVisibleFrom(Vector3 worldPosition)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Bounds bounds = new Bounds(worldPosition, Vector3.one * 1f); // small cube
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    // 🔹 Decrement when NPCs rescued/dropped off
    public void DecreaseActiveNPCs(int amount)
    {
        currentNPCCount = Mathf.Max(0, currentNPCCount - amount);
    }

    public int GetCurrentNPCCount()
    {
        return currentNPCCount;
    }
}
