using System;
using ShroomSniff.App;
using ShroomSniff.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace ShroomSniff.Gameplay
{
    public class GameOverSystem : IGameOverSystem, IStartable, IDisposable
    {
        private readonly GameplayScopeConfiguration _configuration;
        private readonly ITimerSystem _timerSystem;
        private readonly IMushroomSystem _mushroomSystem;
        private readonly IGameUISystem _gameUISystem;
        private readonly IPauseSystem _pauseSystem;
        private readonly AppConfiguration _appConfiguration;
        private readonly ScopeManager _scopeManager;

        private TimeUpMenuController _timeUpMenuInstance;
        private CameraController _cameraController;

        public event Action TimeIsUp;

        [Inject]
        public GameOverSystem(
            GameplayScopeConfiguration configuration,
            ITimerSystem timerSystem,
            IMushroomSystem mushroomSystem,
            IGameUISystem gameUISystem,
            IPauseSystem pauseSystem,
            AppConfiguration appConfiguration,
            ScopeManager scopeManager)
        {
            _configuration = configuration;
            _timerSystem = timerSystem;
            _mushroomSystem = mushroomSystem;
            _gameUISystem = gameUISystem;
            _pauseSystem = pauseSystem;
            _appConfiguration = appConfiguration;
            _scopeManager = scopeManager;
        }

        public void InjectCameraController(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void Start()
        {
            // Subscribe to timer's TimeUp event
            _timerSystem.TimeUp += HandleTimeUp;
            
            // Subscribe to mushroom system's AllMushroomsCollected event
            _mushroomSystem.AllMushroomsCollected += HandleAllMushroomsCollected;
        }

        public void Dispose()
        {
            // Unsubscribe from timer's TimeUp event
            if (_timerSystem != null)
                _timerSystem.TimeUp -= HandleTimeUp;
            
            // Unsubscribe from mushroom system's AllMushroomsCollected event
            if (_mushroomSystem != null)
                _mushroomSystem.AllMushroomsCollected -= HandleAllMushroomsCollected;
        }

        private void HandleTimeUp()
        {
            ShowTimeUpScreen();
        }

        private void HandleAllMushroomsCollected()
        {
            ShowTimeUpScreen();
        }

        public void ShowTimeUpScreen()
        {
            if (_timeUpMenuInstance != null)
                return;

            if (_configuration.timeUpMenuPrefab == null)
            {
                Debug.LogError($"[{nameof(GameOverSystem)}]: Time up menu prefab is not configured.");
                return;
            }

            // Stop game time
            Time.timeScale = 0f;
            
            // Disable camera movement
            if (_cameraController != null)
                _cameraController.SetEnabled(false);
            
            // Disable all mushroom interactions
            DisableAllMushrooms();

            // Hide game UI
            _gameUISystem.Hide();

            // Instantiate time up menu
            _timeUpMenuInstance = Object.Instantiate(_configuration.timeUpMenuPrefab);
            
            // Set coins earned text
            int coinsEarned = _mushroomSystem.GetCoinsEarnedThisRound();
            _timeUpMenuInstance.SetCoinsEarned(coinsEarned);
            
            _timeUpMenuInstance.GoToMenuClicked += HandleGoToMenu;

            TimeIsUp?.Invoke();
        }

        private void HandleGoToMenu()
        {
            if (_timeUpMenuInstance != null)
            {
                _timeUpMenuInstance.GoToMenuClicked -= HandleGoToMenu;
                Object.Destroy(_timeUpMenuInstance.gameObject);
                _timeUpMenuInstance = null;
            }

            // Restore time scale before loading scene
            Time.timeScale = 1f;
            
            // Destroy the current GameplayScope before loading MetaScene
            if (_scopeManager.CurrentScope == ScopeManager.ScopeKind.Gameplay)
            {
                Debug.Log($"[{nameof(GameOverSystem)}]: Destroying GameplayScope...");
                Object.Destroy(_scopeManager.GameScope.gameObject);
            }
            
            // Load meta scene and spawn meta scope when loaded
            Debug.Log($"[{nameof(GameOverSystem)}]: Loading meta scene...");
            var loadOperation = SceneManager.LoadSceneAsync(_appConfiguration.MetaSceneName, LoadSceneMode.Single);
            loadOperation.completed += _ => OnMetaSceneLoaded();
        }

        private void OnMetaSceneLoaded()
        {
            Debug.Log($"[{nameof(GameOverSystem)}]: Meta scene loaded, spawning MetaScope...");
            _scopeManager.SpawnMetaScope();
        }

        private void DisableAllMushrooms()
        {
            var allMushrooms = Object.FindObjectsOfType<MushroomController>();
            foreach (var mushroom in allMushrooms)
            {
                mushroom.SetEnabled(false);
            }
            
            Debug.Log($"[{nameof(GameOverSystem)}]: Disabled {allMushrooms.Length} mushrooms.");
        }
    }
}
