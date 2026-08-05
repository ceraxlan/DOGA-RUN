using System.Collections;
using DogaRun.Gameplay.Collision;
using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    /// <summary>
    /// Lightweight procedural animation layer. It animates semantic rig joints rather
    /// than a specific mesh, keeping the runner controller independent from art assets.
    /// </summary>
    public sealed class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private ProceduralDogaCharacter characterView;
        [SerializeField] private RunnerController runner;
        [SerializeField, Min(0f)] private float runBobHeight = 0.055f;
        [SerializeField, Min(0f)] private float runCycleSpeed = 10f;
        [SerializeField, Range(0f, 60f)] private float limbSwingAngle = 36f;
        [SerializeField, Range(0f, 25f)] private float maximumLaneLean = 13f;

        private HitStateMachine hitStateMachine;
        private Transform visualRoot;
        private Vector3 baseLocalPosition;
        private Coroutine reactionRoutine;
        private float previousRunnerX;
        private float laneLean;

        private void Awake()
        {
            visualRoot = characterView == null ? transform : characterView.transform;
            baseLocalPosition = visualRoot.localPosition;
            previousRunnerX = transform.position.x;
        }

        private void Update()
        {
            if (visualRoot == null) return;
            UpdateRecoveryBlink();
            if (reactionRoutine != null) return;

            if (characterView == null || !characterView.IsBuilt)
            {
                ApplyFallbackBob();
                return;
            }

            UpdateLaneLean();
            if (runner != null && runner.IsSliding)
            {
                ApplySlidePose();
            }
            else if (runner != null && runner.IsJumping)
            {
                ApplyJumpPose();
            }
            else
            {
                ApplyRunPose();
            }
        }

        public void Initialize(ProceduralDogaCharacter view, RunnerController runnerController, HitStateMachine machine)
        {
            characterView = view;
            runner = runnerController;
            visualRoot = characterView == null ? transform : characterView.transform;
            baseLocalPosition = visualRoot.localPosition;
            previousRunnerX = runner == null ? transform.position.x : runner.transform.position.x;
            BindHitState(machine);
            ResetVisual();
        }

        // Compatibility overload for a future imported prefab that only exposes a root transform.
        public void Initialize(Transform root, HitStateMachine machine)
        {
            characterView = root == null ? null : root.GetComponent<ProceduralDogaCharacter>();
            runner = GetComponent<RunnerController>();
            visualRoot = root == null ? transform : root;
            baseLocalPosition = visualRoot.localPosition;
            previousRunnerX = runner == null ? transform.position.x : runner.transform.position.x;
            BindHitState(machine);
            ResetVisual();
        }

        public void ResetVisual()
        {
            if (reactionRoutine != null) StopCoroutine(reactionRoutine);
            reactionRoutine = null;
            laneLean = 0f;
            if (visualRoot != null)
            {
                visualRoot.localPosition = baseLocalPosition;
                visualRoot.localRotation = Quaternion.identity;
            }
            characterView?.ResetPose();
        }

        private void BindHitState(HitStateMachine machine)
        {
            if (hitStateMachine != null) hitStateMachine.StateChanged -= HandleHitState;
            hitStateMachine = machine;
            if (hitStateMachine != null) hitStateMachine.StateChanged += HandleHitState;
        }

        private void ApplyRunPose()
        {
            var phase = Time.time * runCycleSpeed;
            var swing = Mathf.Sin(phase) * limbSwingAngle;
            var secondary = Mathf.Sin(phase * 2f);
            var bob = (0.5f + 0.5f * secondary) * runBobHeight;

            visualRoot.localPosition = baseLocalPosition + Vector3.up * bob;
            visualRoot.localRotation = Quaternion.Euler(secondary * 1.2f, 0f, laneLean);
            characterView.BodyRoot.localRotation = Quaternion.Euler(secondary * 2.5f, 0f, -laneLean * 0.18f);
            characterView.HeadRoot.localRotation = Quaternion.Euler(-secondary * 1.8f, 0f, -laneLean * 0.22f);
            characterView.LeftArmRoot.localRotation = Quaternion.Euler(swing, 0f, -8f);
            characterView.RightArmRoot.localRotation = Quaternion.Euler(-swing, 0f, 8f);
            characterView.LeftLegRoot.localRotation = Quaternion.Euler(-swing * 0.72f, 0f, 0f);
            characterView.RightLegRoot.localRotation = Quaternion.Euler(swing * 0.72f, 0f, 0f);
        }

        private void ApplyJumpPose()
        {
            var airPulse = Mathf.Sin(Time.time * runCycleSpeed * 0.45f) * 4f;
            visualRoot.localPosition = baseLocalPosition;
            visualRoot.localRotation = Quaternion.Euler(-4f, 0f, laneLean);
            characterView.BodyRoot.localRotation = Quaternion.Euler(-6f, 0f, 0f);
            characterView.HeadRoot.localRotation = Quaternion.Euler(5f, 0f, 0f);
            characterView.LeftArmRoot.localRotation = Quaternion.Euler(-42f + airPulse, 0f, -18f);
            characterView.RightArmRoot.localRotation = Quaternion.Euler(-42f - airPulse, 0f, 18f);
            characterView.LeftLegRoot.localRotation = Quaternion.Euler(24f, 0f, -5f);
            characterView.RightLegRoot.localRotation = Quaternion.Euler(-18f, 0f, 5f);
        }

        private void ApplySlidePose()
        {
            visualRoot.localPosition = baseLocalPosition + Vector3.down * 0.38f;
            visualRoot.localRotation = Quaternion.Euler(42f, 0f, laneLean * 0.4f);
            characterView.BodyRoot.localRotation = Quaternion.Euler(18f, 0f, 0f);
            characterView.HeadRoot.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            characterView.LeftArmRoot.localRotation = Quaternion.Euler(62f, 0f, -10f);
            characterView.RightArmRoot.localRotation = Quaternion.Euler(62f, 0f, 10f);
            characterView.LeftLegRoot.localRotation = Quaternion.Euler(-58f, 0f, -8f);
            characterView.RightLegRoot.localRotation = Quaternion.Euler(-58f, 0f, 8f);
        }

        private void ApplyFallbackBob()
        {
            var bob = Mathf.Abs(Mathf.Sin(Time.time * runCycleSpeed)) * runBobHeight;
            visualRoot.localPosition = baseLocalPosition + Vector3.up * bob;
        }

        private void UpdateLaneLean()
        {
            if (runner == null || Time.deltaTime <= 0f)
            {
                laneLean = Mathf.Lerp(laneLean, 0f, 0.15f);
                return;
            }

            var currentX = runner.transform.position.x;
            var horizontalSpeed = (currentX - previousRunnerX) / Time.deltaTime;
            previousRunnerX = currentX;
            var target = Mathf.Clamp(-horizontalSpeed * 1.8f, -maximumLaneLean, maximumLaneLean);
            var blend = 1f - Mathf.Exp(-12f * Time.deltaTime);
            laneLean = Mathf.Lerp(laneLean, target, blend);
        }

        private void UpdateRecoveryBlink()
        {
            if (characterView == null || hitStateMachine == null) return;
            var recovering = hitStateMachine.State == HitState.FirstHitRecovery || hitStateMachine.State == HitState.SecondHitRecovery;
            characterView.SetRenderersEnabled(!recovering || (Mathf.FloorToInt(Time.time * 12f) & 1) == 0);
        }

        private void HandleHitState(HitState state)
        {
            if (state == HitState.Healthy)
            {
                characterView?.SetRenderersEnabled(true);
                return;
            }
            if (state != HitState.FirstHitRecovery && state != HitState.SecondHitRecovery && state != HitState.FinalHit) return;
            if (reactionRoutine != null) StopCoroutine(reactionRoutine);
            reactionRoutine = StartCoroutine(PlayReaction(state));
        }

        private IEnumerator PlayReaction(HitState state)
        {
            var duration = state == HitState.FinalHit ? 0.9f : 0.72f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                if (state == HitState.FinalHit)
                {
                    var settle = Mathf.SmoothStep(0f, 1f, t);
                    visualRoot.localRotation = Quaternion.Euler(68f * settle, 0f, 28f * settle);
                    visualRoot.localPosition = baseLocalPosition + Vector3.down * (0.34f * settle);
                    ApplyFinalHitLimbs(settle);
                }
                else
                {
                    var envelope = Mathf.Sin(t * Mathf.PI);
                    var firstHit = state == HitState.FirstHitRecovery;
                    visualRoot.localRotation = Quaternion.Euler(firstHit ? 34f * envelope : 12f * envelope, 0f, firstHit ? 45f * envelope : -22f * envelope);
                    visualRoot.localPosition = baseLocalPosition + Vector3.down * ((firstHit ? 0.24f : 0.08f) * envelope);
                    ApplyRecoveryLimbs(envelope, firstHit);
                }
                yield return null;
            }

            if (state != HitState.FinalHit)
            {
                visualRoot.localPosition = baseLocalPosition;
                visualRoot.localRotation = Quaternion.identity;
                characterView?.ResetPose();
            }
            reactionRoutine = null;
        }

        private void ApplyRecoveryLimbs(float amount, bool firstHit)
        {
            if (characterView == null) return;
            var armAngle = firstHit ? -72f : -45f;
            characterView.LeftArmRoot.localRotation = Quaternion.Euler(armAngle * amount, 0f, -8f - 28f * amount);
            characterView.RightArmRoot.localRotation = Quaternion.Euler(-armAngle * amount, 0f, 8f + 28f * amount);
            characterView.LeftLegRoot.localRotation = Quaternion.Euler(32f * amount, 0f, 0f);
            characterView.RightLegRoot.localRotation = Quaternion.Euler(-24f * amount, 0f, 0f);
            characterView.HeadRoot.localRotation = Quaternion.Euler(12f * amount, 0f, -10f * amount);
        }

        private void ApplyFinalHitLimbs(float amount)
        {
            if (characterView == null) return;
            characterView.LeftArmRoot.localRotation = Quaternion.Euler(-76f * amount, 0f, -28f * amount);
            characterView.RightArmRoot.localRotation = Quaternion.Euler(-64f * amount, 0f, 34f * amount);
            characterView.LeftLegRoot.localRotation = Quaternion.Euler(28f * amount, 0f, -8f * amount);
            characterView.RightLegRoot.localRotation = Quaternion.Euler(-36f * amount, 0f, 10f * amount);
            characterView.HeadRoot.localRotation = Quaternion.Euler(18f * amount, 0f, -14f * amount);
        }

        private void OnDestroy()
        {
            if (hitStateMachine != null) hitStateMachine.StateChanged -= HandleHitState;
        }
    }
}
