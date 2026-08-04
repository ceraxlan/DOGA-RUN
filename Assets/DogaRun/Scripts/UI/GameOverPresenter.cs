using DogaRun.Core;
using DogaRun.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DogaRun.UI
{
    public sealed class GameOverPresenter : MonoBehaviour
    {
        private GameObject root;
        private TMP_Text summaryText;

        public bool IsVisible => root != null && root.activeSelf;

        public void Initialize(GameObject overlay, TMP_Text summary, Button restart, Button mainMenu, UnityAction restartAction, UnityAction menuAction)
        {
            root = overlay;
            summaryText = summary;
            restart.onClick.AddListener(restartAction);
            mainMenu.onClick.AddListener(menuAction);
            Hide();
        }

        public void Show(RunSession session)
        {
            summaryText.text =
                $"SKOR  {session.Score}\n" +
                $"SÜRE  {TimeFormatter.Format(session.DurationSeconds)}\n" +
                $"MESAFE  {session.DistanceMeters:0} m\n" +
                $"DÖNGÜ  {session.CompletedLoops}\n" +
                $"ZORLUK  {session.DifficultyId}";
            root.SetActive(true);
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }

    public sealed class ProfilePresenter : MonoBehaviour
    {
        // PHASE 3: IRunHistoryRepository'den gelen top-five verisini sunacak.
    }
}
