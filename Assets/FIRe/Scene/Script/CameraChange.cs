using UnityEngine;
using TMPro;

public class CameraChange : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject ThirdCam;
    public GameObject FirstCam;
    public GameObject CarCam;

    [Header("Player / Truck Cameras")]
    public GameObject PlayerCam; // Camera attached to player
    public GameObject TruckCam;  // Camera attached to truck

    [Header("Movement Scripts")]
    public MonoBehaviour Firstmovement;
    public MonoBehaviour Thirdmovement;

    [Header("Hose Ability")]
    public HoseController hoseController;
    public GameObject player;
    public GameObject promptTextObject;

    [Header("Audio")]
    public AudioSource drivingAudio; // assign in Inspector

    [Header("Settings")]
    public int CamMode = 0;
    private bool isPlayerNearTruck = false;
    private bool isDriving = false;
    private int lastPlayerCamMode = 0;
    private string defaultPromptText;

    void Start()
    {
        // Ensure cameras are set correctly
        SetInitialCameras();

        // Save default text
        if (promptTextObject != null)
        {
            TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
            if (tmp != null)
                defaultPromptText = tmp.text;
            promptTextObject.SetActive(false);
        }

        // Setup audio
        if (drivingAudio != null)
        {
            drivingAudio.loop = true;
        }
    }

    void Update()
    {
        // Toggle POV
        if (isPlayerNearTruck && !isDriving)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CamMode = (CamMode == 0) ? 1 : 0;
                SetCameraMode(CamMode);
            }

            if (Input.GetKeyDown(KeyCode.F))
                EnterTruck();
        }
        else if (isDriving)
        {
            if (Input.GetKeyDown(KeyCode.F))
                ExitTruck();
        }
    }

    private void SetInitialCameras()
    {
        // Make sure at least one camera is active at start
        if (PlayerCam != null) PlayerCam.SetActive(true);
        if (TruckCam != null) TruckCam.SetActive(false);
        if (ThirdCam != null) ThirdCam.SetActive(true);
        if (FirstCam != null) FirstCam.SetActive(false);
        if (CarCam != null) CarCam.SetActive(false);
    }

    public void SetCameraMode(int mode)
    {
        bool first = mode == 1;

        if (FirstCam != null) FirstCam.SetActive(first);
        if (ThirdCam != null) ThirdCam.SetActive(!first);
        if (CarCam != null) CarCam.SetActive(false);

        // Ensure at least one camera active
        if (!AnyCameraActive())
        {
            Debug.LogWarning("No camera active! Enabling ThirdCam by default.");
            if (ThirdCam != null) ThirdCam.SetActive(true);
        }

        if (Firstmovement != null) Firstmovement.enabled = first;
        if (Thirdmovement != null) Thirdmovement.enabled = !first;
        if (hoseController != null) hoseController.enabled = first;
    }

    private bool AnyCameraActive()
    {
        return (FirstCam != null && FirstCam.activeSelf) ||
               (ThirdCam != null && ThirdCam.activeSelf) ||
               (CarCam != null && CarCam.activeSelf) ||
               (PlayerCam != null && PlayerCam.activeSelf) ||
               (TruckCam != null && TruckCam.activeSelf);
    }

    private void EnterTruck()
    {
        isDriving = true;
        lastPlayerCamMode = CamMode;

        // Cameras
        if (CarCam != null) CarCam.SetActive(true);
        if (ThirdCam != null) ThirdCam.SetActive(false);
        if (FirstCam != null) FirstCam.SetActive(false);
        if (PlayerCam != null) PlayerCam.SetActive(false);
        if (TruckCam != null) TruckCam.SetActive(true);

        // Movement / hose
        if (Firstmovement != null) Firstmovement.enabled = false;
        if (Thirdmovement != null) Thirdmovement.enabled = false;
        if (hoseController != null) hoseController.enabled = false;

        // Prompt text
        if (promptTextObject != null)
        {
            promptTextObject.SetActive(true);
            TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = "[F] Exit Truck";
        }

        // Play driving audio
        if (drivingAudio != null && !drivingAudio.isPlaying)
            drivingAudio.Play();

        Debug.Log("🚗 Entered truck");
    }

    private void ExitTruck()
    {
        isDriving = false;

        if (CarCam != null) CarCam.SetActive(false);
        SetCameraMode(lastPlayerCamMode);
        if (TruckCam != null) TruckCam.SetActive(false);
        if (PlayerCam != null) PlayerCam.SetActive(true);

        // Prompt text
        if (promptTextObject != null)
        {
            promptTextObject.SetActive(true);
            TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = defaultPromptText;
        }

        // Stop driving audio
        if (drivingAudio != null && drivingAudio.isPlaying)
            drivingAudio.Stop();

        Debug.Log("🚶‍♂️ Exited truck");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNearTruck = true;
            if (promptTextObject != null)
                promptTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerNearTruck = false;
            if (promptTextObject != null)
                promptTextObject.SetActive(false);
        }
    }
}
