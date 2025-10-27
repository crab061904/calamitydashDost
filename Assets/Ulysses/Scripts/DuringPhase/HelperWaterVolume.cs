using Bitgem.VFX.StylisedWater;
using UnityEngine;

public class HelperWaterVolume : MonoBehaviour
{
    public static HelperWaterVolume Instance { get; private set; }

    [Header("References")]
    public WaterVolume waterVolume;

    private void Awake()
    {
        Instance = this;
    }

    public float? GetHeight(Vector3 position)
    {
        if (!waterVolume) return null;
        return waterVolume.GetHeight(position);
    }
}
