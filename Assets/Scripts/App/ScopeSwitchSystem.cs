using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.App
{
    public class ScopeSwitchSystem : ITickable
    {
        private const KeyCode GameplayKey = KeyCode.G;
        private const KeyCode MetaKey = KeyCode.M;
        private const KeyCode ToggleKey = KeyCode.Tab;

        private readonly ScopeManager _scopeManager;

        [Inject]
        public ScopeSwitchSystem(ScopeManager scopeManager)
        {
            _scopeManager = scopeManager;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(MetaKey))
            {
                _scopeManager.SpawnMetaScope();
                return;
            }

            if (Input.GetKeyDown(GameplayKey))
            {
                _scopeManager.SpawnGameScope();
                return;
            }

            if (Input.GetKeyDown(ToggleKey))
                ToggleScope();
        }

        private void ToggleScope()
        {
            if (_scopeManager.CurrentScope == ScopeManager.ScopeKind.Gameplay)
            {
                _scopeManager.SpawnMetaScope();
            }
            else
            {
                _scopeManager.SpawnGameScope();
            }
        }
    }
}
