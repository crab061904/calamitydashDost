using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;

    [Tooltip("Base Y height of the water plane. If waterPlane is set, this will be initialized from its Y.")]
    public float baseHeight = 0f;

    [Tooltip("Optional: assign your water plane here so baseHeight is set automatically.")]
    public Transform waterPlane;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this); return; }

        if (waterPlane != null)
            baseHeight = waterPlane.position.y;
    }

    private void Update()
    {
        offset += Time.deltaTime * speed;
    }

    // returns world-space Y of surface at x (and optionally z)
    public float GetWaveHeight(float x)
    {
        return baseHeight + amplitude * Mathf.Sin(x / length + offset);
    }

    // overload for 2D sampling using x and z (useful for 2D waves)
    public float GetWaveHeight(float x, float z)
    {
        // example combining both axes — tweak as needed
        float waveX = Mathf.Sin(x / length + offset);
        float waveZ = Mathf.Sin(z / length * 0.5f + offset * 0.7f);
        return baseHeight + amplitude * 0.6f * waveX + amplitude * 0.4f * waveZ;
    }
}
