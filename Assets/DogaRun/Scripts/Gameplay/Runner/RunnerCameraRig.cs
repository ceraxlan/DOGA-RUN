using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    public sealed class RunnerCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -7.5f);
        [SerializeField, Min(0.01f)] private float horizontalSmoothTime = 0.25f;
        [SerializeField, Min(0f)] private float verticalFollowRatio = 0.15f;
        [SerializeField, Min(1f)] private float lookAheadDistance = 5f;
        private float horizontalVelocity;

        public void Configure(Vector3 configuredOffset, float horizontalSmoothing, float verticalRatio, float lookAhead)
        {
            offset = configuredOffset;
            horizontalSmoothTime = Mathf.Max(0.01f, horizontalSmoothing);
            verticalFollowRatio = Mathf.Max(0f, verticalRatio);
            lookAheadDistance = Mathf.Max(1f, lookAhead);
        }

        public void Initialize(Transform followTarget)
        {
            target = followTarget;
            Snap();
        }

        public void Snap()
        {
            if (target == null) return;
            transform.position = new Vector3(target.position.x + offset.x, offset.y + target.position.y * verticalFollowRatio, target.position.z + offset.z);
            transform.LookAt(target.position + new Vector3(0f, 1.05f, lookAheadDistance));
        }

        private void LateUpdate()
        {
            if (target == null) return;
            var x = Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref horizontalVelocity, horizontalSmoothTime);
            transform.position = new Vector3(x, offset.y + target.position.y * verticalFollowRatio, target.position.z + offset.z);
            transform.LookAt(target.position + new Vector3(0f, 1.05f, lookAheadDistance));
        }
    }
}
