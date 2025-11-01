using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // <-- TextMeshPro namespace

public class Fire : MonoBehaviour
{
    [Header("Intensity Settings")]
    [SerializeField, Range(0f, 1f)] private float currentIntensity = 0f;
    private float[] startIntensities = new float[0];
    [SerializeField] private ParticleSystem[] fireParticleSystems = new ParticleSystem[0];

    public bool isLit = false;
    private bool isExtinguished = false;

    [Header("Extinguish Settings")]
    private float timeLastWatered = 0f;
    [SerializeField] private float regenDelay = 2.5f;
    [SerializeField] private float regenRate = 0.1f;

    [Header("Ignition Settings")]
    [SerializeField] private float combustionRate = 0.2f;
    [SerializeField] private float spreadDelay = 1f;

    private HashSet<Fire> pendingSpreads = new HashSet<Fire>();
    private Rigidbody rb;
    private Collider col;

    [Header("Health UI")]
    [SerializeField] private Canvas healthCanvas;   // world-space canvas
    [SerializeField] private TMP_Text healthText;   // TextMeshPro text
    [SerializeField] private int maxHealth = 100;

    private void Awake()
    {
        col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 1f;
            col = sc;
        }
        else
        {
            col.isTrigger = true;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start()
    {
        startIntensities = new float[fireParticleSystems.Length];
        for (int i = 0; i < fireParticleSystems.Length; i++)
        {
            startIntensities[i] = fireParticleSystems[i].emission.rateOverTime.constant;
            if (!isLit)
                fireParticleSystems[i].Stop();
        }

        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(isLit);

        UpdateHealthUI();
    }

    private void Update()
    {
        if (isExtinguished)
        {
            if (healthCanvas != null)
                healthCanvas.gameObject.SetActive(false);
            return;
        }

        // Fire builds up if lit
        if (isLit && currentIntensity < 1.0f)
        {
            currentIntensity += combustionRate * Time.deltaTime;
            currentIntensity = Mathf.Clamp01(currentIntensity);
            ChangeIntensity();
        }

        // Fire regenerates only if lit and not yet max
        if (isLit && currentIntensity < 1.0f && Time.time - timeLastWatered >= regenDelay)
        {
            currentIntensity += regenRate * Time.deltaTime;
            currentIntensity = Mathf.Clamp01(currentIntensity);
            ChangeIntensity();
        }

        UpdateHealthUI();
    }

    public bool TryExtinguish(float amount)
    {
        if (isExtinguished) return false;

        timeLastWatered = Time.time;
        currentIntensity -= amount;
        currentIntensity = Mathf.Clamp01(currentIntensity);

        ChangeIntensity();
        UpdateHealthUI();

        if (currentIntensity <= 0f)
        {
            isLit = false;
            isExtinguished = true;

            foreach (var ps in fireParticleSystems)
                ps.Stop();

            if (healthCanvas != null)
                healthCanvas.gameObject.SetActive(false);

            Debug.Log($"Fire '{name}': fully extinguished.");

            if (GameManagerFire.Instance != null)
                GameManagerFire.Instance.FireExtinguished();

            return true;
        }

        return false;
    }

    public void Ignite()
    {
        if (isExtinguished) return;

        if (!isLit)
        {
            isLit = true;
            currentIntensity = 1f;
            ChangeIntensity();

            foreach (var ps in fireParticleSystems)
            {
                if (!ps.isPlaying)
                    ps.Play();
            }

            if (healthCanvas != null)
                healthCanvas.gameObject.SetActive(true);
        }
    }

    private void ChangeIntensity()
    {
        for (int i = 0; i < fireParticleSystems.Length; i++)
        {
            var emission = fireParticleSystems[i].emission;
            emission.rateOverTime = currentIntensity * startIntensities[i];

            if (currentIntensity <= 0f)
                fireParticleSystems[i].Stop();
            else if (!fireParticleSystems[i].isPlaying)
                fireParticleSystems[i].Play();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            int health = Mathf.RoundToInt(currentIntensity * maxHealth);
            healthText.text = $"{health}/{maxHealth}";
        }

        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(isLit && !isExtinguished);

        // Make canvas face camera
        if (healthCanvas != null && Camera.main != null)
        {
            healthCanvas.transform.rotation =
                Quaternion.LookRotation(healthCanvas.transform.position - Camera.main.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other) => HandlePossibleSpread(other);
    private void OnTriggerStay(Collider other) => HandlePossibleSpread(other);

    private void HandlePossibleSpread(Collider other)
    {
        if (!isLit || currentIntensity < 1f) return;

        Fire otherFire = other.GetComponentInParent<Fire>();
        if (otherFire != null && !otherFire.isLit && !otherFire.isExtinguished && !pendingSpreads.Contains(otherFire))
        {
            pendingSpreads.Add(otherFire);
            StartCoroutine(SpreadFire(otherFire));
        }
    }

    private IEnumerator SpreadFire(Fire otherFire)
    {
        yield return new WaitForSeconds(spreadDelay);

        if (isLit && currentIntensity >= 1f && otherFire != null && !otherFire.isLit && !otherFire.isExtinguished)
        {
            otherFire.Ignite();
        }

        pendingSpreads.Remove(otherFire);
    }

    private void OnDrawGizmosSelected()
    {
        if (GetComponent<Collider>() is SphereCollider sc)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + sc.center, sc.radius * transform.lossyScale.x);
        }
    }

    public float CurrentIntensity => currentIntensity;
}
