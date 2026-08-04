using System;

namespace DogaRun.Core
{
    public enum GameState
    {
        Booting,
        MainMenu,
        Countdown,
        Running,
        Recovering,
        Paused,
        GameOver
    }

    public sealed class GameStateMachine
    {
        public GameStateMachine(GameState initialState = GameState.Booting)
        {
            CurrentState = initialState;
        }

        public event Action<GameState, GameState> StateChanged;

        public GameState CurrentState { get; private set; }

        public bool IsGameplayActive => CurrentState == GameState.Running || CurrentState == GameState.Recovering;

        public bool TryTransition(GameState next)
        {
            if (next == CurrentState || !CanTransition(CurrentState, next))
            {
                return false;
            }

            var previous = CurrentState;
            CurrentState = next;
            StateChanged?.Invoke(previous, next);
            return true;
        }

        public void ResetToRunning()
        {
            var previous = CurrentState;
            CurrentState = GameState.Running;
            StateChanged?.Invoke(previous, CurrentState);
        }

        private static bool CanTransition(GameState current, GameState next)
        {
            switch (current)
            {
                case GameState.Booting:
                    return next == GameState.MainMenu || next == GameState.Countdown || next == GameState.Running;
                case GameState.MainMenu:
                    return next == GameState.Countdown || next == GameState.Running;
                case GameState.Countdown:
                    return next == GameState.Running || next == GameState.MainMenu;
                case GameState.Running:
                    return next == GameState.Recovering || next == GameState.Paused || next == GameState.GameOver;
                case GameState.Recovering:
                    return next == GameState.Running || next == GameState.Paused || next == GameState.GameOver;
                case GameState.Paused:
                    return next == GameState.Running || next == GameState.Recovering || next == GameState.MainMenu;
                case GameState.GameOver:
                    return next == GameState.Running || next == GameState.MainMenu;
                default:
                    return false;
            }
        }
    }
}
