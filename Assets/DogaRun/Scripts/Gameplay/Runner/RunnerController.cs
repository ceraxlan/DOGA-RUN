using DogaRun.Core;
using DogaRun.Gameplay.Input;
using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class RunnerController : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float laneDistance = 2.4f;
        [SerializeField, Min(0.05f)] private float laneChangeSmoothTime = 0.12f;
        [SerializeField, Min(0.1f)] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -25f;
        [SerializeField, Min(0.1f)] private float slideDuration = 0.75f;
        [SerializeField, Range(0.25f, 0.9f)] private float slideHeightRatio = 0.5f;

        private CharacterController characterController;
        private GameStateMachine gameStateMachine;
        private KeyboardInputReader keyboard;
        private SwipeInputReader swipe;
        private float horizontalVelocity;
        private float verticalVelocity;
        private float standingHeight;
        private Vector3 standingCenter;
        private float slideRemaining;
        private bool inputEnabled = true;

        public int LaneIndex { get; private set; }
        public bool IsSliding => slideRemaining > 0f;
        public bool IsJumping => verticalVelocity > 0.01f;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            standingHeight = characterController.height;
            standingCenter = characterController.center;
        }

        private void Update()
        {
            if (gameStateMachine != null && !gameStateMachine.IsGameplayActive) return;

            var deltaTime = Time.deltaTime;
            var current = transform.position;
            var targetX = LaneIndex * laneDistance;
            var nextX = Mathf.SmoothDamp(current.x, targetX, ref horizontalVelocity, laneChangeSmoothTime, Mathf.Infinity, deltaTime);

            if (characterController.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * deltaTime;

            characterController.Move(new Vector3(nextX - current.x, verticalVelocity * deltaTime, 0f));

            if (slideRemaining > 0f)
            {
                slideRemaining -= deltaTime;
                if (slideRemaining <= 0f) EndSlide();
            }
        }

        public void Initialize(GameStateMachine stateMachine)
        {
            gameStateMachine = stateMachine;
        }

        public void Configure(float configuredLaneDistance, float configuredJumpHeight, float configuredSlideDuration)
        {
            laneDistance = Mathf.Max(1f, configuredLaneDistance);
            jumpHeight = Mathf.Max(0.1f, configuredJumpHeight);
            slideDuration = Mathf.Max(0.1f, configuredSlideDuration);
        }

        public void Bind(KeyboardInputReader keyboardReader, SwipeInputReader swipeReader)
        {
            Unbind();
            keyboard = keyboardReader;
            swipe = swipeReader;
            if (keyboard != null)
            {
                keyboard.MoveLeftRequested += HandleMoveLeft;
                keyboard.MoveRightRequested += HandleMoveRight;
                keyboard.JumpRequested += HandleJump;
                keyboard.SlideRequested += HandleSlide;
            }
            if (swipe != null)
            {
                swipe.MoveLeftRequested += HandleMoveLeft;
                swipe.MoveRightRequested += HandleMoveRight;
                swipe.JumpRequested += HandleJump;
                swipe.SlideRequested += HandleSlide;
            }
        }

        public bool RequestMoveLeft()
        {
            if (!CanAcceptInput()) return false;
            var next = LaneMath.Clamp(LaneIndex - 1);
            if (next == LaneIndex) return false;
            LaneIndex = next;
            return true;
        }

        public bool RequestMoveRight()
        {
            if (!CanAcceptInput()) return false;
            var next = LaneMath.Clamp(LaneIndex + 1);
            if (next == LaneIndex) return false;
            LaneIndex = next;
            return true;
        }

        public bool RequestJump()
        {
            if (!CanAcceptInput() || IsSliding || IsJumping) return false;
            if (!characterController.isGrounded && transform.position.y > 0.05f) return false;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            return true;
        }

        public bool RequestSlide()
        {
            if (!CanAcceptInput() || IsSliding || IsJumping) return false;
            if (!characterController.isGrounded && transform.position.y > 0.05f) return false;
            slideRemaining = slideDuration;
            characterController.height = standingHeight * slideHeightRatio;
            characterController.center = standingCenter - Vector3.up * (standingHeight - characterController.height) * 0.5f;
            return true;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        public void ResetRunner()
        {
            LaneIndex = 0;
            horizontalVelocity = 0f;
            verticalVelocity = -2f;
            transform.position = Vector3.zero;
            EndSlide();
            inputEnabled = true;
        }

        private bool CanAcceptInput()
        {
            return inputEnabled && (gameStateMachine == null || gameStateMachine.CurrentState == GameState.Running);
        }

        private void HandleMoveLeft() => RequestMoveLeft();
        private void HandleMoveRight() => RequestMoveRight();
        private void HandleJump() => RequestJump();
        private void HandleSlide() => RequestSlide();

        private void EndSlide()
        {
            slideRemaining = 0f;
            if (characterController == null) return;
            characterController.height = standingHeight;
            characterController.center = standingCenter;
        }

        private void Unbind()
        {
            if (keyboard != null)
            {
                keyboard.MoveLeftRequested -= HandleMoveLeft;
                keyboard.MoveRightRequested -= HandleMoveRight;
                keyboard.JumpRequested -= HandleJump;
                keyboard.SlideRequested -= HandleSlide;
            }
            if (swipe != null)
            {
                swipe.MoveLeftRequested -= HandleMoveLeft;
                swipe.MoveRightRequested -= HandleMoveRight;
                swipe.JumpRequested -= HandleJump;
                swipe.SlideRequested -= HandleSlide;
            }
            keyboard = null;
            swipe = null;
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
