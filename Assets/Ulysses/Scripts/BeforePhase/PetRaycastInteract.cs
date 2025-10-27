using UnityEngine;

public class PetRaycastInteract : MonoBehaviour
{
    [Header("Interaction Zone")]
    public BoxCollider interactZone;  // waist collider

    private PetController currentTarget;

    void Awake()
    {
        if (interactZone == null)
            interactZone = GetComponent<BoxCollider>();

        if (interactZone != null)
            interactZone.isTrigger = true;
        else
            Debug.LogError("⚠️ No BoxCollider assigned for PetRaycastInteract!");
    }

    void Update()
    {
        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            if (currentTarget.CanInteract())
            {
                currentTarget.StartRescueQTEFromPlayer(this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PetController pet = other.GetComponent<PetController>();
        if (pet != null)
        {
            currentTarget = pet;

            // ✅ Show prompt if pet allows interaction
            if (pet.CanInteract())
                pet.GetComponent<PetInteract>()?.EnablePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PetController pet = other.GetComponent<PetController>();
        if (pet != null && currentTarget == pet)
        {
            // ✅ Hide prompt when leaving
            pet.GetComponent<PetInteract>()?.DisablePrompt();
            currentTarget = null;
        }
    }
}
