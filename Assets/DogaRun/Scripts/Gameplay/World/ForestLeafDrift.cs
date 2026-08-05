using System;
using UnityEngine;

namespace DogaRun.Gameplay.World
{
    /// <summary>
    /// Keeps a small deterministic field of leaves drifting around the runner.
    /// One component updates the whole field without requiring a particle module.
    /// </summary>
    public sealed class ForestLeafDrift : MonoBehaviour
    {
        [SerializeField] private Transform[] leaves = Array.Empty<Transform>();
        [SerializeField] private Vector3 minimumBounds = new(-4.5f, 0.8f, -2f);
        [SerializeField] private Vector3 maximumBounds = new(4.5f, 6.2f, 25f);
        [SerializeField] private float fallSpeed = 0.45f;
        [SerializeField] private float backwardSpeed = 0.8f;
        [SerializeField] private float swayStrength = 0.55f;

        private float[] phases = Array.Empty<float>();

        public int LeafCount => leaves.Length;

        public void Configure(Transform[] configuredLeaves)
        {
            leaves = configuredLeaves ?? Array.Empty<Transform>();
            phases = new float[leaves.Length];

            for (var index = 0; index < leaves.Length; index++)
            {
                phases[index] = index * 1.73f;
                ResetLeaf(index, InitialDepth(index));
            }
        }

        private void Update()
        {
            var time = Time.time;
            var deltaTime = Time.deltaTime;

            for (var index = 0; index < leaves.Length; index++)
            {
                var leaf = leaves[index];
                if (leaf == null)
                {
                    continue;
                }

                var speedVariation = 0.82f + (index % 4) * 0.09f;
                var position = leaf.localPosition;
                position.x += Mathf.Sin(time * 1.25f + phases[index]) * swayStrength * deltaTime;
                position.y -= fallSpeed * speedVariation * deltaTime;
                position.z -= backwardSpeed * speedVariation * deltaTime;
                leaf.localPosition = position;
                leaf.localRotation = Quaternion.Euler(
                    24f + Mathf.Sin(time * 1.7f + phases[index]) * 22f,
                    time * (28f + index * 2f) + phases[index] * 20f,
                    Mathf.Sin(time * 1.1f + phases[index]) * 35f);

                if (position.y < minimumBounds.y || position.z < minimumBounds.z)
                {
                    ResetLeaf(index, 1f);
                }
            }
        }

        private void ResetLeaf(int index, float depthFraction)
        {
            var leaf = leaves[index];
            if (leaf == null)
            {
                return;
            }

            var horizontalFraction = Mathf.Repeat(index * 0.6180339f + 0.19f, 1f);
            var heightOffset = Mathf.Repeat(index * 0.347f, 1f) * 1.4f;
            leaf.localPosition = new Vector3(
                Mathf.Lerp(minimumBounds.x, maximumBounds.x, horizontalFraction),
                maximumBounds.y + heightOffset,
                Mathf.Lerp(minimumBounds.z, maximumBounds.z, depthFraction));
        }

        private float InitialDepth(int index)
        {
            return Mathf.Repeat(index * 0.41421356f + 0.08f, 1f);
        }
    }
}
