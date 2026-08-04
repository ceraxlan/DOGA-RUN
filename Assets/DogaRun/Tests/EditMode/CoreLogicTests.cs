using System;
using System.Linq;
using DogaRun.Configuration;
using DogaRun.Data;
using DogaRun.Gameplay.Collision;
using DogaRun.Gameplay.Runner;
using DogaRun.Gameplay.Scoring;
using DogaRun.Gameplay.World;
using DogaRun.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace DogaRun.Tests.EditMode
{
    public sealed class CoreLogicTests
    {
        [TestCase("Çok Kolay", 6f, 0.08f, 2.4f, 3.2f, 2.5f, 0.8f)]
        [TestCase("Kolay", 7f, 0.10f, 2f, 2.8f, 2.2f, 1f)]
        [TestCase("Normal", 8f, 0.12f, 1.7f, 2.5f, 2f, 1.2f)]
        [TestCase("Zor", 9f, 0.15f, 1.4f, 2.2f, 1.7f, 1.5f)]
        [TestCase("Çok Zor", 10f, 0.18f, 1.2f, 1.9f, 1.5f, 2f)]
        public void DifficultyPresets_MatchDesignValues(string id, float speed, float increase, float min, float max, float invulnerability, float score)
        {
            var config = DifficultyPresetFactory.CreateRuntime(id);
            Assert.That(config.BaseSpeed, Is.EqualTo(speed));
            Assert.That(config.LoopSpeedIncrease, Is.EqualTo(increase));
            Assert.That(config.MinObstacleSpawnInterval, Is.EqualTo(min));
            Assert.That(config.MaxObstacleSpawnInterval, Is.EqualTo(max));
            Assert.That(config.RecoveryInvulnerability, Is.EqualTo(invulnerability));
            Assert.That(config.ScoreMultiplier, Is.EqualTo(score));
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void LaneIndex_IsClampedToThreeLanes()
        {
            Assert.That(LaneMath.Clamp(-99), Is.EqualTo(-1));
            Assert.That(LaneMath.Clamp(0), Is.EqualTo(0));
            Assert.That(LaneMath.Clamp(99), Is.EqualTo(1));
        }

        [Test]
        public void ScoreCalculator_UsesDistanceMultiplierAndLoopBonus()
        {
            Assert.That(new ScoreCalculator().Calculate(123.9f, 1.2f, 2), Is.EqualTo(1148));
        }

        [Test]
        public void HitStateMachine_TransitionsThroughRecoveriesAndGameOver()
        {
            var hits = new HitStateMachine();
            Assert.That(hits.TryRegisterHit(2f), Is.True);
            Assert.That(hits.State, Is.EqualTo(HitState.FirstHitRecovery));
            hits.CompleteRecoveryImmediately();
            Assert.That(hits.State, Is.EqualTo(HitState.Healthy));

            Assert.That(hits.TryRegisterHit(2f), Is.True);
            Assert.That(hits.State, Is.EqualTo(HitState.SecondHitRecovery));
            hits.CompleteRecoveryImmediately();

            Assert.That(hits.TryRegisterHit(2f), Is.True);
            Assert.That(hits.State, Is.EqualTo(HitState.FinalHit));
            hits.CompleteFinalHit();
            Assert.That(hits.State, Is.EqualTo(HitState.GameOver));
            Assert.That(hits.HitCount, Is.EqualTo(3));
        }

        [Test]
        public void HitStateMachine_RejectsHitDuringInvulnerability()
        {
            var hits = new HitStateMachine();
            Assert.That(hits.TryRegisterHit(2f), Is.True);
            Assert.That(hits.TryRegisterHit(2f), Is.False);
            Assert.That(hits.HitCount, Is.EqualTo(1));
        }

        [Test]
        public void WorldSequence_UsesRequiredOrderAndIncrementsLoop()
        {
            var sequence = new WorldSequenceModel();
            Assert.That(sequence.CurrentTheme, Is.EqualTo(WorldTheme.CityToForest));
            for (var index = 1; index < WorldSequenceModel.DefaultOrder.Length; index++)
                Assert.That(sequence.Advance(), Is.EqualTo(WorldSequenceModel.DefaultOrder[index]));
            Assert.That(sequence.LoopCount, Is.Zero);
            Assert.That(sequence.Advance(), Is.EqualTo(WorldTheme.CityToForest));
            Assert.That(sequence.LoopCount, Is.EqualTo(1));
        }

        [Test]
        public void SpeedMultiplier_IncreasesAndRespectsCap()
        {
            var config = DifficultyPresetFactory.CreateRuntime("Normal");
            Assert.That(config.GetSpeedMultiplier(0), Is.EqualTo(1f));
            Assert.That(config.GetSpeedMultiplier(2), Is.EqualTo(1.24f).Within(0.0001f));
            Assert.That(config.GetSpeedMultiplier(999), Is.EqualTo(config.MaximumSpeedMultiplier));
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void RunHistory_MergesDuplicateIdsAndKeepsBestFive()
        {
            var local = Enumerable.Range(1, 6).Select(index => Record($"run-{index}", index * 100, index * 10)).ToArray();
            var cloud = new[] { Record("run-3", 999, 999), Record("run-7", 50, 500) };
            var result = RunHistoryMerger.MergeTopFive(local, cloud);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.Count(item => item.RunId == "run-3"), Is.EqualTo(1));
            Assert.That(result[0].RunId, Is.EqualTo("run-3"));
            Assert.That(result[0].Score, Is.EqualTo(999));
        }

        [TestCase(0f, "00:00")]
        [TestCase(65.9f, "01:05")]
        [TestCase(3661f, "01:01:01")]
        public void TimeFormatter_UsesExpectedFormat(float seconds, string expected)
        {
            Assert.That(TimeFormatter.Format(seconds), Is.EqualTo(expected));
        }

        private static RunRecord Record(string id, int score, float distance) => new RunRecord
        {
            RunId = id,
            Score = score,
            DistanceMeters = distance,
            CreatedAtUtc = DateTime.UtcNow.ToString("O")
        };
    }
}
