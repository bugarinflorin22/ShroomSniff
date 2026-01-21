using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShroomSniff.UI
{
    public class UpgradeItemController : MonoBehaviour
    {
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _upgradeImage; // Image that will fade out when purchased
        [SerializeField] private GameObject _purchasedIndicator; // Optional: show checkmark or "Owned" text
        
        [Header("Upgrade Info")]
        [SerializeField] private string _upgradeId;
        [SerializeField] private int _cost;

        private bool _isPurchased;

        public string UpgradeId => _upgradeId;
        public int Cost => _cost;
        public bool IsPurchased => _isPurchased;

        public event System.Action<UpgradeItemController> PurchaseClicked;

        private void Awake()
        {
            if (_purchaseButton != null)
                _purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
            
            if (_costText != null)
                _costText.text = _cost.ToString();
        }

        private void OnDestroy()
        {
            if (_purchaseButton != null)
                _purchaseButton.onClick.RemoveListener(OnPurchaseButtonClicked);
        }

        private void OnPurchaseButtonClicked()
        {
            if (!_isPurchased)
            {
                PurchaseClicked?.Invoke(this);
            }
        }

        public void SetPurchased(bool purchased)
        {
            _isPurchased = purchased;
            UpdateVisuals();
        }

        public void SetInteractable(bool interactable)
        {
            if (_purchaseButton != null)
                _purchaseButton.interactable = interactable && !_isPurchased;
        }

        private void UpdateVisuals()
        {
            if (_purchaseButton != null)
                _purchaseButton.interactable = !_isPurchased;
            
            if (_purchasedIndicator != null)
                _purchasedIndicator.SetActive(_isPurchased);
            
            // Set image color to white when purchased
            if (_upgradeImage != null)
            {
                _upgradeImage.color = _isPurchased ? Color.white : _upgradeImage.color;
            }
        }
    }
}
