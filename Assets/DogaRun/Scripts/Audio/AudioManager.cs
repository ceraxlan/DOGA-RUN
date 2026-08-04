using System.Collections.Generic;
using UnityEngine;

namespace DogaRun.Audio
{
    public enum AudioCategory { Music, Ambience, Sfx, UI, Character }
    public enum SurfaceType { Asphalt, Dirt, Wood, Grass, Rock }

    public sealed class AudioManager : MonoBehaviour
    {
        private readonly Dictionary<AudioCategory, float> volumes = new Dictionary<AudioCategory, float>();

        private void Awake()
        {
            foreach (AudioCategory category in System.Enum.GetValues(typeof(AudioCategory))) volumes[category] = 1f;
        }

        public void SetVolume(AudioCategory category, float value) => volumes[category] = Mathf.Clamp01(value);
        public float GetVolume(AudioCategory category) => volumes.TryGetValue(category, out var value) ? value : 1f;

        public void PlayFootstep(SurfaceType surface)
        {
            // PHASE 5: licensed clips will be mapped here. No fake AudioClip is played.
        }
    }
}
