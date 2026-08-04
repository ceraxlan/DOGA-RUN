using System;
using System.Collections.Generic;
using DogaRun.Configuration;
using DogaRun.Core;
using UnityEngine;

namespace DogaRun.Gameplay.World
{
    public sealed class WorldSequenceController : MonoBehaviour
    {
        private readonly List<WorldChunk> activeChunks = new List<WorldChunk>();
        private WorldChunkPool pool;
        private DifficultyConfig difficulty;
        private GameStateMachine gameStateMachine;
        private float nextSpawnZ;
        private int recycledSinceLoop;
        private int chunksPerLoop = 8;
        private float speedScale = 1f;

        public event Action<int> LoopCompleted;

        public int LoopCount { get; private set; }
        public int ActiveChunkCount => activeChunks.Count;
        public float CurrentSpeedMultiplier => difficulty == null ? 1f : difficulty.GetSpeedMultiplier(LoopCount);
        public float CurrentSpeed => difficulty == null ? 0f : difficulty.BaseSpeed * CurrentSpeedMultiplier * speedScale;

        private void Update()
        {
            if (gameStateMachine == null || !gameStateMachine.IsGameplayActive) return;
            Scroll(Time.deltaTime);
        }

        public void Initialize(
            WorldChunkPool chunkPool,
            DifficultyConfig difficultyConfig,
            GameStateMachine stateMachine,
            int activeCount = 6,
            int loopChunkCount = 8)
        {
            pool = chunkPool;
            difficulty = difficultyConfig;
            gameStateMachine = stateMachine;
            chunksPerLoop = Mathf.Max(1, loopChunkCount);
            ResetSequence(activeCount);
        }

        public void Scroll(float deltaTime)
        {
            if (deltaTime <= 0f || activeChunks.Count == 0) return;

            var distance = CurrentSpeed * deltaTime;
            for (var index = 0; index < activeChunks.Count; index++)
                activeChunks[index].transform.position += Vector3.back * distance;
            nextSpawnZ -= distance;

            while (activeChunks.Count > 0 && activeChunks[0].transform.position.z < -activeChunks[0].Length)
            {
                var recycled = activeChunks[0];
                activeChunks.RemoveAt(0);
                var length = recycled.Length;
                pool.Return(recycled);

                var next = pool.Rent();
                if (next == null) return;
                next.ActivateAt(nextSpawnZ);
                nextSpawnZ += length;
                activeChunks.Add(next);

                recycledSinceLoop++;
                if (recycledSinceLoop >= chunksPerLoop)
                {
                    recycledSinceLoop = 0;
                    LoopCount++;
                    LoopCompleted?.Invoke(LoopCount);
                }
            }
        }

        public void SetSpeedScale(float value)
        {
            speedScale = Mathf.Clamp01(value);
        }

        public void ResetSequence(int activeCount = 6)
        {
            for (var index = 0; index < activeChunks.Count; index++) pool?.Return(activeChunks[index]);
            activeChunks.Clear();
            LoopCount = 0;
            recycledSinceLoop = 0;
            speedScale = 1f;
            nextSpawnZ = 0f;

            if (pool == null) return;
            for (var index = 0; index < activeCount; index++)
            {
                var chunk = pool.Rent();
                if (chunk == null) break;
                chunk.ActivateAt(nextSpawnZ);
                nextSpawnZ += chunk.Length;
                activeChunks.Add(chunk);
            }
        }
    }
}
