using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DogaRun.Gameplay.Input
{
    public sealed class KeyboardInputReader : MonoBehaviour
    {
        public event Action MoveLeftRequested;
        public event Action MoveRightRequested;
        public event Action JumpRequested;
        public event Action SlideRequested;
        public event Action PauseRequested;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
                MoveLeftRequested?.Invoke();
            if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
                MoveRightRequested?.Invoke();
            if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                JumpRequested?.Invoke();
            if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
                SlideRequested?.Invoke();
            if (keyboard.escapeKey.wasPressedThisFrame)
                PauseRequested?.Invoke();
        }
    }
}
