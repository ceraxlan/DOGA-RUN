using System;
using System.Collections.Generic;
using DogaRun.Configuration;
using DogaRun.Core;
using DogaRun.Gameplay.Collision;
using DogaRun.Gameplay.World;
using UnityEngine;

namespace DogaRun.Gameplay.Obstacles
{
    public sealed class AnimalObstacleSpawner : MonoBehaviour
    {
        private readonly Queue<AnimalObstacle> available = new Queue<AnimalObstacle>();
        private readonly List<AnimalObstacle> active = new List<AnimalObstacle>();
        private readonly List<AnimalObstacle> all = new List<AnimalObstacle>();
        private DifficultyConfig difficulty;
        private GameStateMachine gameStateMachine;
        private HitStateMachine hitStateMachine;
        private WorldSequenceController world;
        private System.Random random;
        private float spawnTimer;
        private float spawnZ = 42f;
        private float laneDistance = 2.4f;

        public int Capacity => all.Count;
        public int ActiveCount => active.Count;

        private void Update()
        {
            if (gameStateMachine == null || !gameStateMachine.IsGameplayActive) return;

            var distance = world.CurrentSpeed * Time.deltaTime;
            for (var index = active.Count - 1; index >= 0; index--)
            {
                var obstacle = active[index];
                obstacle.transform.position += Vector3.back * distance;
                if (obstacle.transform.position.z < -5f) RecycleAt(index);
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnOne();
                ScheduleNextSpawn();
            }
        }

        public void Initialize(
            AnimalObstacle template,
            int capacity,
            DifficultyConfig difficultyConfig,
            GameStateMachine stateMachine,
            HitStateMachine machine,
            WorldSequenceController sequence,
            int randomSeed = 1207)
        {
            difficulty = difficultyConfig;
            gameStateMachine = stateMachine;
            hitStateMachine = machine;
            world = sequence;
            random = new System.Random(randomSeed);

            for (var index = 0; index < Mathf.Max(1, capacity); index++)
            {
                var obstacle = Instantiate(template, transform);
                obstacle.name = $"AnimalObstacle_{index:00}";
                obstacle.Deactivate();
                all.Add(obstacle);
                available.Enqueue(obstacle);
            }
            template.gameObject.SetActive(false);
            ScheduleNextSpawn();
        }

        public bool SpawnOne()
        {
            if (available.Count == 0) return false;
            var obstacle = available.Dequeue();
            var lane = random.Next(-1, 2);
            obstacle.Activate(hitStateMachine, difficulty.RecoveryInvulnerability, new Vector3(lane * laneDistance, 0f, spawnZ));
            active.Add(obstacle);
            return true;
        }

        public void ResetSpawner(int randomSeed = 1207)
        {
            for (var index = active.Count - 1; index >= 0; index--) RecycleAt(index);
            random = new System.Random(randomSeed);
            ScheduleNextSpawn();
        }

        private void RecycleAt(int index)
        {
            var obstacle = active[index];
            active.RemoveAt(index);
            obstacle.Deactivate();
            available.Enqueue(obstacle);
        }

        private void ScheduleNextSpawn()
        {
            var min = difficulty == null ? 1.7f : difficulty.MinObstacleSpawnInterval;
            var max = difficulty == null ? 2.5f : difficulty.MaxObstacleSpawnInterval;
            spawnTimer = Mathf.Lerp(min, max, (float)random.NextDouble());
        }
    }
}
