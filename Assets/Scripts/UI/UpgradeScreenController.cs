using ShroomSniff.App;
using ShroomSniff.Data;
using ShroomSniff.Meta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.UI
{
    public class UpgradeScreenController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _changeTierButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _coinsText;
        
        [Header("Tier GameObjects")]
        [SerializeField] private GameObject _tier1Container;
        [SerializeField] private GameObject _tier2Container;
        [SerializeField] private GameObject _tier3Container;
        
        [Header("Upgrade Items")]
        [SerializeField] private UpgradeItemController[] _allUpgradeItems;
        
        [Header("References")]
        [SerializeField] private MetaMenuController _menuController;

        private IAppSaveSystem _saveSystem;
        private UpgradeTier _currentTier = UpgradeTier.Tier1;

        private void Start()
        {
            // Manually resolve dependencies from MetaScope
            var metaScope = LifetimeScope.Find<MetaScope>();
            if (metaScope != null)
            {
                _saveSystem = metaScope.Container.Resolve<IAppSaveSystem>();
                _saveSystem.SaveDataChanged += OnSaveDataChanged;
                
                InitializeUpgradeItems();
                UpdateCoinsDisplay();
            }
            else
            {
                Debug.LogError($"[{nameof(UpgradeScreenController)}]: Could not find MetaScope!");
            }
        }

        private void Awake()
        {
            if (_changeTierButton != null)
                _changeTierButton.onClick.AddListener(OnChangeTierClicked);
            
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);
            
            // Show Tier 1 by default
            ShowTier(UpgradeTier.Tier1);
        }

        private void OnDestroy()    
        {
            if (_changeTierButton != null)
                _changeTierButton.onClick.RemoveListener(OnChangeTierClicked);
            
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
            
            if (_saveSystem != null)
                _saveSystem.SaveDataChanged -= OnSaveDataChanged;
            
            UnsubscribeFromUpgradeItems();
        }

        private void InitializeUpgradeItems()
        {
            if (_allUpgradeItems == null)
                return;

            foreach (var item in _allUpgradeItems)
            {
                if (item == null)
                    continue;

                // Set purchased state from save data
                var isPurchased = _saveSystem.SaveData.upgradeData.IsUpgradePurchased(item.UpgradeId);
                item.SetPurchased(isPurchased);
                
                // Subscribe to purchase event
                item.PurchaseClicked += OnUpgradePurchaseClicked;
            }
            
            UpdateUpgradeInteractability();
        }

        private void UnsubscribeFromUpgradeItems()
        {
            if (_allUpgradeItems == null)
                return;

            foreach (var item in _allUpgradeItems)
            {
                if (item != null)
                    item.PurchaseClicked -= OnUpgradePurchaseClicked;
            }
        }

        private void OnUpgradePurchaseClicked(UpgradeItemController item)
        {
            var coins = _saveSystem.SaveData.coins;
            var cost = item.Cost;

            // Check if player has enough coins
            if (coins >= cost)
            {
                // Spend coins
                _saveSystem.SpendCoins(cost);
                
                // Mark upgrade as purchased
                _saveSystem.UpdateUpgradeData(data =>
                {
                    data.PurchaseUpgrade(item.UpgradeId, cost);
                });
                
                // Update visuals
                item.SetPurchased(true);
                UpdateUpgradeInteractability();
                
                Debug.Log($"[{nameof(UpgradeScreenController)}]: Purchased upgrade '{item.UpgradeId}' for {cost} coins");
            }
            else
            {
                Debug.Log($"[{nameof(UpgradeScreenController)}]: Not enough coins! Need {cost}, have {coins}");
            }
        }

        private void UpdateUpgradeInteractability()
        {
            if (_allUpgradeItems == null)
                return;

            var coins = _saveSystem.SaveData.coins;
            
            foreach (var item in _allUpgradeItems)
            {
                if (item != null)
                    item.SetInteractable(coins >= item.Cost);
            }
        }

        private void OnSaveDataChanged(AppSaveData saveData)
        {
            UpdateCoinsDisplay();
            UpdateUpgradeInteractability();
        }

        private void UpdateCoinsDisplay()
        {
            if (_coinsText != null && _saveSystem != null)
            {
                _coinsText.text = _saveSystem.SaveData.coins.ToString();
            }
        }

        private void OnChangeTierClicked()
        {
            // Cycle through tiers: Tier1 -> Tier2 -> Tier3 -> Tier1
            _currentTier = _currentTier switch
            {
                UpgradeTier.Tier1 => UpgradeTier.Tier2,
                UpgradeTier.Tier2 => UpgradeTier.Tier3,
                UpgradeTier.Tier3 => UpgradeTier.Tier1,
                _ => UpgradeTier.Tier1
            };
            
            ShowTier(_currentTier);
        }

        private void ShowTier(UpgradeTier tier)
        {
            // Hide all tiers first
            if (_tier1Container != null)
                _tier1Container.SetActive(false);
            
            if (_tier2Container != null)
                _tier2Container.SetActive(false);
            
            if (_tier3Container != null)
                _tier3Container.SetActive(false);
            
            // Show only the requested tier
            switch (tier)
            {
                case UpgradeTier.Tier1:
                    if (_tier1Container != null)
                        _tier1Container.SetActive(true);
                    break;
                    
                case UpgradeTier.Tier2:
                    if (_tier2Container != null)
                        _tier2Container.SetActive(true);
                    break;
                    
                case UpgradeTier.Tier3:
                    if (_tier3Container != null)
                        _tier3Container.SetActive(true);
                    break;
            }
            
            Debug.Log($"[{nameof(UpgradeScreenController)}]: Showing Tier {(int)tier}");
        }
        
        private void OnBackClicked()
        {
            Debug.Log($"[{nameof(UpgradeScreenController)}]: Back button clicked");
            
            if (_menuController != null)
            {
                _menuController.ShowMenu();
            }
            else
            {
                Debug.LogError($"[{nameof(UpgradeScreenController)}]: Menu controller reference is null!");
            }
        }
    }
}
