using UnityEngine;

public class BusParking : MonoBehaviour
{
    public LayerMask parkingZoneLayer;   // assign a "ParkingZone" layer
    public Transform busTransform;        // assign the bus transform
    public Vector3 checkSize = new Vector3(2, 1, 4); // approximate size of the bus

    [HideInInspector] public int currentStreetID = -1; // -1 = none
    [HideInInspector] public ParkingPlane currentPlane; // reference to the plane the bus is on

    // Returns true if the bus is inside any parking zone
    public bool IsProperlyParked()
    {
        Collider[] hits = Physics.OverlapBox(busTransform.position, checkSize / 2, busTransform.rotation, parkingZoneLayer);

        if (hits.Length > 0)
        {
            // Get the first plane the bus is overlapping
            currentPlane = hits[0].GetComponent<ParkingPlane>();
            if (currentPlane != null)
            {
                currentStreetID = currentPlane.streetID;
                return true;
            }
        }

        // If no plane detected
        currentPlane = null;
        currentStreetID = -1;
        return false;
    }

    // Optional: visualize the check box in the editor
    void OnDrawGizmos()
    {
        if (busTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(busTransform.position, busTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, checkSize);
        }
    }
}
