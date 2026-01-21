using ShroomSniff.App;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.Meta
{
    public class MetaMenuController : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _resetSaveButton;
        [SerializeField] private GameObject _upgradeScreen;
        [SerializeField] private GameObject _menuScreen;

        private AppConfiguration _appConfiguration;
        private ScopeManager _scopeManager;
        private IAppSaveSystem _saveSystem;

        private void Start()
        {
            // Get dependencies from the parent scope
            var metaScope = LifetimeScope.Find<MetaScope>();
            if (metaScope != null)
            {
                _appConfiguration = metaScope.Container.Resolve<AppConfiguration>();
                _scopeManager = metaScope.Container.Resolve<ScopeManager>();
                _saveSystem = metaScope.Container.Resolve<IAppSaveSystem>();
            }
        }

        private void Awake()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayClicked);
            
            if (_upgradeButton != null)
                _upgradeButton.onClick.AddListener(OnUpgradeClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);
            
            if (_resetSaveButton != null)
                _resetSaveButton.onClick.AddListener(OnResetSaveClicked);
            
            // Hide upgrade screen by default
            if (_upgradeScreen != null)
                _upgradeScreen.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlayClicked);
            
            if (_upgradeButton != null)
                _upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);
            
            if (_resetSaveButton != null)
                _resetSaveButton.onClick.RemoveListener(OnResetSaveClicked);
        }

        private void OnPlayClicked()
        {
            Debug.Log($"[{nameof(MetaMenuController)}]: Play button clicked, loading gameplay scene...");
            
            // Destroy MetaScope before loading gameplay scene
            if (_scopeManager.CurrentScope == ScopeManager.ScopeKind.Meta)
            {
                Destroy(_scopeManager.MetaScope.gameObject);
            }
            
            // Load gameplay scene
            var loadOperation = SceneManager.LoadSceneAsync(_appConfiguration.GameplaySceneName, LoadSceneMode.Single);
            loadOperation.completed += _ => OnGameplaySceneLoaded();
        }

        private void OnGameplaySceneLoaded()
        {
            Debug.Log($"[{nameof(MetaMenuController)}]: Gameplay scene loaded, spawning GameplayScope...");
            _scopeManager.SpawnGameScope();
        }

        private void OnUpgradeClicked()
        {
            if (_upgradeScreen != null)
            {
                bool isActive = _upgradeScreen.activeSelf;
                _upgradeScreen.SetActive(!isActive);
                
                // Hide menu screen when showing upgrade screen
                if (_menuScreen != null)
                    _menuScreen.SetActive(isActive);
                
                Debug.Log($"[{nameof(MetaMenuController)}]: Upgrade screen {(!isActive ? "opened" : "closed")}");
            }
        }

        public void ShowMenu()
        {
            Debug.Log($"[{nameof(MetaMenuController)}]: ShowMenu called");
            
            if (_menuScreen != null)
            {
                _menuScreen.SetActive(true);
                Debug.Log($"[{nameof(MetaMenuController)}]: Menu screen activated");
            }
            else
            {
                Debug.LogError($"[{nameof(MetaMenuController)}]: Menu screen is null!");
            }
            
            if (_upgradeScreen != null)
            {
                _upgradeScreen.SetActive(false);
                Debug.Log($"[{nameof(MetaMenuController)}]: Upgrade screen deactivated");
            }
        }

        private void OnQuitClicked()
        {
            Debug.Log($"[{nameof(MetaMenuController)}]: Quit button clicked");
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnResetSaveClicked()
        {
            Debug.Log($"[{nameof(MetaMenuController)}]: Reset save button clicked");
            
            if (_saveSystem != null)
            {
                _saveSystem.ResetToDefaults();
                Debug.Log($"[{nameof(MetaMenuController)}]: All save data has been reset to defaults");
            }
            else
            {
                Debug.LogError($"[{nameof(MetaMenuController)}]: SaveSystem is null, cannot reset!");
            }
        }
    }
}
