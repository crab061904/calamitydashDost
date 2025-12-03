using UnityEngine;

public class EnterExitCar : MonoBehaviour
{
    [Header("References")]
    public GameObject player;                   
    public GameObject car;                      
    public Transform carSeat;                   
    public CameraChange playerCameraChange;     // Reference to your CameraChange script
    public CarCameraFollow carCamera;           
    public PrometeoCarController carController; 

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.F;
    public float interactDistance = 4f;         

    private bool isInCar = false;
    private Rigidbody carRigidbody;             

    void Start()
    {
        carRigidbody = car.GetComponent<Rigidbody>();
        
        // Lock the car in place immediately so it doesn't drift
        FreezeCar();
        
        carController.enabled = false;
        if (carCamera != null) carCamera.enabled = false;
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

        // 1. Switch Cameras FIRST (Before disabling player)
        if (playerCameraChange != null)
        {
            playerCameraChange.EnterTruck();
        }

        // 2. Move & Disable Player
        player.transform.position = carSeat.position;
        player.transform.rotation = carSeat.rotation;
        player.SetActive(false); 

        // 3. Enable Car Scripts
        carController.enabled = true;
        if (carCamera != null) carCamera.enabled = true;

        // 4. Unlock physics so the car can drive
        UnfreezeCar();
        
        // Audio
        if (carController.useSounds && carController.carEngineSound != null)
        {
            if (!carController.carEngineSound.isPlaying)
                carController.carEngineSound.Play();
        }
    }

    void ExitCar()
    {
        isInCar = false;

        // 1. Enable Player FIRST
        Vector3 exitPos = carSeat.position + car.transform.right * 2f + Vector3.up * 1f;
        player.transform.position = exitPos;
        player.transform.rotation = Quaternion.LookRotation(car.transform.forward, Vector3.up);
        player.SetActive(true);

        // 2. Switch Cameras Back
        if (playerCameraChange != null)
        {
            playerCameraChange.ExitTruck();
        }

        // 3. Disable Car Scripts
        carController.enabled = false;
        if (carCamera != null) carCamera.enabled = false;

        if (carController.carEngineSound != null && carController.carEngineSound.isPlaying)
            carController.carEngineSound.Stop();

        // 4. Lock physics again so the player can't push the truck
        FreezeCar();
    }

    // --- PHYSICS FIX FOR SLIDING/JUMPING ---
    void FreezeCar()
    {
        if (carRigidbody != null)
        {
            // FREEZE EVERYTHING: Stops rotation AND sliding (Position X/Z)
            // This makes the truck act like a solid wall when parked.
            carRigidbody.constraints = RigidbodyConstraints.FreezeRotation | 
                                       RigidbodyConstraints.FreezePositionX | 
                                       RigidbodyConstraints.FreezePositionZ;
                                       
            carRigidbody.linearDamping = 2f;
            carRigidbody.angularDamping = 3f;
        }
    }

    void UnfreezeCar()
    {
        if (carRigidbody != null)
        {
            // Release all constraints so you can drive
            carRigidbody.constraints = RigidbodyConstraints.None;
            carRigidbody.linearDamping = 0.05f;
            carRigidbody.angularDamping = 0.05f;
        }
    }
}