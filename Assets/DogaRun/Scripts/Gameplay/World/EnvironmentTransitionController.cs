using UnityEngine;

namespace DogaRun.Gameplay.World
{
    public sealed class EnvironmentTransitionController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float transitionDuration = 2.5f;
        public float TransitionDuration => transitionDuration;

        public void SetTransitionDuration(float seconds)
        {
            transitionDuration = Mathf.Clamp(seconds, 0.1f, 10f);
        }
    }
}
