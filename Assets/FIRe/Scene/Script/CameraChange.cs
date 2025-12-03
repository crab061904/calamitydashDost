using UnityEngine;
using TMPro;

public class CameraChange : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject ThirdCam;
    public GameObject FirstCam;
    public GameObject CarCam;       // The Camera object on the vehicle
    public GameObject PlayerCam;    // The Camera object on the player
    public GameObject TruckCam;     // Alternate truck camera

    [Header("Movement Scripts")]
    public MonoBehaviour Firstmovement;
    public MonoBehaviour Thirdmovement;

    [Header("Hose Ability")]
    public HoseController hoseController;
    public GameObject promptTextObject;

    [Header("Audio")]
    public AudioSource drivingAudio;

    [Header("Settings")]
    public int CamMode = 0;
    private bool isPlayerNearTruck = false;
    private bool isDriving = false; // Add this missing variable back
    private int lastPlayerCamMode = 0;
    private string defaultPromptText;

    void Start()
    {
        SetInitialCameras();
        if (promptTextObject != null)
        {
            TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
            if (tmp != null) defaultPromptText = tmp.text;
            promptTextObject.SetActive(false);
        }
    }

    void Update()
    {
        // Only handle Camera Switching (POV) here. 
        // We REMOVED the 'F' key logic to prevent conflicts with EnterExitCar.cs

        if (isPlayerNearTruck && !isDriving)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CamMode = (CamMode == 0) ? 1 : 0;
                SetCameraMode(CamMode);
            }
        }
    }

    // Changed from private to PUBLIC so EnterExitCar can call it
    public void EnterTruck()
    {
        isDriving = true;
        lastPlayerCamMode = CamMode;

        // 1. Disable Player Cameras
        if (ThirdCam != null) ThirdCam.SetActive(false);
        if (FirstCam != null) FirstCam.SetActive(false);
        if (PlayerCam != null) PlayerCam.SetActive(false);
        
        // 2. Enable Vehicle Cameras
        if (CarCam != null) CarCam.SetActive(true);
        if (TruckCam != null) TruckCam.SetActive(true);

        // 3. Disable Player Controls
        if (Firstmovement != null) Firstmovement.enabled = false;
        if (Thirdmovement != null) Thirdmovement.enabled = false;
        if (hoseController != null) hoseController.enabled = false;

        // 4. UI & Audio
        if (promptTextObject != null)
        {
            promptTextObject.SetActive(true);
            TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = "[F] Exit Truck";
        }

        if (drivingAudio != null && !drivingAudio.isPlaying)
            drivingAudio.Play();

        Debug.Log("CameraChange: Switched to Truck Cameras");
    }

    // Changed from private to PUBLIC
    public void ExitTruck()
    {
        isDriving = false;

        // 1. Disable Vehicle Cameras
        if (CarCam != null) CarCam.SetActive(false);
        if (TruckCam != null) TruckCam.SetActive(false);
        
        // 2. Restore Player Cameras
        if (PlayerCam != null) PlayerCam.SetActive(true);
        SetCameraMode(lastPlayerCamMode); // Restores First/Third person correctly

        // 3. UI & Audio
        if (promptTextObject != null)
        {
            promptTextObject.SetActive(isPlayerNearTruck);
            if (isPlayerNearTruck)
            {
                TMP_Text tmp = promptTextObject.GetComponent<TMP_Text>();
                if (tmp != null) tmp.text = defaultPromptText;
            }
        }

        if (drivingAudio != null && drivingAudio.isPlaying)
            drivingAudio.Stop();
            
        Debug.Log("CameraChange: Switched to Player Cameras");
    }

    public void SetCameraMode(int mode)
    {
        bool first = mode == 1;
        if (FirstCam != null) FirstCam.SetActive(first);
        if (ThirdCam != null) ThirdCam.SetActive(!first);
        
        if (Firstmovement != null) Firstmovement.enabled = first;
        if (Thirdmovement != null) Thirdmovement.enabled = !first;
        if (hoseController != null) hoseController.enabled = first;
    }

    private void SetInitialCameras()
    {
        if (PlayerCam != null) PlayerCam.SetActive(true);
        if (TruckCam != null) TruckCam.SetActive(false);
        if (ThirdCam != null) ThirdCam.SetActive(true);
        if (FirstCam != null) FirstCam.SetActive(false);
        if (CarCam != null) CarCam.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Safer than checking gameObject directly
        {
            isPlayerNearTruck = true;
            if (promptTextObject != null) promptTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearTruck = false;
            if (promptTextObject != null) promptTextObject.SetActive(false);
        }
    }
}