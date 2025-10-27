using UnityEngine;

public class StartParticleOnce : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        // Get the ParticleSystem component on this GameObject
        ps = GetComponent<ParticleSystem>();

        // Play when the game starts
        if (ps != null)
        {
            ps.Play();
            // Start a coroutine to disable after it's done
            StartCoroutine(DisableAfterPlay());
        }
    }

    private System.Collections.IEnumerator DisableAfterPlay()
    {
        // Wait until the particle system finishes
        yield return new WaitForSeconds(ps.main.duration);

        // Optional: if it has sub-emitters, wait until completely done
        while (ps.IsAlive(true))
        {
            yield return null;
        }

        // Disable the GameObject
        gameObject.SetActive(false);
    }
}
