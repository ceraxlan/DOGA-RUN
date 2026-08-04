using System.Collections;
using DogaRun.Gameplay.Collision;
using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    public sealed class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField, Min(0f)] private float runBobHeight = 0.04f;
        [SerializeField, Min(0f)] private float runBobFrequency = 8f;

        private HitStateMachine hitStateMachine;
        private Vector3 baseLocalPosition;
        private Coroutine reactionRoutine;

        private void Awake()
        {
            if (visualRoot == null) visualRoot = transform;
            baseLocalPosition = visualRoot.localPosition;
        }

        private void Update()
        {
            if (reactionRoutine != null || visualRoot == null) return;
            var bob = Mathf.Abs(Mathf.Sin(Time.time * runBobFrequency)) * runBobHeight;
            visualRoot.localPosition = baseLocalPosition + Vector3.up * bob;
        }

        public void Initialize(Transform root, HitStateMachine machine)
        {
            visualRoot = root == null ? transform : root;
            baseLocalPosition = visualRoot.localPosition;
            if (hitStateMachine != null) hitStateMachine.StateChanged -= HandleHitState;
            hitStateMachine = machine;
            if (hitStateMachine != null) hitStateMachine.StateChanged += HandleHitState;
        }

        public void ResetVisual()
        {
            if (reactionRoutine != null) StopCoroutine(reactionRoutine);
            reactionRoutine = null;
            visualRoot.localPosition = baseLocalPosition;
            visualRoot.localRotation = Quaternion.identity;
        }

        private void HandleHitState(HitState state)
        {
            if (state != HitState.FirstHitRecovery && state != HitState.SecondHitRecovery && state != HitState.FinalHit) return;
            if (reactionRoutine != null) StopCoroutine(reactionRoutine);
            reactionRoutine = StartCoroutine(PlayReaction(state));
        }

        private IEnumerator PlayReaction(HitState state)
        {
            var duration = state == HitState.FinalHit ? 0.85f : 0.45f;
            var tilt = state == HitState.SecondHitRecovery ? 16f : 68f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = Mathf.Sin(t * Mathf.PI);
                visualRoot.localRotation = Quaternion.Euler(0f, 0f, tilt * eased);
                visualRoot.localPosition = baseLocalPosition + Vector3.down * (state == HitState.SecondHitRecovery ? 0.08f : 0.25f) * eased;
                yield return null;
            }
            if (state != HitState.FinalHit)
            {
                visualRoot.localPosition = baseLocalPosition;
                visualRoot.localRotation = Quaternion.identity;
            }
            reactionRoutine = null;
        }

        private void OnDestroy()
        {
            if (hitStateMachine != null) hitStateMachine.StateChanged -= HandleHitState;
        }
    }
}
