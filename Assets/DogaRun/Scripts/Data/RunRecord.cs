using System;
using System.Collections.Generic;
using System.Linq;

namespace DogaRun.Data
{
    [Serializable]
    public sealed class RunRecord
    {
        public string RunId;
        public string PlayerId;
        public string Username;
        public int Score;
        public float DurationSeconds;
        public float DistanceMeters;
        public string Difficulty;
        public int CompletedLoops;
        public int HitCount;
        public string CreatedAtUtc;
    }

    public static class RunHistoryMerger
    {
        public static IReadOnlyList<RunRecord> MergeTopFive(IEnumerable<RunRecord> local, IEnumerable<RunRecord> cloud)
        {
            var byId = new Dictionary<string, RunRecord>(StringComparer.Ordinal);
            AddUnique(byId, local);
            AddUnique(byId, cloud);

            return byId.Values
                .OrderByDescending(record => record.Score)
                .ThenByDescending(record => record.DistanceMeters)
                .ThenBy(record => record.CreatedAtUtc, StringComparer.Ordinal)
                .Take(5)
                .ToArray();
        }

        private static void AddUnique(IDictionary<string, RunRecord> destination, IEnumerable<RunRecord> records)
        {
            if (records == null)
            {
                return;
            }

            foreach (var record in records)
            {
                if (record == null || string.IsNullOrWhiteSpace(record.RunId))
                {
                    continue;
                }

                if (!destination.TryGetValue(record.RunId, out var existing) || IsBetter(record, existing))
                {
                    destination[record.RunId] = record;
                }
            }
        }

        private static bool IsBetter(RunRecord candidate, RunRecord existing)
        {
            return candidate.Score > existing.Score ||
                   candidate.Score == existing.Score && candidate.DistanceMeters > existing.DistanceMeters;
        }
    }
}
