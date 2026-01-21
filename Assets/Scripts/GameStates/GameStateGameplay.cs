using System.Threading;
using ShroomSniff.Constants;
using ShroomSniff.Gameplay;
using ShroomSniff.UI;
using UnityEngine;
using VContainer;

namespace ShroomSniff
{
    public class GameStateGameplay : GameStateBase
    {
        private readonly GameplayScene _gameplayScenePrefab;
        private readonly ILevelSystem _levelSystem;
        private readonly IMushroomSystem _mushroomSystem;
        private readonly CameraConfiguration _cameraConfig;
        private readonly ITimerSystem _timerSystem;
        private readonly IPauseSystem _pauseSystem;
        private readonly IGameOverSystem _gameOverSystem;
        private readonly IGameUISystem _gameUISystem;
        private readonly UpgradeApplicationSystem _upgradeApplicationSystem;

        private GameplayScene _gameplaySceneInstance;
        private CancellationTokenSource _gameplayCts;
        
        [Inject]
        public GameStateGameplay(GameplayScene gameplayScene, 
            ILevelSystem levelSystem, 
            IMushroomSystem mushroomSystem, 
            CameraConfiguration cameraConfig, 
            ITimerSystem timerSystem,
            IPauseSystem pauseSystem,
            IGameOverSystem gameOverSystem,
            IGameUISystem gameUISystem,
            UpgradeApplicationSystem upgradeApplicationSystem) 
        {
            _gameplayScenePrefab = gameplayScene;
            _levelSystem = levelSystem;
            _mushroomSystem = mushroomSystem;
            _cameraConfig = cameraConfig;
            _timerSystem = timerSystem;
            _pauseSystem = pauseSystem;
            _gameOverSystem = gameOverSystem;
            _gameUISystem = gameUISystem;
            _upgradeApplicationSystem = upgradeApplicationSystem;
            
            _gameplayCts = new CancellationTokenSource();
        }

        public override void OnRun()
        {
            // Apply upgrades before starting gameplay
            _upgradeApplicationSystem.ApplyUpgradesToSaveData();
            
            // Ensure UI is instantiated and visible
            if (_gameUISystem.CurrentUIController == null)
            {
                _gameUISystem.InstantiateUI();
            }
            else
            {
                _gameUISystem.Show();
            }
            
            _gameplaySceneInstance = Object.Instantiate(_gameplayScenePrefab);
            _gameplaySceneInstance.name = Texts.GAMEPLAY_ROOT_NAME;
            _gameplaySceneInstance.Inject(_cameraConfig);
            
            // Inject camera controller into pause system
            if (_pauseSystem is PauseSystem pauseSystem)
            {
                pauseSystem.InjectCameraController(_gameplaySceneInstance.GetCameraController());
            }
            
            // Inject camera controller into game over system
            if (_gameOverSystem is GameOverSystem gameOverSystem)
            {
                gameOverSystem.InjectCameraController(_gameplaySceneInstance.GetCameraController());
            }
            
            _levelSystem.InjectGameplayScene(_gameplaySceneInstance);
            _levelSystem.GenerateLevel();

            _mushroomSystem.InjectGameplayScene(_gameplaySceneInstance);
            _mushroomSystem.SpawnMushrooms();
            
            _timerSystem.StartTimer(_gameplayCts.Token);
        }

        public override void OnDispose()
        {
            _gameplayCts.Cancel();
            Object.Destroy(_gameplaySceneInstance);
        }
    }
}