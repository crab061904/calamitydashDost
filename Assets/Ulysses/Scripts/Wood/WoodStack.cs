using UnityEngine;

public class WoodStack : MonoBehaviour
{
    public Transform[] planks; // assign planks in hierarchy order (top plank last)

    public GameObject PickUpPlank()
    {
        if (planks.Length == 0) return null;

        // Pick the top plank (last in array)
        GameObject topPlank = planks[planks.Length - 1].gameObject;

        // Detach it from the stack
        topPlank.transform.SetParent(null);
        topPlank.SetActive(true);

        // Resize array
        Transform[] newPlanks = new Transform[planks.Length - 1];
        for (int i = 0; i < planks.Length - 1; i++)
            newPlanks[i] = planks[i];
        planks = newPlanks;

        return topPlank;
    }

    public bool HasPlanks()
    {
        return planks.Length > 0;
    }
}
