using System;
using ShroomSniff.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.App
{
    public class AppScope : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private AppConfiguration _appConfiguration;

        private static AppScope _instance;

        protected override void Awake()
        {
            // Ensure only one AppScope exists
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_appConfiguration);
            builder.Register<ScopeManager>(Lifetime.Singleton)
                .WithParameter(this);

            builder.Register<AppSaveSystem>(Lifetime.Singleton)
                .As<IAppSaveSystem>();

            builder.Register<GameUISystem>(Lifetime.Singleton)
                .WithParameter(_appConfiguration.GameUIController)
                .As<IGameUISystem>();

            builder.Register<AudioSystem>(Lifetime.Singleton)
                .As<IAudioSystem>();

            builder.RegisterEntryPoint<ScopeSwitchSystem>();
            builder.RegisterEntryPoint<AppManager>();
        }
    }
}