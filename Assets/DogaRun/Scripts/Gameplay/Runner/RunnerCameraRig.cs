using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    public sealed class RunnerCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.5f, -7.5f);
        [SerializeField, Min(0.01f)] private float horizontalSmoothTime = 0.25f;
        [SerializeField, Min(0f)] private float verticalFollowRatio = 0.15f;
        private float horizontalVelocity;

        public void Initialize(Transform followTarget)
        {
            target = followTarget;
            Snap();
        }

        public void Snap()
        {
            if (target == null) return;
            transform.position = new Vector3(target.position.x + offset.x, offset.y + target.position.y * verticalFollowRatio, target.position.z + offset.z);
            transform.LookAt(target.position + new Vector3(0f, 1.1f, 5f));
        }

        private void LateUpdate()
        {
            if (target == null) return;
            var x = Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref horizontalVelocity, horizontalSmoothTime);
            transform.position = new Vector3(x, offset.y + target.position.y * verticalFollowRatio, target.position.z + offset.z);
            transform.LookAt(target.position + new Vector3(0f, 1.1f, 5f));
        }
    }
}
