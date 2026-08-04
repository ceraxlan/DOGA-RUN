using DogaRun.Configuration;

namespace DogaRun.Gameplay.Difficulty
{
    public sealed class DifficultyManager
    {
        public DifficultyManager(DifficultyConfig initial)
        {
            Current = initial;
        }

        public DifficultyConfig Current { get; private set; }

        public void Select(DifficultyConfig config)
        {
            if (config != null) Current = config;
        }
    }
}
