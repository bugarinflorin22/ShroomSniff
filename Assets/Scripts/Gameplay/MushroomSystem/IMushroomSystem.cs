namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// System responsible for spawning and managing mushrooms in the level
    /// </summary>
    public interface IMushroomSystem
    {
        /// <summary>
        /// Event triggered when all mushrooms have been collected
        /// </summary>
        event System.Action AllMushroomsCollected;
        
        /// <summary>
        /// Injects the gameplay scene reference for accessing level root
        /// </summary>
        void InjectGameplayScene(GameplayScene gameplayScene);
        
        /// <summary>
        /// Spawns mushrooms on all tiles based on configuration and save settings
        /// </summary>
        void SpawnMushrooms();
        
        /// <summary>
        /// Gets the total number of mushrooms spawned
        /// </summary>
        int GetTotalMushroomCount();
        
        /// <summary>
        /// Gets the number of mushrooms collected
        /// </summary>
        int GetCollectedMushroomCount();
        
        /// <summary>
        /// Gets the total coins earned in the current round
        /// </summary>
        int GetCoinsEarnedThisRound();
    }
}