using ShroomSniff.Gameplay;
using ShroomSniff.Meta;
using UnityEngine;
using VContainer.Unity;
using VContainer;
using Object = UnityEngine.Object;

namespace ShroomSniff.App
{
    /// <summary>
    /// Controls which scope (Game or Base) is spawned as a child
    /// Only one scope exists at a time - spawned when needed, destroyed when switching
    /// </summary>
    public class ScopeManager
    {
        public enum ScopeKind
        {
            None,
            Gameplay,
            Meta
        }

        private readonly AppConfiguration _configuration;
        private readonly LifetimeScope _appScope;
        
        private LifetimeScope _currentScope;
        private GameplayScope _gameScope;
        private MetaScope _metaScope;
        private ScopeKind _currentScopeKind = ScopeKind.None;
        
        public GameplayScope GameScope => _gameScope;
        public MetaScope MetaScope => _metaScope;
        public ScopeKind CurrentScope => _currentScopeKind;
        
        [Inject]
        public ScopeManager(AppConfiguration configuration, LifetimeScope appScope)
        {
            _configuration = configuration;
            _appScope = appScope;
        }
        
        /// <summary>
        /// Spawns the gameplay scope, destroying any existing scope
        /// </summary>
        public void SpawnGameScope()
        {
            if (_currentScopeKind == ScopeKind.Gameplay)
                return;

            DestroyCurrentScope();
            
            // Use EnqueueParent to set parent context before instantiation
            using (LifetimeScope.EnqueueParent(_appScope))
            {
                _gameScope = Object.Instantiate(_configuration.GameplayScopePrefab, _appScope.transform);
                _gameScope.gameObject.SetActive(true);
            }
            
            _currentScope = _gameScope;
            _currentScopeKind = ScopeKind.Gameplay;
            
            Debug.Log($"[{nameof(ScopeManager)}]: Scope just changed in {nameof(GameScope)}");
        }
        
        /// <summary>
        /// Spawns the meta scope, destroying any existing scope
        /// </summary>
        public void SpawnMetaScope()
        {
            if (_currentScopeKind == ScopeKind.Meta)
                return;

            DestroyCurrentScope();
            
            // Use EnqueueParent to set parent context before instantiation
            using (LifetimeScope.EnqueueParent(_appScope))
            {
                _metaScope = Object.Instantiate(_configuration.MetaScopePrefab, _appScope.transform);
                _metaScope.gameObject.SetActive(true);
            }
            
            _currentScope = _metaScope;
            _currentScopeKind = ScopeKind.Meta;
            
            Debug.Log($"[{nameof(ScopeManager)}]: Scope just changed in {nameof(MetaScope)}");
        }
        
        private void DestroyCurrentScope()
        {
            if (_currentScope != null)
            {
                Debug.Log($"[{nameof(ScopeManager)}]: Destroying {_currentScope.GetType().Name}");
                Object.Destroy(_currentScope.gameObject);
                _currentScope = null;
                _gameScope = null;
                _metaScope = null;
                _currentScopeKind = ScopeKind.None;
            }
        }
    }
}
