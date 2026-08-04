using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DogaRun.Editor
{
    public static class AndroidBuildCommand
    {
        public static void BuildDevelopment()
        {
            ConfigureCommon(false);
            Build("Builds/Android/DogaRun-development.apk", BuildOptions.Development | BuildOptions.ConnectWithProfiler);
        }

        public static void BuildRelease()
        {
            ConfigureCommon(true);
            try
            {
                ConfigureSigningFromEnvironment();
                Build("Builds/Android/DogaRun-release.aab", BuildOptions.None);
            }
            finally
            {
                ClearSigningFromMemory();
            }
        }

        private static void ConfigureCommon(bool appBundle)
        {
            ProjectConfigurator.ApplyAll();
            EditorUserBuildSettings.buildAppBundle = appBundle;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        }

        private static void ConfigureSigningFromEnvironment()
        {
            var keystore = RequireEnvironment("DOGARUN_KEYSTORE_PATH");
            if (!File.Exists(keystore)) throw new BuildFailedException("DOGARUN_KEYSTORE_PATH mevcut bir dosyayı göstermiyor.");

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystore;
            PlayerSettings.Android.keystorePass = RequireEnvironment("DOGARUN_KEYSTORE_PASSWORD");
            PlayerSettings.Android.keyaliasName = RequireEnvironment("DOGARUN_KEY_ALIAS");
            PlayerSettings.Android.keyaliasPass = RequireEnvironment("DOGARUN_KEY_ALIAS_PASSWORD");
        }

        private static string RequireEnvironment(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value)) throw new BuildFailedException($"Gerekli ortam değişkeni eksik: {name}");
            return value;
        }

        private static void ClearSigningFromMemory()
        {
            PlayerSettings.Android.keystorePass = string.Empty;
            PlayerSettings.Android.keyaliasPass = string.Empty;
            PlayerSettings.Android.keyaliasName = string.Empty;
            PlayerSettings.Android.keystoreName = string.Empty;
            PlayerSettings.Android.useCustomKeystore = false;
        }

        private static void Build(string relativeOutput, BuildOptions options)
        {
            var output = Path.GetFullPath(relativeOutput);
            Directory.CreateDirectory(Path.GetDirectoryName(output) ?? throw new BuildFailedException("Build çıktı dizini çözümlenemedi."));
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (scenes.Length == 0) throw new BuildFailedException("Build Settings içinde etkin sahne yok.");

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.Android,
                options = options
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Android build başarısız: {report.summary.result}");
            Debug.Log($"DogaRun Android build başarılı: {output}");
        }
    }
}
