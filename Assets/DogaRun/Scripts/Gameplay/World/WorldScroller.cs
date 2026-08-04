using DogaRun.Core;
using UnityEngine;

namespace DogaRun.Gameplay.World
{
    public sealed class WorldScroller : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        private GameStateMachine gameStateMachine;

        public void Initialize(GameStateMachine stateMachine, float configuredSpeed)
        {
            gameStateMachine = stateMachine;
            speed = Mathf.Max(0f, configuredSpeed);
        }

        private void Update()
        {
            if (gameStateMachine != null && gameStateMachine.IsGameplayActive)
                transform.position += Vector3.back * (speed * Time.deltaTime);
        }
    }
}
