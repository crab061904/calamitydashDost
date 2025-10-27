using UnityEngine;

namespace Bitgem.VFX.StylisedWater
{
    [RequireComponent(typeof(Rigidbody))]
    public class WaterVolumeFloater : MonoBehaviour
    {
        public HelperWaterVolume WaterVolumeHelper = null;
        public float floatStrength = 10f;   // upward force multiplier
        public float waterDrag = 1f;        // slows down in water

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            var instance = WaterVolumeHelper ? WaterVolumeHelper : HelperWaterVolume.Instance;
            if (!instance) return;

            float? waterHeight = instance.GetHeight(transform.position);
            if (!waterHeight.HasValue) return;

            float difference = waterHeight.Value - transform.position.y;

            // Apply buoyancy if below water surface
            if (difference > 0)
            {
                rb.AddForce(Vector3.up * difference * floatStrength, ForceMode.Acceleration);

                // Add some drag in water
                rb.AddForce(-rb.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }
}
