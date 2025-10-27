using UnityEngine;

public class OrientationFollower : MonoBehaviour
{
    public Transform cam;

    void Update()
    {
        // follow camera Y rotation only
        Vector3 rot = new Vector3(0, cam.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(rot);
    }
}