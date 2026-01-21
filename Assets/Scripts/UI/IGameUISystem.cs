namespace ShroomSniff.UI
{
    public interface IGameUISystem
    {
        GameUIController CurrentUIController { get; }
        void InstantiateUI();
        void Dispose();
        void UpdateTimerText(int seconds);
        void Show();
        void Hide();
    }
}