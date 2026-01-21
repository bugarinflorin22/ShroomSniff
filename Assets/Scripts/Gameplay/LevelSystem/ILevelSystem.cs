namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// System responsible for generating and managing the level grid
    /// </summary>
    public interface ILevelSystem
    {
        /// <summary>
        /// Injects the gameplay scene reference for accessing level root
        /// </summary>
        void InjectGameplayScene(GameplayScene gameplayScene);
        
        /// <summary>
        /// Generates the level grid based on configuration and save settings
        /// </summary>
        void GenerateLevel();
    }
}