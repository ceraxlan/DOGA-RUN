using DogaRun.Gameplay.Collision;
using UnityEngine;

namespace DogaRun.Gameplay.Obstacles
{
    public sealed class AnimalObstacle : MonoBehaviour
    {
        private HitStateMachine hitStateMachine;
        private float recoveryDuration;

        public bool HitConsumed { get; private set; }

        public void Activate(HitStateMachine machine, float configuredRecoveryDuration, Vector3 position)
        {
            hitStateMachine = machine;
            recoveryDuration = configuredRecoveryDuration;
            HitConsumed = false;
            transform.position = position;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);
        }

        public bool TryHit()
        {
            if (HitConsumed || hitStateMachine == null) return false;
            HitConsumed = true;
            return hitStateMachine.TryRegisterHit(recoveryDuration);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<DogaRun.Gameplay.Runner.RunnerController>() != null) TryHit();
        }
    }
}
