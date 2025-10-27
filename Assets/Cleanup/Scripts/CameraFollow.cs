using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Camera Position Settings")]
    public Vector3 baseOffset = new Vector3(0, 4, -3); // MUCH closer
    public float offsetGrowthFactor = 0.3f;
    public float followSmoothness = 6f;

    [Header("Camera Angle")]
    [Range(0f, 90f)] public float tiltAngle = 55f; // lower angle, closer look

    private Camera cam;
    private int playerSize = 1;

    [Header("Camera Zoom Settings")]
    public float baseFOV = 45f; // narrower view for closeness
    public float maxFOV = 65f;
    public float zoomSpeed = 2f;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFOV;

        PlayerGrowth pg = player.GetComponent<PlayerGrowth>();
        if (pg != null)
        {
            pg.onGrowth += HandlePlayerGrowth;
            playerSize = 1;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Dynamic offset (scales as player grows)
        Vector3 dynamicOffset = baseOffset * (1f + (playerSize - 1) * offsetGrowthFactor);

        // Rotate that offset by the tilt angle (pitch only, no yaw)
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        Vector3 rotatedOffset = tiltRotation * Vector3.forward * -dynamicOffset.magnitude;

        // Smooth follow to the new position
        Vector3 targetPosition = player.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSmoothness);

        // Lock camera rotation: only tilt, no yaw wobble
        transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);
    }

    void HandlePlayerGrowth(int newSize)
    {
        playerSize = newSize;
    }
}
