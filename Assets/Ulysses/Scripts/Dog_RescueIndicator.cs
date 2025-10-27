using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Dog_RescueIndicator : MonoBehaviour
{
    private Canvas canvas;
    private Dog_Seat dogSeat;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.enabled = false;

        dogSeat = GetComponentInParent<Dog_Seat>();
        if (dogSeat == null)
            Debug.LogError("Dog_RescueIndicator: No Dog_Seat found in parent!");

        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dogSeat == null || dogSeat.IsRescued) return;

        if (other.CompareTag("Boat") && canvas != null)
        {
            canvas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat") && canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private void Update()
    {
        if (dogSeat != null && dogSeat.IsRescued)
        {
            if (canvas != null)
                canvas.enabled = false;

            GetComponent<Collider>().enabled = false;
        }
    }
}
