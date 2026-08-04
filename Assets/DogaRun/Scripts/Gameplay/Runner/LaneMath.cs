namespace DogaRun.Gameplay.Runner
{
    public static class LaneMath
    {
        public const int MinimumLane = -1;
        public const int MaximumLane = 1;

        public static int Clamp(int laneIndex)
        {
            if (laneIndex < MinimumLane) return MinimumLane;
            if (laneIndex > MaximumLane) return MaximumLane;
            return laneIndex;
        }
    }
}
