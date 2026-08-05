using System.Collections;
using DogaRun.Configuration;
using DogaRun.Core;
using DogaRun.Gameplay.Collision;
using DogaRun.Gameplay.Obstacles;
using DogaRun.Gameplay.Runner;
using DogaRun.Gameplay.World;
using DogaRun.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace DogaRun.Tests.PlayMode
{
    public sealed class VerticalSlicePlayModeTests
    {
        [UnityTest]
        public IEnumerator Runner_ChangesLanesAndStopsAtBounds()
        {
            var runner = CreateRunner(out var root);
            Assert.That(runner.RequestMoveLeft(), Is.True);
            Assert.That(runner.LaneIndex, Is.EqualTo(-1));
            Assert.That(runner.RequestMoveLeft(), Is.False);
            Assert.That(runner.RequestMoveRight(), Is.True);
            Assert.That(runner.RequestMoveRight(), Is.True);
            Assert.That(runner.LaneIndex, Is.EqualTo(1));
            Assert.That(runner.RequestMoveRight(), Is.False);
            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Runner_JumpsOnlyOnceAndSlides()
        {
            var runner = CreateRunner(out var root);
            Assert.That(runner.RequestJump(), Is.True);
            Assert.That(runner.RequestJump(), Is.False);
            Object.Destroy(root);
            yield return null;

            runner = CreateRunner(out root);
            Assert.That(runner.RequestSlide(), Is.True);
            Assert.That(runner.IsSliding, Is.True);
            Assert.That(runner.RequestSlide(), Is.False);
            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ProceduralDoga_BuildsChildFriendlySemanticRig()
        {
            var root = new GameObject("DogaVisualTest");
            var character = root.AddComponent<ProceduralDogaCharacter>();
            character.Build();

            Assert.That(character.IsBuilt, Is.True);
            Assert.That(character.PartCount, Is.GreaterThanOrEqualTo(35));
            Assert.That(character.HeadRoot, Is.Not.Null);
            Assert.That(character.LeftArmRoot, Is.Not.Null);
            Assert.That(character.RightArmRoot, Is.Not.Null);
            Assert.That(character.LeftLegRoot, Is.Not.Null);
            Assert.That(character.RightLegRoot, Is.Not.Null);
            Assert.That(root.transform.Find("DogaRig/Head/Sol Mavi İris"), Is.Not.Null);
            Assert.That(root.transform.Find("DogaRig/Body/Yaprak Rozeti"), Is.Not.Null);

            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterAnimation_ProducesRunningLimbMotion()
        {
            var root = new GameObject("AnimatedRunner");
            var controller = root.AddComponent<CharacterController>();
            controller.height = 1.7f;
            controller.center = new Vector3(0f, 0.85f, 0f);
            var runner = root.AddComponent<RunnerController>();
            runner.Initialize(new GameStateMachine(GameState.Running));

            var visualObject = new GameObject("DogaVisual");
            visualObject.transform.SetParent(root.transform, false);
            var character = visualObject.AddComponent<ProceduralDogaCharacter>();
            character.Build();
            var animationController = root.AddComponent<CharacterAnimationController>();
            animationController.Initialize(character, runner, new HitStateMachine());
            var initialRotation = character.LeftArmRoot.localRotation;

            yield return new WaitForSeconds(0.15f);

            Assert.That(Quaternion.Angle(initialRotation, character.LeftArmRoot.localRotation), Is.GreaterThan(1f));
            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AnimalObstacle_ConsumesColliderHitOnce()
        {
            var hitMachine = new HitStateMachine();
            var obstacleObject = new GameObject("Obstacle");
            var obstacle = obstacleObject.AddComponent<AnimalObstacle>();
            obstacle.Activate(hitMachine, 2f, Vector3.zero);
            Assert.That(obstacle.TryHit(), Is.True);
            Assert.That(obstacle.TryHit(), Is.False);
            Assert.That(hitMachine.HitCount, Is.EqualTo(1));
            Object.Destroy(obstacleObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator HitMachine_FirstSecondAndThirdHitBehaveAsDesigned()
        {
            var hits = new HitStateMachine();
            hits.TryRegisterHit(1f);
            Assert.That(hits.State, Is.EqualTo(HitState.FirstHitRecovery));
            hits.CompleteRecoveryImmediately();
            hits.TryRegisterHit(1f);
            Assert.That(hits.State, Is.EqualTo(HitState.SecondHitRecovery));
            hits.CompleteRecoveryImmediately();
            hits.TryRegisterHit(1f);
            hits.CompleteFinalHit();
            Assert.That(hits.State, Is.EqualTo(HitState.GameOver));
            yield return null;
        }

        [UnityTest]
        public IEnumerator GameOverPresenter_OpensOverlay()
        {
            var root = new GameObject("PresenterRoot");
            var overlay = new GameObject("Overlay");
            var summary = new GameObject("Summary").AddComponent<TextMeshProUGUI>();
            var restart = new GameObject("Restart").AddComponent<Button>();
            var menu = new GameObject("Menu").AddComponent<Button>();
            var presenter = root.AddComponent<GameOverPresenter>();
            presenter.Initialize(overlay, summary, restart, menu, () => { }, () => { });
            var session = new RunSession();
            session.Start("Normal");
            session.Tick(2f, 8f);
            presenter.Show(session);
            Assert.That(presenter.IsVisible, Is.True);
            Object.Destroy(root);
            Object.Destroy(overlay);
            Object.Destroy(summary.gameObject);
            Object.Destroy(restart.gameObject);
            Object.Destroy(menu.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RestartSemantics_ResetSession()
        {
            var session = new RunSession();
            session.Start("Normal");
            session.Tick(10f, 8f);
            session.SetHitCount(2);
            session.Start("Normal");
            Assert.That(session.DurationSeconds, Is.Zero);
            Assert.That(session.DistanceMeters, Is.Zero);
            Assert.That(session.HitCount, Is.Zero);
            Assert.That(session.Score, Is.Zero);
            yield return null;
        }

        [UnityTest]
        public IEnumerator WorldChunkPool_RecyclesWithoutGrowing()
        {
            var root = new GameObject("WorldTest");
            var templateObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            templateObject.transform.SetParent(root.transform);
            var template = templateObject.AddComponent<WorldChunk>();
            template.Configure(10f);
            var pool = root.AddComponent<WorldChunkPool>();
            pool.Initialize(template, 4);
            var config = DifficultyPresetFactory.CreateRuntime("Normal");
            var sequence = root.AddComponent<WorldSequenceController>();
            sequence.Initialize(pool, config, new GameStateMachine(GameState.Running), 3, 8);
            sequence.Scroll(10f);
            Assert.That(pool.Capacity, Is.EqualTo(4));
            Assert.That(sequence.ActiveChunkCount, Is.EqualTo(3));
            Object.Destroy(config);
            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PausedState_DoesNotAdvanceSessionWhenLoopHonorsState()
        {
            var state = new GameStateMachine(GameState.Running);
            var session = new RunSession();
            session.Start("Normal");
            if (state.IsGameplayActive) session.Tick(1f, 8f);
            state.TryTransition(GameState.Paused);
            if (state.IsGameplayActive) session.Tick(1f, 8f);
            Assert.That(session.DurationSeconds, Is.EqualTo(1f));
            yield return null;
        }

        private static RunnerController CreateRunner(out GameObject root)
        {
            root = new GameObject("Runner");
            var characterController = root.AddComponent<CharacterController>();
            characterController.height = 1.7f;
            characterController.center = new Vector3(0f, 0.85f, 0f);
            var runner = root.AddComponent<RunnerController>();
            runner.Initialize(new GameStateMachine(GameState.Running));
            return runner;
        }
    }
}
