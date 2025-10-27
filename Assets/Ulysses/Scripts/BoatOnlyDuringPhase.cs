using UnityEngine;

public class BoatOnlyDuringPhase : MonoBehaviour
{
    [Header("References")]
    public GameObject rescueBoat;   // 🚤 the rescue boat
    public GameObject boatCamera;   // 🚤 boat camera
    public LimitCamera minimapCamera;

    [Header("Boat Driver Visual")]
    public GameObject boatDriverModel; // kneeling player inside the boat

    [Header("Markers")]
    public GameObject boatArrow;     // 🚤 boat arrow when driving


    private DuringPhase_BoatController boatController;

    private void Awake()
    {
        if (rescueBoat != null)
            boatController = rescueBoat.GetComponent<DuringPhase_BoatController>();
    }

    private void Start()
    {
        // Always driving from the start
        if (boatController != null)
            boatController.isPlayerDriving = true;

        // Show boat driver model
        if (boatDriverModel != null)
            boatDriverModel.SetActive(true);

        // Activate boat camera
        if (boatCamera != null)
            boatCamera.SetActive(true);

        // Update minimap to follow boat
        if (minimapCamera != null)
            minimapCamera.SetTarget(rescueBoat.transform, true);

        if (boatArrow != null)
            boatArrow.SetActive(true); // Show boat arrow
    }
}
