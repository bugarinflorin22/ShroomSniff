using UnityEngine;
using UnityEngine.UI;

namespace ShroomSniff.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _goToMenuButton;
        [SerializeField] private Button _quitGameButton;

        public event System.Action ContinueClicked;
        public event System.Action GoToMenuClicked;
        public event System.Action QuitGameClicked;

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(OnContinueClicked);

            if (_goToMenuButton != null)
                _goToMenuButton.onClick.AddListener(OnGoToMenuClicked);

            if (_quitGameButton != null)
                _quitGameButton.onClick.AddListener(OnQuitGameClicked);
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnContinueClicked);

            if (_goToMenuButton != null)
                _goToMenuButton.onClick.RemoveListener(OnGoToMenuClicked);

            if (_quitGameButton != null)
                _quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        }

        private void OnContinueClicked()
        {
            ContinueClicked?.Invoke();
        }

        private void OnGoToMenuClicked()
        {
            GoToMenuClicked?.Invoke();
        }

        private void OnQuitGameClicked()
        {
            QuitGameClicked?.Invoke();
        }
    }
}