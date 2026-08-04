using System;

namespace DogaRun.Gameplay.Collision
{
    public enum HitState
    {
        Healthy,
        FirstHitRecovery,
        SecondHitRecovery,
        FinalHit,
        GameOver
    }

    public sealed class HitStateMachine
    {
        private float recoveryRemaining;

        public event Action<HitState> StateChanged;

        public HitState State { get; private set; } = HitState.Healthy;
        public int HitCount { get; private set; }
        public bool IsInvulnerable => recoveryRemaining > 0f || State != HitState.Healthy;

        public bool TryRegisterHit(float recoveryDuration)
        {
            if (IsInvulnerable) return false;

            HitCount++;
            if (HitCount == 1)
            {
                recoveryRemaining = Math.Max(0.01f, recoveryDuration);
                SetState(HitState.FirstHitRecovery);
            }
            else if (HitCount == 2)
            {
                recoveryRemaining = Math.Max(0.01f, recoveryDuration);
                SetState(HitState.SecondHitRecovery);
            }
            else
            {
                recoveryRemaining = 0f;
                SetState(HitState.FinalHit);
            }

            return true;
        }

        public void Tick(float deltaTime)
        {
            if (recoveryRemaining <= 0f || deltaTime <= 0f) return;
            recoveryRemaining = Math.Max(0f, recoveryRemaining - deltaTime);
            if (recoveryRemaining <= 0f && (State == HitState.FirstHitRecovery || State == HitState.SecondHitRecovery))
                SetState(HitState.Healthy);
        }

        public void CompleteFinalHit()
        {
            if (State == HitState.FinalHit) SetState(HitState.GameOver);
        }

        public void CompleteRecoveryImmediately()
        {
            recoveryRemaining = 0f;
            if (State == HitState.FirstHitRecovery || State == HitState.SecondHitRecovery) SetState(HitState.Healthy);
        }

        public void Reset()
        {
            HitCount = 0;
            recoveryRemaining = 0f;
            SetState(HitState.Healthy);
        }

        private void SetState(HitState next)
        {
            State = next;
            StateChanged?.Invoke(next);
        }
    }
}
