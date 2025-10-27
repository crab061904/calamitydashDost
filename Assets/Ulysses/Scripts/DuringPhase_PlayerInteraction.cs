using UnityEngine;

public class DuringPhase_PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject rescueBoat;   // 🚤 instead of bus
    public GameObject playerCamera;
    public GameObject boatCamera;   // 🚤 instead of busCamera
    public Transform exitPoint;
    public LimitCamera minimapCamera;

    [Header("Markers")]
    public GameObject playerArrow;   // 👣 player on foot
    public GameObject boatArrow;     // 🚤 boat arrow when driving
    public GameObject boatIcon;      // 🚤 boat position icon when on foot

    [Header("Boat Driver Visual")]
    public GameObject boatDriverModel; // kneeling player inside the boat

    private bool isDriving = false;
    private DuringPhase_BoatController boatController;

    private void Awake()
    {
        if (rescueBoat != null)
            boatController = rescueBoat.GetComponent<DuringPhase_BoatController>();

        // make sure boat driver is hidden at start
        if (boatDriverModel != null)
            boatDriverModel.SetActive(false);
    }

    private void Start()
    {
        if (boatArrow != null) boatArrow.SetActive(false);
        if (boatIcon != null) boatIcon.SetActive(true); // show boat icon initially
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isDriving)
            {
                float distance = Vector3.Distance(player.transform.position, rescueBoat.transform.position);
                if (distance <= 7f)
                {
                    EnterBoat();
                }
            }
            else
            {
                ExitBoat();
            }
        }
    }

    void EnterBoat()
    {
        if (boatController == null) return;

        isDriving = true;

        // Hide walking player
        if (player != null) player.SetActive(false);

        // Show kneeling driver inside boat
        if (boatDriverModel != null) boatDriverModel.SetActive(true);

        // Enable boat driving
        boatController.isPlayerDriving = true;

        // Switch cameras
        if (playerCamera != null) playerCamera.SetActive(false);
        if (boatCamera != null) boatCamera.SetActive(true);

        if (minimapCamera != null)
            minimapCamera.SetTarget(rescueBoat.transform, true);

        // 🔄 Toggle markers
        if (playerArrow != null) playerArrow.SetActive(false);
        if (boatArrow != null) boatArrow.SetActive(true);
        if (boatIcon != null) boatIcon.SetActive(false);
    }

    void ExitBoat()
    {
        if (boatController == null) return;

        isDriving = false;

        // Place player back outside
        if (exitPoint != null)
        {
            player.transform.position = exitPoint.position;
            player.transform.rotation = exitPoint.rotation;
        }
        else
        {
            player.transform.position = rescueBoat.transform.position + rescueBoat.transform.right * 2f;
        }

        // Show walking player again
        if (player != null) player.SetActive(true);

        // Hide kneeling driver
        if (boatDriverModel != null) boatDriverModel.SetActive(false);

        // Disable boat driving
        boatController.isPlayerDriving = false;

        // Switch cameras
        if (playerCamera != null) playerCamera.SetActive(true);
        if (boatCamera != null) boatCamera.SetActive(false);

        if (minimapCamera != null)
            minimapCamera.SetTarget(player.transform, false);

        // 🔄 Toggle markers
        if (playerArrow != null) playerArrow.SetActive(true);
        if (boatArrow != null) boatArrow.SetActive(false);
        if (boatIcon != null) boatIcon.SetActive(true);
    }
}
