namespace ShroomSniff
{
    public class GameStateBase : IGameStateBase
    {
        public void Start()
        {
            OnRun();
        }
        
        public void Dispose()
        {
            OnDispose();
        }

        public virtual void OnRun()
        {
            
        }

        public virtual void OnDispose()
        {
            
        }

    }
}