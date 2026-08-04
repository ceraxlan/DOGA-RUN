using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DogaRun.Core
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField, Min(30)] private int targetFrameRate = 60;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    public sealed class SceneLoader
    {
        public IEnumerator LoadAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            while (operation != null && !operation.isDone) yield return null;
        }
    }
}
