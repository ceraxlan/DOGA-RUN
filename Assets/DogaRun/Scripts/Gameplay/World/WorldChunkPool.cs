using System.Collections.Generic;
using UnityEngine;

namespace DogaRun.Gameplay.World
{
    public sealed class WorldChunkPool : MonoBehaviour
    {
        private readonly Queue<WorldChunk> available = new Queue<WorldChunk>();
        private readonly List<WorldChunk> all = new List<WorldChunk>();

        public int Capacity => all.Count;
        public int AvailableCount => available.Count;

        public void Initialize(WorldChunk template, int capacity)
        {
            ClearPool();
            for (var index = 0; index < Mathf.Max(1, capacity); index++)
            {
                var instance = Instantiate(template, transform);
                instance.name = $"ForestChunk_{index:00}";
                instance.gameObject.SetActive(false);
                all.Add(instance);
                available.Enqueue(instance);
            }
            template.gameObject.SetActive(false);
        }

        public WorldChunk Rent()
        {
            return available.Count == 0 ? null : available.Dequeue();
        }

        public void Return(WorldChunk chunk)
        {
            if (chunk == null || available.Contains(chunk)) return;
            chunk.MarkRecycled();
            available.Enqueue(chunk);
        }

        private void ClearPool()
        {
            available.Clear();
            all.Clear();
        }
    }
}
