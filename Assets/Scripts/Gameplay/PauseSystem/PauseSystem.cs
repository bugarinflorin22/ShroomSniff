using ShroomSniff.App;
using ShroomSniff.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace ShroomSniff.Gameplay
{
    public class PauseSystem : IPauseSystem, ITickable
    {
        private readonly GameplayScopeConfiguration _configuration;
        private readonly ITimerSystem _timerSystem;
        private readonly ScopeManager _scopeManager;
        private readonly IGameUISystem _gameUISystem;
        private readonly AppConfiguration _appConfiguration;

        private PauseMenuController _pauseMenuInstance;
        private CameraController _cameraController;
        private bool _isPaused;

        public bool IsPaused => _isPaused;

        [Inject]
        public PauseSystem(
            GameplayScopeConfiguration configuration, 
            ITimerSystem timerSystem, 
            ScopeManager scopeManager,
            IGameUISystem gameUISystem,
            AppConfiguration appConfiguration)
        {
            _configuration = configuration;
            _timerSystem = timerSystem;
            _scopeManager = scopeManager;
            _gameUISystem = gameUISystem;
            _appConfiguration = appConfiguration;
        }

        public void InjectCameraController(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void PauseGame()
        {
            if (_isPaused)
                return;

            _isPaused = true;
            Time.timeScale = 0f;
            _timerSystem.PauseTimer();
            
            // Disable camera movement
            if (_cameraController != null)
                _cameraController.SetEnabled(false);
            
            // Hide game UI
            _gameUISystem.Hide();
            
            ShowPauseMenu();
        }

        public void ResumeGame()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            Time.timeScale = 1f;
            _timerSystem.UnPauseTimer();
            
            // Re-enable camera movement
            if (_cameraController != null)
                _cameraController.SetEnabled(true);
            
            // Show game UI
            _gameUISystem.Show();
            
            HidePauseMenu();
        }

        public void TogglePause()
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        private void ShowPauseMenu()
        {
            if (_pauseMenuInstance != null)
                return;

            if (_configuration.pauseMenuPrefab == null)
            {
                Debug.LogError($"[{nameof(PauseSystem)}]: Pause menu prefab is not configured.");
                return;
            }

            _pauseMenuInstance = Object.Instantiate(_configuration.pauseMenuPrefab);
            _pauseMenuInstance.ContinueClicked += HandleContinue;
            _pauseMenuInstance.GoToMenuClicked += HandleGoToMenu;
            _pauseMenuInstance.QuitGameClicked += HandleQuitGame;
        }

        private void HidePauseMenu()
        {
            if (_pauseMenuInstance == null)
                return;

            _pauseMenuInstance.ContinueClicked -= HandleContinue;
            _pauseMenuInstance.GoToMenuClicked -= HandleGoToMenu;
            _pauseMenuInstance.QuitGameClicked -= HandleQuitGame;

            Object.Destroy(_pauseMenuInstance.gameObject);
            _pauseMenuInstance = null;
        }

        private void HandleContinue()
        {
            ResumeGame();
        }

        private void HandleGoToMenu()
        {
            if (_pauseMenuInstance != null)
            {
                _pauseMenuInstance.ContinueClicked -= HandleContinue;
                _pauseMenuInstance.GoToMenuClicked -= HandleGoToMenu;
                _pauseMenuInstance.QuitGameClicked -= HandleQuitGame;
                Object.Destroy(_pauseMenuInstance.gameObject);
                _pauseMenuInstance = null;
            }
            
            // Restore time scale before loading scene
            Time.timeScale = 1f;
            
            // Destroy the current GameplayScope before loading MetaScene
            if (_scopeManager.CurrentScope == ScopeManager.ScopeKind.Gameplay)
            {
                Debug.Log($"[{nameof(PauseSystem)}]: Destroying GameplayScope...");
                Object.Destroy(_scopeManager.GameScope.gameObject);
            }
            
            // Load meta scene and spawn meta scope when loaded
            Debug.Log($"[{nameof(PauseSystem)}]: Loading meta scene...");
            var loadOperation = SceneManager.LoadSceneAsync(_appConfiguration.MetaSceneName, LoadSceneMode.Single);
            loadOperation.completed += _ => OnMetaSceneLoaded();
        }

        private void OnMetaSceneLoaded()
        {
            Debug.Log($"[{nameof(PauseSystem)}]: Meta scene loaded, spawning MetaScope...");
            _scopeManager.SpawnMetaScope();
        }

        private void HandleQuitGame()
        {
            Debug.Log($"[{nameof(PauseSystem)}]: Quitting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
