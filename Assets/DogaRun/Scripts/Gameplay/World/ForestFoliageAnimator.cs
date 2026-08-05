using UnityEngine;

namespace DogaRun.Gameplay.World
{
    /// <summary>
    /// One animator per pooled chunk keeps the forest alive without adding Update to every plant.
    /// </summary>
    public sealed class ForestFoliageAnimator : MonoBehaviour
    {
        [SerializeField] private Transform[] foliageGroups = System.Array.Empty<Transform>();
        [SerializeField, Min(0f)] private float swayDegrees = 2.8f;
        [SerializeField, Min(0f)] private float swaySpeed = 1.15f;

        private Quaternion[] baseRotations = System.Array.Empty<Quaternion>();

        public int AnimatedGroupCount => foliageGroups.Length;

        public void Configure(Transform[] groups)
        {
            foliageGroups = groups ?? System.Array.Empty<Transform>();
            CacheBaseRotations();
        }

        private void Awake()
        {
            CacheBaseRotations();
        }

        private void Update()
        {
            if (baseRotations.Length != foliageGroups.Length) CacheBaseRotations();
            var time = Time.time * swaySpeed;
            for (var index = 0; index < foliageGroups.Length; index++)
            {
                var target = foliageGroups[index];
                if (target == null) continue;
                var offset = Mathf.Sin(time + index * 1.37f) * swayDegrees;
                target.localRotation = baseRotations[index] * Quaternion.Euler(offset * 0.3f, 0f, offset);
            }
        }

        private void CacheBaseRotations()
        {
            baseRotations = new Quaternion[foliageGroups.Length];
            for (var index = 0; index < foliageGroups.Length; index++)
                baseRotations[index] = foliageGroups[index] == null ? Quaternion.identity : foliageGroups[index].localRotation;
        }
    }
}
