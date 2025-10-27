using UnityEngine;

public class ParkingOverlay : MonoBehaviour
{
    public BusParking busParkingCheck;      // Assign your BusParking script
    public GameObject parkingWarningUI;     // Assign the Text/Image UI here
    public BusController busController;     // Assign your BusController script here

    void Update()
    {

        if (busParkingCheck != null && parkingWarningUI != null && busController != null)
        {
            // Show warning if bus is NOT properly parked AND the player is NOT driving
            bool showWarning = !busParkingCheck.IsProperlyParked() && !busController.isPlayerDriving;
            parkingWarningUI.SetActive(showWarning);

        }
    }
}
