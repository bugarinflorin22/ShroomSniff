using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace ShroomSniff.UI
{
    public class GameUISystem : IGameUISystem
    {
        private readonly GameUIController _gameUIControllerPrefab;
        private readonly IObjectResolver _resolver;
        
        private GameUIController _gameUIController;

        public GameUIController CurrentUIController => _gameUIController;
        
        [Inject]
        public GameUISystem(GameUIController gameUIController, IObjectResolver resolver)
        {
            _gameUIControllerPrefab = gameUIController;
            _resolver = resolver;
        }

        public void InstantiateUI()
        {
            _gameUIController = Object.Instantiate(_gameUIControllerPrefab);
            _resolver.InjectGameObject(_gameUIController.gameObject);
        }

        public void UpdateTimerText(int seconds)
        {
            if (_gameUIController != null)
                _gameUIController.SetTimerText(seconds.ToString());
        }

        public void Show()
        {
            if (_gameUIController != null)
                _gameUIController.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_gameUIController != null)
                _gameUIController.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            if (_gameUIController != null)
                Object.Destroy(_gameUIController.gameObject);

            _gameUIController = null;
        }
    }
}