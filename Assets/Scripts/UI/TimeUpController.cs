using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShroomSniff.UI
{
    public class TimeUpMenuController : MonoBehaviour
    {
        [SerializeField] private Button _goToMenuButton;
        [SerializeField] private TextMeshProUGUI _coinsEarnedText;

        public event System.Action GoToMenuClicked;

        private void Awake()
        {
            if (_goToMenuButton != null)
                _goToMenuButton.onClick.AddListener(OnGoToMenuClicked);
        }

        private void OnDestroy()
        {
            if (_goToMenuButton != null)
                _goToMenuButton.onClick.RemoveListener(OnGoToMenuClicked);
        }

        public void SetCoinsEarned(int coinsEarned)
        {
            if (_coinsEarnedText != null)
                _coinsEarnedText.text = $"Coins Earned: {coinsEarned}";
        }

        private void OnGoToMenuClicked()
        {
            GoToMenuClicked?.Invoke();
        }
    }
}