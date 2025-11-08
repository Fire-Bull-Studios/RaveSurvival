using UnityEngine;

namespace RaveSurvival
{
    public class StageLightRotationHandler : MonoBehaviour
    {
        [SerializeField] float maxYaw = 20f;
        [SerializeField] float maxPitch = 10f;
        [SerializeField] private float rotationSpeed = 20f;
        [SerializeField] private float maxRotationOffset = 15f;
        [SerializeField] private float minChangeInterval = .2f;
        [SerializeField] private float maxChangeInterval = 1f;
         Quaternion baseRot;
        Quaternion targetRot;        
        private float t;

        void Start()
        {
            baseRot = transform.localRotation;
            PickNewTarget();
        }
        void Update()
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation, targetRot, rotationSpeed * Time.deltaTime);
            t -= Time.deltaTime;
            if (t <= 0f) PickNewTarget();
        }

        void PickNewTarget()
        {
            t = Random.Range(minChangeInterval, maxChangeInterval);
            float yaw   = Random.Range(-maxYaw,   maxYaw);
            float pitch = Random.Range(-maxPitch, maxPitch);

            // order matters: X (pitch) then Y (yaw)
            targetRot = baseRot * Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}

