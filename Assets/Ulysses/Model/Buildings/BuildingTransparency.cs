using UnityEngine;
using System.Collections.Generic;

public class BuildingTransparency : MonoBehaviour
{
    public Transform player;                // Reference to the player
    public Material transparentMaterial;    // Your TransparentBuilding material
    public LayerMask buildingLayer;         // Layer containing buildings

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();

    void Update()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, player.position), buildingLayer);

        // Restore materials of buildings no longer in the way
        List<Renderer> toRestore = new List<Renderer>();
        foreach (var kvp in originalMaterials)
        {
            bool stillHit = false;
            foreach (var hit in hits)
            {
                if (hit.collider.GetComponent<Renderer>() == kvp.Key)
                {
                    stillHit = true;
                    break;
                }
            }
            if (!stillHit) toRestore.Add(kvp.Key);
        }

        foreach (var rend in toRestore)
        {
            rend.material = originalMaterials[rend];
            originalMaterials.Remove(rend);
        }

        // Make current blocking buildings transparent
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && !originalMaterials.ContainsKey(rend))
            {
                originalMaterials[rend] = rend.material;
                rend.material = transparentMaterial;
            }
        }
    }
}
