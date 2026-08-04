using System;

namespace DogaRun.Gameplay.Scoring
{
    public sealed class ScoreCalculator
    {
        public int Calculate(float distanceMeters, float difficultyScoreMultiplier, int completedLoopCount)
        {
            var distance = Math.Max(0f, distanceMeters);
            var multiplier = Math.Max(0f, difficultyScoreMultiplier);
            var loops = Math.Max(0, completedLoopCount);
            return (int)Math.Floor(distance * multiplier) + loops * 500;
        }
    }
}
