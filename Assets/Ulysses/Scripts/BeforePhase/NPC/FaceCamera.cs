using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Make the canvas face the camera, ignoring the NPC rotation
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}
