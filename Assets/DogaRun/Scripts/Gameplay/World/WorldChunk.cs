using UnityEngine;

namespace DogaRun.Gameplay.World
{
    public sealed class WorldChunk : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float length = 18f;

        public float Length => length;
        public int RecycleCount { get; private set; }

        public void Configure(float chunkLength)
        {
            length = Mathf.Max(1f, chunkLength);
        }

        public void ActivateAt(float zPosition)
        {
            transform.position = new Vector3(0f, 0f, zPosition);
            gameObject.SetActive(true);
        }

        public void MarkRecycled()
        {
            RecycleCount++;
            gameObject.SetActive(false);
        }
    }
}
