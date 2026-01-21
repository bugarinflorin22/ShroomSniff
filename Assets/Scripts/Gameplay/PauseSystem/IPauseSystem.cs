namespace ShroomSniff.Gameplay
{
    public interface IPauseSystem
    {
        bool IsPaused { get; }
        void PauseGame();
        void ResumeGame();
        void TogglePause();
    }
}