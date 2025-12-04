using UnityEngine;
using UnityEngine.UI;

public class FireWaypoint : MonoBehaviour
{
    [Header("UI Reference")]
    public Image waypointImage;

    [Header("Player Setup")]
    public Transform player;        // Drag your Player object here
    public Camera playerCamera;     // Drag your "ThirdCam" or "FirstCam" here

    [Header("Car Setup")]
    public Transform car;           // Drag your Car object here
    public Camera carCamera;        // Drag your "CarCam" or "TruckCam" here

    [Header("Settings")]
    public float edgeBuffer = 50f;

    private Fire[] allFires;

    void Start()
    {
        allFires = Object.FindObjectsByType<Fire>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (waypointImage == null) return;

        // --- STEP 1: Determine which Mode we are in (Player vs Car) ---
        // We assume we are in Car Mode if the player object is disabled
        bool isCarMode = (car != null && player != null && !player.gameObject.activeInHierarchy);

        // --- STEP 2: Select the correct Camera and Position ---
        Camera activeCam;
        Vector3 centerPoint;

        if (isCarMode)
        {
            activeCam = carCamera;
            centerPoint = car.position;
        }
        else
        {
            activeCam = playerCamera;
            centerPoint = player.position;
        }

        // Safety check: If no camera is assigned/found, stop to prevent errors
        if (activeCam == null)
        {
            // Try to fallback to whatever Unity thinks is Main, just in case
            activeCam = Camera.main;
            if (activeCam == null) return;
        }

        // --- STEP 3: Find Closest Fire ---
        Fire targetFire = GetClosestActiveFire(centerPoint);

        if (targetFire == null)
        {
            waypointImage.gameObject.SetActive(false);
            return;
        }

        waypointImage.gameObject.SetActive(true);

        // --- STEP 4: Calculate Screen Position ---
        // This effectively projects the 3D fire position onto the 2D screen of the ACTIVE camera
        Vector3 screenPos = activeCam.WorldToScreenPoint(targetFire.transform.position);

        // --- STEP 5: Handle "Behind the Camera" ---
        // If z is negative, the target is behind the camera. 
        // We invert the position so the arrow points to the edge correctly.
        bool isBehind = screenPos.z < 0;
        
        if (isBehind)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        // --- STEP 6: Clamp to Screen Edges ---
        // This ensures the icon stays visible on the border
        float x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
        float y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);

        waypointImage.transform.position = new Vector3(x, y, 0);
    }

    Fire GetClosestActiveFire(Vector3 origin)
    {
        Fire closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Fire fire in allFires)
        {
            if (fire != null && fire.CurrentIntensity > 0 && fire.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(fire.transform.position, origin);
                if (dist < minDistance)
                {
                    closest = fire;
                    minDistance = dist;
                }
            }
        }
        return closest;
    }
}