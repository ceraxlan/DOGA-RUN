using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DogaRun.Gameplay.Input
{
    public sealed class SwipeInputReader : MonoBehaviour
    {
        [SerializeField, Min(10f)] private float minimumSwipePixels = 70f;
        [SerializeField, Min(0.05f)] private float maximumSwipeDuration = 0.8f;

        private Vector2 startPosition;
        private float startTime;
        private bool tracking;

        public event Action MoveLeftRequested;
        public event Action MoveRightRequested;
        public event Action JumpRequested;
        public event Action SlideRequested;

        private void Update()
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen == null) return;

            var touch = touchscreen.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                startPosition = touch.position.ReadValue();
                startTime = Time.unscaledTime;
                tracking = true;
            }

            if (tracking && touch.press.wasReleasedThisFrame)
            {
                tracking = false;
                ResolveSwipe(touch.position.ReadValue(), Time.unscaledTime - startTime);
            }
        }

        public bool ResolveSwipe(Vector2 endPosition, float duration)
        {
            if (duration < 0f || duration > maximumSwipeDuration) return false;

            var delta = endPosition - startPosition;
            if (delta.sqrMagnitude < minimumSwipePixels * minimumSwipePixels) return false;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x < 0f) MoveLeftRequested?.Invoke();
                else MoveRightRequested?.Invoke();
            }
            else
            {
                if (delta.y > 0f) JumpRequested?.Invoke();
                else SlideRequested?.Invoke();
            }

            return true;
        }

        public void SetTestStart(Vector2 position)
        {
            startPosition = position;
        }
    }
}
