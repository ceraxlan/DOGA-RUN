using System;
using System.Collections.Generic;
using UnityEngine;

namespace DogaRun.Configuration
{
    public enum WorldTheme
    {
        CityToForest,
        SunlitForest,
        WoodenBridge,
        OpenMeadow,
        MountainAscent,
        CliffPanorama,
        MountainDescent,
        DeepForestReturn
    }

    public enum AnimalBehaviourType
    {
        Stationary,
        CrossRoad,
        SmallHop,
        ChangeLane
    }

    [CreateAssetMenu(menuName = "DogaRun/World Segment", fileName = "WorldSegmentDefinition")]
    public sealed class WorldSegmentDefinition : ScriptableObject
    {
        [SerializeField] private WorldTheme theme = WorldTheme.SunlitForest;
        [SerializeField, Min(1f)] private float durationSeconds = 30f;
        [SerializeField] private GameObject[] chunkPrefabs = Array.Empty<GameObject>();

        public WorldTheme Theme => theme;
        public float DurationSeconds => durationSeconds;
        public IReadOnlyList<GameObject> ChunkPrefabs => chunkPrefabs;
    }

    [CreateAssetMenu(menuName = "DogaRun/World Sequence", fileName = "WorldSequenceDefinition")]
    public sealed class WorldSequenceDefinition : ScriptableObject
    {
        [SerializeField] private WorldSegmentDefinition[] segments = Array.Empty<WorldSegmentDefinition>();
        public IReadOnlyList<WorldSegmentDefinition> Segments => segments;
    }

    [CreateAssetMenu(menuName = "DogaRun/Animal Obstacle", fileName = "AnimalObstacleDefinition")]
    public sealed class AnimalObstacleDefinition : ScriptableObject
    {
        [SerializeField] private string animalId = "DogPlaceholder";
        [SerializeField] private GameObject prefab;
        [SerializeField] private AnimalBehaviourType behaviour;

        public string AnimalId => animalId;
        public GameObject Prefab => prefab;
        public AnimalBehaviourType Behaviour => behaviour;
    }

    [CreateAssetMenu(menuName = "DogaRun/Audio Configuration", fileName = "AudioConfiguration")]
    public sealed class AudioConfiguration : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.75f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.85f;
        [SerializeField, Range(0f, 1f)] private float uiVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float characterVolume = 0.8f;

        public float MusicVolume => musicVolume;
        public float AmbienceVolume => ambienceVolume;
        public float SfxVolume => sfxVolume;
        public float UiVolume => uiVolume;
        public float CharacterVolume => characterVolume;
    }

    [CreateAssetMenu(menuName = "DogaRun/Game Balance", fileName = "GameBalanceConfiguration")]
    public sealed class GameBalanceConfiguration : ScriptableObject
    {
        [SerializeField, Min(30)] private int targetFrameRate = 60;
        [SerializeField, Min(1f)] private float laneDistance = 2.4f;
        [SerializeField, Min(0.05f)] private float laneChangeDuration = 0.18f;
        [SerializeField, Min(0.1f)] private float jumpHeight = 2f;
        [SerializeField, Min(0.1f)] private float slideDuration = 0.75f;

        public int TargetFrameRate => targetFrameRate;
        public float LaneDistance => laneDistance;
        public float LaneChangeDuration => laneChangeDuration;
        public float JumpHeight => jumpHeight;
        public float SlideDuration => slideDuration;
    }

    public static class AppConfiguration
    {
        public const string AccountDeletionUrlKey = "ACCOUNT_DELETION_URL";
        public const string AccountDeletionUrl = "";
    }
}
