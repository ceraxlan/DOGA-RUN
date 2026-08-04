using System;

namespace DogaRun.Utilities
{
    public static class TimeFormatter
    {
        public static string Format(float durationSeconds)
        {
            var totalSeconds = Math.Max(0, (int)Math.Floor(durationSeconds));
            var hours = totalSeconds / 3600;
            var minutes = totalSeconds % 3600 / 60;
            var seconds = totalSeconds % 60;
            return hours > 0
                ? $"{hours:00}:{minutes:00}:{seconds:00}"
                : $"{minutes:00}:{seconds:00}";
        }
    }
}
