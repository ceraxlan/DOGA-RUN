using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DogaRun.Editor
{
    public static class PlayGameCommand
    {
        private const string GameScenePath = "Assets/DogaRun/Scenes/Game.unity";

        [MenuItem("DogaRun/Play Vertical Slice")]
        public static void Start()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.Log("DogaRun: Play Mode zaten etkin.");
                return;
            }

            var gameScene = EditorBuildSettings.scenes.FirstOrDefault(scene =>
                scene.enabled && string.Equals(scene.path, GameScenePath, StringComparison.Ordinal));

            if (gameScene == null)
                throw new InvalidOperationException($"Etkin oyun sahnesi bulunamadı: {GameScenePath}");

            EditorSceneManager.OpenScene(gameScene.path, OpenSceneMode.Single);
            EditorApplication.delayCall += EnterPlayMode;
        }

        private static void EnterPlayMode()
        {
            EditorApplication.delayCall -= EnterPlayMode;
            EditorApplication.isPlaying = true;
        }
    }
}
