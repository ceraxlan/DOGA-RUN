using System;
using DogaRun.Configuration;

namespace DogaRun.Gameplay.World
{
    public sealed class WorldSequenceModel
    {
        public static readonly WorldTheme[] DefaultOrder =
        {
            WorldTheme.CityToForest,
            WorldTheme.SunlitForest,
            WorldTheme.WoodenBridge,
            WorldTheme.OpenMeadow,
            WorldTheme.MountainAscent,
            WorldTheme.CliffPanorama,
            WorldTheme.MountainDescent,
            WorldTheme.DeepForestReturn
        };

        private readonly WorldTheme[] order;

        public WorldSequenceModel(WorldTheme[] customOrder = null)
        {
            order = customOrder == null || customOrder.Length == 0
                ? (WorldTheme[])DefaultOrder.Clone()
                : (WorldTheme[])customOrder.Clone();
        }

        public int SegmentIndex { get; private set; }
        public int LoopCount { get; private set; }
        public WorldTheme CurrentTheme => order[SegmentIndex];
        public int SegmentCount => order.Length;

        public WorldTheme Advance()
        {
            SegmentIndex++;
            if (SegmentIndex >= order.Length)
            {
                SegmentIndex = 0;
                LoopCount++;
            }
            return CurrentTheme;
        }

        public void Reset()
        {
            SegmentIndex = 0;
            LoopCount = 0;
        }
    }
}
