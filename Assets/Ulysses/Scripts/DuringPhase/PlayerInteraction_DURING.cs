using UnityEngine;

public class PlayerInteraction_DURING : MonoBehaviour
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

    private bool isDriving = false;

    private void Start()
    {
        boatArrow.SetActive(false);
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
        isDriving = true;

        player.SetActive(false);
        rescueBoat.GetComponent<BoatController_DURING>().isPlayerDriving = true; // 🚤 swapped

        playerCamera.SetActive(false);
        boatCamera.SetActive(true);

        if (minimapCamera != null)
            minimapCamera.SetTarget(rescueBoat.transform, true);

        // 🔄 Toggle markers
        if (playerArrow != null) playerArrow.SetActive(false);
        if (boatArrow != null) boatArrow.SetActive(true);
        if (boatIcon != null) boatIcon.SetActive(false); // hide icon since we're driving
    }

    void ExitBoat()
    {
        isDriving = false;

        if (exitPoint != null)
        {
            player.transform.position = exitPoint.position;
            player.transform.rotation = exitPoint.rotation;
        }
        else
        {
            player.transform.position = rescueBoat.transform.position + rescueBoat.transform.right * 2f;
        }

        player.SetActive(true);
        rescueBoat.GetComponent<BoatController_DURING>().isPlayerDriving = false; // 🚤 swapped

        playerCamera.SetActive(true);
        boatCamera.SetActive(false);

        if (minimapCamera != null)
            minimapCamera.SetTarget(player.transform, false);

        // 🔄 Toggle markers
        if (playerArrow != null) playerArrow.SetActive(true);
        if (boatArrow != null) boatArrow.SetActive(false);
        if (boatIcon != null) boatIcon.SetActive(true); // show boat position when on foot
    }
}
