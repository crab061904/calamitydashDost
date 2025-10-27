using UnityEngine;

public class MouseController: MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 10f;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y; // keep rotation flat

            Vector3 direction = (target - transform.position).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
