using System;
using System.Threading;

namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// System responsible for managing game timer
    /// </summary>
    public interface ITimerSystem
    {
        /// <summary>
        /// Event triggered when time limit is reached
        /// </summary>
        event Action TimeUp;
        
        /// <summary>
        /// Starts the timer with the specified cancellation token
        /// </summary>
        void StartTimer(CancellationToken cancellationToken);
        
        /// <summary>
        /// Pauses the timer
        /// </summary>
        void PauseTimer();
        
        /// <summary>
        /// Resumes the timer
        /// </summary>
        void UnPauseTimer();
        
        /// <summary>
        /// Gets the current elapsed time in seconds
        /// </summary>
        int GetCurrentTime();
    }
}