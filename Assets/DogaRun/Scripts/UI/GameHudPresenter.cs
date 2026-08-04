using DogaRun.Core;
using DogaRun.Gameplay.Collision;
using DogaRun.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DogaRun.UI
{
    public sealed class GameHudPresenter : MonoBehaviour
    {
        private TMP_Text timeText;
        private TMP_Text scoreText;
        private TMP_Text hitText;
        private TMP_Text debugText;
        private GameObject pausePanel;
        private RunSession session;
        private HitStateMachine hitStateMachine;

        public void Initialize(
            TMP_Text timer,
            TMP_Text score,
            TMP_Text hits,
            TMP_Text debug,
            Button pauseButton,
            GameObject pauseOverlay,
            RunSession runSession,
            HitStateMachine hitMachine,
            UnityEngine.Events.UnityAction pauseAction)
        {
            timeText = timer;
            scoreText = score;
            hitText = hits;
            debugText = debug;
            pausePanel = pauseOverlay;
            session = runSession;
            hitStateMachine = hitMachine;
            pauseButton.onClick.AddListener(pauseAction);
            SetPaused(false);
            Refresh(1f);
        }

        public void Refresh(float speedMultiplier)
        {
            if (session == null || hitStateMachine == null) return;
            timeText.text = TimeFormatter.Format(session.DurationSeconds);
            scoreText.text = $"SKOR {session.Score:000000}";
            hitText.text = $"HAK  {GetHitMarkers(hitStateMachine.HitCount)}";
            debugText.text = $"{speedMultiplier:0.00}x";
        }

        public void SetPaused(bool paused)
        {
            if (pausePanel != null) pausePanel.SetActive(paused);
        }

        private static string GetHitMarkers(int hits)
        {
            var remaining = Mathf.Clamp(3 - hits, 0, 3);
            return new string('●', remaining) + new string('○', 3 - remaining);
        }
    }
}
