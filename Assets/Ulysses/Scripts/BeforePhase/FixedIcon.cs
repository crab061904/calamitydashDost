using UnityEngine;

public class FixedIcon : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        euler.y = 0f;   
        transform.eulerAngles = euler;
    }
}
