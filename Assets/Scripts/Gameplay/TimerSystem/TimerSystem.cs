using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShroomSniff.UI;
using VContainer;

namespace ShroomSniff.Gameplay
{
    public class TimerSystem : ITimerSystem
    {
        private readonly IGameUISystem _uiSystem;
        private readonly int _timeLimitSeconds;
        
        private int _currentTime;
        private bool _shouldPauseTimer;

        public event Action TimeUp;

        [Inject]
        public TimerSystem(IGameUISystem uiSystem, GameplayScopeConfiguration configuration)
        {
            _uiSystem = uiSystem;
            _timeLimitSeconds = configuration.timeLimitSeconds;
        }

        public int GetCurrentTime()
        {
            return _currentTime;
        }

        public void PauseTimer()
        {
            _shouldPauseTimer = true;
        }

        public void UnPauseTimer()
        {
            _shouldPauseTimer = false;
        }

        public void StartTimer(CancellationToken cancellationToken)
        {
            UpdateTimer(cancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        private async UniTask UpdateTimer(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
                
                _currentTime += 1;
                _uiSystem.UpdateTimerText(_currentTime);
                
                // Check if time limit is reached
                if (_timeLimitSeconds > 0 && _currentTime >= _timeLimitSeconds)
                {
                    StopGame();
                    return;
                }
                
                await UniTask.WaitUntil(() => _shouldPauseTimer == false, cancellationToken: cancellationToken);
                await UniTask.NextFrame();
            }
            
            StopGame();
        }

        private void StopGame()
        {
            UnityEngine.Debug.Log($"[{nameof(TimerSystem)}]: Timer stopped at {_currentTime} seconds.");
            
            if (_timeLimitSeconds > 0 && _currentTime >= _timeLimitSeconds)
            {
                UnityEngine.Debug.Log($"[{nameof(TimerSystem)}]: Time limit reached! Triggering TimeUp event.");
                TimeUp?.Invoke();
            }
        }
    }
}