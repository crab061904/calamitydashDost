using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterManager : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
    }

    private void Update()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Get world x position of vertex
            float worldX = transform.position.x + vertices[i].x;

            // Apply wave height (assuming WaveManager has a singleton + method)
            vertices[i].y = WaveManager.instance.GetWaveHeight(worldX);
        }

        // Apply back the updated vertices
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
