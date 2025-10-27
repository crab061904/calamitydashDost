using UnityEngine;

public class NPC_FloatingIndicator : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatHeight = 0.25f;   // how high it bobs up/down
    public float floatSpeed = 2f;       // speed of the bobbing

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // Smooth up & down bounce using sine wave
        float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

        // Always face the camera (billboarding)
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
