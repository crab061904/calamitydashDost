using UnityEngine;

public class UIArrowDirection : MonoBehaviour
{
    public Transform target;       // Exit point
    public Transform player;       // Player on foot
    public Transform bus;          // Vehicle
    public RectTransform arrowUI;  // UI Arrow

    void Update()
    {
        Transform source = (player != null && player.gameObject.activeInHierarchy) ? player : bus;
        if (!source || !target || !arrowUI) return;

        // ✅ World direction (flat on XZ plane)
        Vector3 direction = target.position - source.position;
        direction.y = 0f;

        // ✅ Angle relative to world north (Z+ is 0°)
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // ✅ Rotate UI arrow (ONLY Z axis)
        arrowUI.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
