using System;

namespace DogaRun.Core
{
    public sealed class RunSession
    {
        public float DurationSeconds { get; private set; }
        public float DistanceMeters { get; private set; }
        public int CompletedLoops { get; private set; }
        public int HitCount { get; private set; }
        public int Score { get; private set; }
        public string DifficultyId { get; private set; } = "Normal";

        public void Start(string difficultyId)
        {
            Reset();
            DifficultyId = string.IsNullOrWhiteSpace(difficultyId) ? "Normal" : difficultyId;
        }

        public void Tick(float deltaTime, float metersPerSecond)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            DurationSeconds += deltaTime;
            DistanceMeters += Math.Max(0f, metersPerSecond) * deltaTime;
        }

        public void SetHitCount(int hitCount)
        {
            HitCount = Math.Max(0, hitCount);
        }

        public void SetCompletedLoops(int completedLoops)
        {
            CompletedLoops = Math.Max(0, completedLoops);
        }

        public void SetScore(int score)
        {
            Score = Math.Max(0, score);
        }

        public void Reset()
        {
            DurationSeconds = 0f;
            DistanceMeters = 0f;
            CompletedLoops = 0;
            HitCount = 0;
            Score = 0;
        }
    }
}
