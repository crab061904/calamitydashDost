using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    [Header("Water Settings")]
    public Vector3 dimensions = new Vector3(10, 5, 10);
    public float tileSize = 6f;

    [Header("Wave Settings")]
    public float waveFrequency = 0.2f;
    public float waveScale = 0.5f;
    public float waveSpeed = 1f;

    // Get water height at a given world position
    public float GetHeight(Vector3 position)
    {
        float baseHeight = transform.position.y;

        float time = Time.time * waveSpeed;
        float waveOffset = (Mathf.Sin(position.x * waveFrequency + time) +
                            Mathf.Cos(position.z * waveFrequency + time)) * waveScale;

        return baseHeight + waveOffset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawCube(transform.position, dimensions);
    }
}
