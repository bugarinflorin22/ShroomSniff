using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// Gameplay scope - Manages game-specific dependencies (player, enemies, etc.)
    /// </summary>
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private GameplayScopeConfiguration _config;

        public GameplayScopeConfiguration Configuration => _config;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Register configuration
            builder.RegisterInstance(_config);
            
            // Register coin reward configuration if available
            if (_config.coinRewardConfiguration != null)
                builder.RegisterInstance(_config.coinRewardConfiguration);
            
            // Register mushroom configuration for upgrade system
            if (_config.mushroomConfiguration != null)
                builder.RegisterInstance(_config.mushroomConfiguration);
            
            // Register upgrade application system
            builder.Register<UpgradeApplicationSystem>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<CursorSystem>()
                .WithParameter(_config.cursorConfiguration)
                .As<ICursorSystem>();

            builder.Register<TimerSystem>(Lifetime.Singleton)
                .WithParameter(_config)
                .As<ITimerSystem>();

            builder.Register<LevelSystem>(Lifetime.Singleton)
                .WithParameter(_config.levelConfiguration)
                .As<ILevelSystem>();

            builder.Register<MushroomSystem>(Lifetime.Singleton)
                .WithParameter(_config.mushroomConfiguration)
                .As<IMushroomSystem>();

            builder.RegisterEntryPoint<GameOverSystem>()
                .WithParameter(_config)
                .As<IGameOverSystem>();

            builder.RegisterEntryPoint<PauseSystem>()
                .WithParameter(_config)
                .As<IPauseSystem>();
            
            builder.RegisterEntryPoint<GameStateGameplay>()
                .WithParameter(_config.gameplayScene)
                .WithParameter(_config.cameraConfiguration)
                .As<IGameStateBase>();
        }
    }
}