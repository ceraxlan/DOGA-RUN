using System.Collections;
using DogaRun.Configuration;
using DogaRun.Core;
using DogaRun.Gameplay.Collision;
using DogaRun.Gameplay.Obstacles;
using DogaRun.Gameplay.Runner;
using DogaRun.Gameplay.Scoring;
using DogaRun.Gameplay.World;
using UnityEngine;

namespace DogaRun.UI
{
    public sealed class GameLoopController : MonoBehaviour
    {
        private GameStateMachine gameState;
        private RunSession session;
        private DifficultyConfig difficulty;
        private HitStateMachine hits;
        private RunnerController runner;
        private CharacterAnimationController animationController;
        private WorldSequenceController world;
        private AnimalObstacleSpawner obstacles;
        private GameHudPresenter hud;
        private GameOverPresenter gameOver;
        private ScoreCalculator scoreCalculator;
        private GameState stateBeforePause;
        private Coroutine finalHitRoutine;

        private void Update()
        {
            if (gameState == null) return;
            if (gameState.IsGameplayActive)
            {
                hits.Tick(Time.deltaTime);
                session.Tick(Time.deltaTime, world.CurrentSpeed);
                session.SetCompletedLoops(world.LoopCount);
                session.SetScore(scoreCalculator.Calculate(session.DistanceMeters, difficulty.ScoreMultiplier, session.CompletedLoops));
                session.SetHitCount(hits.HitCount);
            }
            hud.Refresh(world.CurrentSpeedMultiplier);
        }

        public void Initialize(
            GameStateMachine stateMachine,
            RunSession runSession,
            DifficultyConfig difficultyConfig,
            HitStateMachine hitMachine,
            RunnerController runnerController,
            CharacterAnimationController characterAnimation,
            WorldSequenceController worldSequence,
            AnimalObstacleSpawner obstacleSpawner,
            ScoreCalculator calculator,
            GameHudPresenter hudPresenter,
            GameOverPresenter gameOverPresenter)
        {
            gameState = stateMachine;
            session = runSession;
            difficulty = difficultyConfig;
            hits = hitMachine;
            runner = runnerController;
            animationController = characterAnimation;
            world = worldSequence;
            obstacles = obstacleSpawner;
            scoreCalculator = calculator;
            hud = hudPresenter;
            gameOver = gameOverPresenter;
            hits.StateChanged += HandleHitState;
            gameState.StateChanged += HandleGameStateChanged;
        }

        public void TogglePause()
        {
            if (gameState.CurrentState == GameState.Paused)
            {
                gameState.TryTransition(stateBeforePause);
                return;
            }
            if (!gameState.IsGameplayActive) return;
            stateBeforePause = gameState.CurrentState;
            gameState.TryTransition(GameState.Paused);
        }

        public void Restart()
        {
            if (finalHitRoutine != null) StopCoroutine(finalHitRoutine);
            finalHitRoutine = null;
            hits.Reset();
            session.Start(difficulty.DifficultyId);
            runner.ResetRunner();
            animationController.ResetVisual();
            world.SetSpeedScale(1f);
            world.ResetSequence();
            obstacles.ResetSpawner();
            gameOver.Hide();
            hud.SetPaused(false);
            gameState.ResetToRunning();
        }

        public void ReturnToMenuPlaceholder()
        {
            gameOver.Hide();
            gameState.TryTransition(GameState.MainMenu);
        }

        private void HandleHitState(HitState state)
        {
            switch (state)
            {
                case HitState.FirstHitRecovery:
                case HitState.SecondHitRecovery:
                    gameState.TryTransition(GameState.Recovering);
                    runner.SetInputEnabled(false);
                    world.SetSpeedScale(0.7f);
                    break;
                case HitState.Healthy:
                    world.SetSpeedScale(1f);
                    runner.SetInputEnabled(true);
                    if (gameState.CurrentState == GameState.Recovering) gameState.TryTransition(GameState.Running);
                    break;
                case HitState.FinalHit:
                    runner.SetInputEnabled(false);
                    gameState.TryTransition(GameState.Recovering);
                    if (finalHitRoutine != null) StopCoroutine(finalHitRoutine);
                    finalHitRoutine = StartCoroutine(CompleteFinalHit());
                    break;
                case HitState.GameOver:
                    world.SetSpeedScale(0f);
                    gameState.TryTransition(GameState.GameOver);
                    gameOver.Show(session);
                    break;
            }
        }

        private IEnumerator CompleteFinalHit()
        {
            const float duration = 1f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                world.SetSpeedScale(1f - Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            hits.CompleteFinalHit();
            finalHitRoutine = null;
        }

        private void HandleGameStateChanged(GameState previous, GameState next)
        {
            hud.SetPaused(next == GameState.Paused);
        }

        private void OnDestroy()
        {
            if (hits != null) hits.StateChanged -= HandleHitState;
            if (gameState != null) gameState.StateChanged -= HandleGameStateChanged;
        }
    }
}
