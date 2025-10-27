using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LimitCamera : MonoBehaviour
{
    public Transform target; // Player OR Bus
    private Camera cam;

    [Header("Zoom Settings")]
    public float playerZoom = 80f; // size/FOV when on foot
    public float busZoom = 120f;   // size/FOV when driving bus
    public float zoomSpeed = 5f;   // smooth zoom

    private float targetZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = playerZoom;

        if (cam.orthographic)
            cam.orthographicSize = playerZoom;
        else
            cam.fieldOfView = playerZoom;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Follow position
        transform.position = new Vector3(target.position.x, 70, target.position.z);

        // Smooth zoom based on projection
        if (cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }

    public void SetTarget(Transform newTarget, bool isBus = false)
    {
        target = newTarget;
        targetZoom = isBus ? busZoom : playerZoom;
    }
}
