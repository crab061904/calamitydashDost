using UnityEngine;

public class EnterExitCar : MonoBehaviour
{
    [Header("References")]
    public GameObject player;                   // The player GameObject
    public GameObject car;                      // The car GameObject
    public Transform carSeat;                   // Where the player sits when entering
    public CameraChange playerCameraChange;     // Your camera switch script
    public CarCameraFollow carCamera;           // Car camera follow script
    public PrometeoCarController carController; // Car control script

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.F;
    public float interactDistance = 3f;         // Distance to enter/exit the car

    private bool isInCar = false;
    private Rigidbody carRigidbody;             // Reference to the car's Rigidbody

    void Start()
    {
        carRigidbody = car.GetComponent<Rigidbody>();

        // Make sure the car starts frozen and controls off
        FreezeCar();
        carController.enabled = false;
        carCamera.enabled = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, car.transform.position);

        // --- ENTER CAR ---
        if (!isInCar && distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            EnterCar();
        }

        // --- EXIT CAR ---
        else if (isInCar && Input.GetKeyDown(interactKey))
        {
            ExitCar();
        }
    }

    void EnterCar()
    {
        isInCar = true;

        // Move player to the car seat and disable the player model
        player.transform.position = carSeat.position;
        player.transform.rotation = carSeat.rotation;
        player.SetActive(false);

        // Enable car control and car camera
        carController.enabled = true;
        carCamera.enabled = true;

        // Allow car movement
        UnfreezeCar();

        // Start car engine sound if available
        if (carController.useSounds && carController.carEngineSound != null)
        {
            if (!carController.carEngineSound.isPlaying)
            {
                carController.carEngineSound.Play();
            }
        }

        Debug.Log("Entered car");
    }

    void ExitCar()
    {
        isInCar = false;

        // Position player safely beside the car
        Vector3 exitPos = carSeat.position + car.transform.right * 2f + Vector3.up * 1f;
        player.transform.position = exitPos;
        player.transform.rotation = Quaternion.LookRotation(car.transform.forward, Vector3.up);
        player.SetActive(true);

        // Disable car driving and car camera
        carController.enabled = false;
        carCamera.enabled = false;

        // Stop car engine sound
        if (carController.carEngineSound != null && carController.carEngineSound.isPlaying)
        {
            carController.carEngineSound.Stop();
        }

        // Reset camera back to player
        if (playerCameraChange != null)
            playerCameraChange.SetCameraMode(0);

        // Slightly freeze the car to stop motion
        FreezeCar();

        Debug.Log("Exited car");
    }

    void FreezeCar()
    {
        if (carRigidbody != null)
        {
            carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                       RigidbodyConstraints.FreezeRotationZ;
            carRigidbody.linearDamping = 2f;
            carRigidbody.angularDamping = 3f;
        }
    }

    void UnfreezeCar()
    {
        if (carRigidbody != null)
        {
            carRigidbody.constraints = RigidbodyConstraints.None;
            carRigidbody.linearDamping = 0.05f;
            carRigidbody.angularDamping = 0.05f;
        }
    }
}

