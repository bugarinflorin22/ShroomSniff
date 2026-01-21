using System;
using ShroomSniff.UI;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.App
{
    public class AppManager : IStartable, IDisposable
    {
        private readonly ScopeManager _scopeManager;
        private readonly IGameUISystem _gameUISystem;
        private readonly IAudioSystem _audioSystem;
        private readonly IAppSaveSystem _saveSystem;

        [Inject]
        public AppManager(ScopeManager scopeManager, 
            IGameUISystem gameUISystem,
            IAudioSystem audioSystem,
            IAppSaveSystem saveSystem)
        {
            _scopeManager = scopeManager;
            _gameUISystem = gameUISystem;
            _audioSystem = audioSystem;
            _saveSystem = saveSystem;
        }

        public void Start()
        {
            _audioSystem.Initialize();
            _saveSystem.Load();
            _scopeManager.SpawnGameScope();
            _gameUISystem.InstantiateUI();
        }
        
        public void Dispose()
        {
            _audioSystem.Dispose();
            _gameUISystem.Dispose();
        }
    }
}