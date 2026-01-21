using ShroomSniff.Configurations;
using ShroomSniff.UI;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    [CreateAssetMenu(fileName = "GameplayScopeConfiguration", menuName = "ShroomSniff/Gameplay Scope Configuration")]
    public class GameplayScopeConfiguration : ScriptableObject
    {
        [Header("Scene & Camera")]
        public GameplayScene gameplayScene;
        public CameraConfiguration cameraConfiguration;
        public CursorConfiguration cursorConfiguration;
        
        [Header("Gameplay")]
        public LevelConfiguration levelConfiguration;
        public MushroomConfiguration mushroomConfiguration;
        public CoinRewardConfiguration coinRewardConfiguration;
        
        [Header("UI Prefabs")]
        public PauseMenuController pauseMenuPrefab;
        public TimeUpMenuController timeUpMenuPrefab;
        
        [Header("Game Settings")]
        [Tooltip("Time limit in seconds. Set to 0 for no limit.")]
        public int timeLimitSeconds = 15;
    }
}