using System;
using DogaRun.Configuration;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

namespace DogaRun.Editor
{
    public static class ProjectConfigurator
    {
        private const string SessionKey = "DogaRun.ProjectConfigured";
        private const string SettingsFolder = "Assets/DogaRun/Settings";

        [InitializeOnLoadMethod]
        private static void ScheduleFirstConfiguration()
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += ApplyAll;
        }

        [MenuItem("DogaRun/Apply Project Settings")]
        public static void ApplyAll()
        {
            ApplyPlayerSettings();
            EnsureUrpAssets();
            EnsureConfigurationAssets();
            AssetDatabase.SaveAssets();
        }

        private static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = "Ceraxlan Software";
            PlayerSettings.productName = "Doğa Koşusu";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "org.ceraxlan.dogarun");
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
        }

        private static void EnsureUrpAssets()
        {
            EnsureFolder(SettingsFolder);
            const string rendererPath = SettingsFolder + "/DogaRunUniversalRenderer.asset";
            const string pipelinePath = SettingsFolder + "/DogaRunUniversalRenderPipeline.asset";

            var pipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(pipelinePath);
            if (pipelineAsset == null)
            {
                var rendererType = Type.GetType("UnityEngine.Rendering.Universal.UniversalRendererData, Unity.RenderPipelines.Universal.Runtime");
                var pipelineType = Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
                if (rendererType == null || pipelineType == null)
                {
                    Debug.LogWarning("DogaRun: URP paketi henüz çözümlenmedi; ayarlar bir sonraki importta oluşturulacak.");
                    return;
                }

                var rendererData = ScriptableObject.CreateInstance(rendererType);
                rendererData.name = "DogaRunUniversalRenderer";
                AssetDatabase.CreateAsset(rendererData, rendererPath);

                var rawPipeline = ScriptableObject.CreateInstance(pipelineType);
                rawPipeline.name = "DogaRunUniversalRenderPipeline";
                var serialized = new SerializedObject(rawPipeline);
                var rendererList = serialized.FindProperty("m_RendererDataList");
                if (rendererList != null)
                {
                    rendererList.arraySize = 1;
                    rendererList.GetArrayElementAtIndex(0).objectReferenceValue = rendererData;
                }
                var defaultRenderer = serialized.FindProperty("m_DefaultRendererIndex");
                if (defaultRenderer != null) defaultRenderer.intValue = 0;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.CreateAsset(rawPipeline, pipelinePath);
                pipelineAsset = rawPipeline as RenderPipelineAsset;
            }

            if (pipelineAsset != null)
            {
                GraphicsSettings.defaultRenderPipeline = pipelineAsset;
                QualitySettings.renderPipeline = pipelineAsset;
            }
        }

        private static void EnsureConfigurationAssets()
        {
            const string baseFolder = "Assets/DogaRun/ScriptableObjects";
            EnsureFolder(baseFolder);
            EnsureFolder(baseFolder + "/Difficulty");
            CreateDifficulty("VeryEasy", "Çok Kolay", 6f, 0.08f, 2.4f, 3.2f, 2.5f, 0.8f);
            CreateDifficulty("Easy", "Kolay", 7f, 0.10f, 2f, 2.8f, 2.2f, 1f);
            CreateDifficulty("Normal", "Normal", 8f, 0.12f, 1.7f, 2.5f, 2f, 1.2f);
            CreateDifficulty("Hard", "Zor", 9f, 0.15f, 1.4f, 2.2f, 1.7f, 1.5f);
            CreateDifficulty("VeryHard", "Çok Zor", 10f, 0.18f, 1.2f, 1.9f, 1.5f, 2f);

            EnsureAsset<CharacterDefinition>(baseFolder + "/Character/PlaceholderDoga.asset");
            EnsureAsset<WorldSegmentDefinition>(baseFolder + "/Environments/SunlitForest.asset");
            EnsureAsset<WorldSequenceDefinition>(baseFolder + "/Environments/WorldSequence.asset");
            EnsureAsset<AnimalObstacleDefinition>(baseFolder + "/Obstacles/DogPlaceholder.asset");
            EnsureAsset<DogaRun.Configuration.AudioConfiguration>(baseFolder + "/AudioConfiguration.asset");
            EnsureAsset<GameBalanceConfiguration>(baseFolder + "/GameBalanceConfiguration.asset");
        }

        private static void CreateDifficulty(string fileName, string id, float speed, float increase, float minSpawn, float maxSpawn, float invulnerability, float score)
        {
            var path = $"Assets/DogaRun/ScriptableObjects/Difficulty/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<DifficultyConfig>(path) != null) return;
            var config = ScriptableObject.CreateInstance<DifficultyConfig>();
            config.Configure(id, speed, increase, minSpawn, maxSpawn, invulnerability, score);
            AssetDatabase.CreateAsset(config, path);
        }

        private static void EnsureAsset<T>(string path) where T : ScriptableObject
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) != null) return;
            EnsureFolder(System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/'));
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
        }

        private static void EnsureFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || AssetDatabase.IsValidFolder(path)) return;
            var normalized = path.Replace('\\', '/');
            var slash = normalized.LastIndexOf('/');
            if (slash <= 0) return;
            var parent = normalized.Substring(0, slash);
            var child = normalized.Substring(slash + 1);
            EnsureFolder(parent);
            if (!AssetDatabase.IsValidFolder(normalized)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
