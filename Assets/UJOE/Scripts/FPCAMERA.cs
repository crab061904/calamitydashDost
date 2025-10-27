using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody; // Drag your player root here (the one that moves)

    [Header("Settings")]
    public float sensitivity = 200f;
    public float minPitch = -80f; 
    public float maxPitch = 80f;

    private float pitch = 0f;

    void Start()
    {
        // Lock cursor to screen center
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate up/down (pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Rotate left/right (yaw) affects player body
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
