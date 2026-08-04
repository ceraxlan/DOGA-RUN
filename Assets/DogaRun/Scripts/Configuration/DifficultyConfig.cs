using UnityEngine;

namespace DogaRun.Configuration
{
    [CreateAssetMenu(menuName = "DogaRun/Difficulty Config", fileName = "DifficultyConfig")]
    public sealed class DifficultyConfig : ScriptableObject
    {
        [SerializeField] private string difficultyId = "Normal";
        [SerializeField, Min(0.1f)] private float baseSpeed = 8f;
        [SerializeField, Min(0f)] private float loopSpeedIncrease = 0.12f;
        [SerializeField, Min(0.1f)] private float minObstacleSpawnInterval = 1.7f;
        [SerializeField, Min(0.1f)] private float maxObstacleSpawnInterval = 2.5f;
        [SerializeField, Min(0f)] private float recoveryInvulnerability = 2f;
        [SerializeField, Min(0f)] private float scoreMultiplier = 1.2f;
        [SerializeField, Min(1f)] private float maximumSpeedMultiplier = 2.5f;

        public string DifficultyId => difficultyId;
        public float BaseSpeed => baseSpeed;
        public float LoopSpeedIncrease => loopSpeedIncrease;
        public float MinObstacleSpawnInterval => minObstacleSpawnInterval;
        public float MaxObstacleSpawnInterval => maxObstacleSpawnInterval;
        public float RecoveryInvulnerability => recoveryInvulnerability;
        public float ScoreMultiplier => scoreMultiplier;
        public float MaximumSpeedMultiplier => maximumSpeedMultiplier;

        public float GetSpeedMultiplier(int loopCount)
        {
            return Mathf.Min(maximumSpeedMultiplier, 1f + Mathf.Max(0, loopCount) * loopSpeedIncrease);
        }

        public void Configure(
            string id,
            float speed,
            float loopIncrease,
            float minSpawn,
            float maxSpawn,
            float invulnerability,
            float score,
            float speedCap = 2.5f)
        {
            difficultyId = id;
            baseSpeed = speed;
            loopSpeedIncrease = loopIncrease;
            minObstacleSpawnInterval = minSpawn;
            maxObstacleSpawnInterval = maxSpawn;
            recoveryInvulnerability = invulnerability;
            scoreMultiplier = score;
            maximumSpeedMultiplier = speedCap;
        }

        private void OnValidate()
        {
            maxObstacleSpawnInterval = Mathf.Max(minObstacleSpawnInterval, maxObstacleSpawnInterval);
            maximumSpeedMultiplier = Mathf.Max(1f, maximumSpeedMultiplier);
        }
    }

    public static class DifficultyPresetFactory
    {
        public static DifficultyConfig CreateRuntime(string id)
        {
            var config = ScriptableObject.CreateInstance<DifficultyConfig>();
            config.hideFlags = HideFlags.DontSave;

            switch (id)
            {
                case "Çok Kolay":
                    config.Configure(id, 6f, 0.08f, 2.4f, 3.2f, 2.5f, 0.8f);
                    break;
                case "Kolay":
                    config.Configure(id, 7f, 0.10f, 2f, 2.8f, 2.2f, 1f);
                    break;
                case "Zor":
                    config.Configure(id, 9f, 0.15f, 1.4f, 2.2f, 1.7f, 1.5f);
                    break;
                case "Çok Zor":
                    config.Configure(id, 10f, 0.18f, 1.2f, 1.9f, 1.5f, 2f);
                    break;
                default:
                    config.Configure("Normal", 8f, 0.12f, 1.7f, 2.5f, 2f, 1.2f);
                    break;
            }

            return config;
        }
    }
}
