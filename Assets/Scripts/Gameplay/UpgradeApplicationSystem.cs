using ShroomSniff.App;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// Applies purchased upgrades to gameplay settings
    /// </summary>
    public class UpgradeApplicationSystem
    {
        private readonly IAppSaveSystem _saveSystem;
        private readonly MushroomConfiguration _mushroomConfig;

        // Upgrade IDs
        private const string TIER1_UNLOCK_MUSHROOM = "tier1_unlock_mushroom";
        private const string TIER1_INCREASE_SPEED = "tier1_increase_speed";
        private const string TIER1_ADD_TILE = "tier1_add_tile";
        
        private const string TIER2_UNLOCK_MUSHROOM = "tier2_unlock_mushroom";
        private const string TIER2_INCREASE_SPEED = "tier2_increase_speed";
        private const string TIER2_ADD_TILE = "tier2_add_tile";
        private const string TIER2_BIG_MUSHROOM_CHANCE = "tier2_big_mushroom_chance";
        
        private const string TIER3_UNLOCK_MUSHROOM_1 = "tier3_unlock_mushroom_1";
        private const string TIER3_UNLOCK_MUSHROOM_2 = "tier3_unlock_mushroom_2";
        private const string TIER3_INCREASE_SPEED = "tier3_increase_speed";
        private const string TIER3_ADD_3_TILES = "tier3_add_3_tiles";
        private const string TIER3_BIG_MUSHROOM_CHANCE = "tier3_big_mushroom_chance";

        public UpgradeApplicationSystem(IAppSaveSystem saveSystem, MushroomConfiguration mushroomConfiguration)
        {
            _saveSystem = saveSystem;
            _mushroomConfig = mushroomConfiguration;
        }

        /// <summary>
        /// Applies all purchased upgrades to the save data before gameplay starts
        /// </summary>
        public void ApplyUpgradesToSaveData()
        {
            var upgradeData = _saveSystem.SaveData.upgradeData;
            
            // Apply tile upgrades
            int bonusTiles = 0;
            if (upgradeData.IsUpgradePurchased(TIER1_ADD_TILE)) bonusTiles += 1;
            if (upgradeData.IsUpgradePurchased(TIER2_ADD_TILE)) bonusTiles += 1;
            if (upgradeData.IsUpgradePurchased(TIER3_ADD_3_TILES)) bonusTiles += 3;
            
            if (bonusTiles > 0)
            {
                _saveSystem.UpdateLevelSettings(settings =>
                {
                    settings.tileCount += bonusTiles;
                    Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: Added {bonusTiles} bonus tiles. Total: {settings.tileCount}");
                });
            }
            
            // Apply speed upgrades (each adds 10% speed)
            float speedMultiplier = 1f;
            if (upgradeData.IsUpgradePurchased(TIER1_INCREASE_SPEED)) speedMultiplier += 0.1f;
            if (upgradeData.IsUpgradePurchased(TIER2_INCREASE_SPEED)) speedMultiplier += 0.1f;
            if (upgradeData.IsUpgradePurchased(TIER3_INCREASE_SPEED)) speedMultiplier += 0.1f;
            
            if (speedMultiplier > 1f)
            {
                // Store speed multiplier (you'll need to apply this to camera speed)
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: Speed multiplier: {speedMultiplier}x");
            }
            
            // Apply mushroom category unlocks
            var unlockedCategories = GetUnlockedCategories();
            _saveSystem.UpdateMushroomSettings(settings =>
            {
                // Rebuild full category list from config, then filter to unlocked only
                var allCategories = new System.Collections.Generic.List<MushroomCategoryChanceEntry>();
                
                if (_mushroomConfig != null && _mushroomConfig.mushroomCategoryChances != null)
                {
                    foreach (var pair in _mushroomConfig.mushroomCategoryChances)
                    {
                        allCategories.Add(new MushroomCategoryChanceEntry
                        {
                            category = pair.Key,
                            value = pair.Value.value
                        });
                    }
                }
                
                // Filter to only include unlocked categories
                var filteredChances = new System.Collections.Generic.List<MushroomCategoryChanceEntry>();
                foreach (var entry in allCategories)
                {
                    if (unlockedCategories.Contains(entry.category))
                    {
                        filteredChances.Add(entry);
                    }
                }
                settings.mushroomCategoryChances = filteredChances;
                
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: Unlocked categories: {string.Join(", ", unlockedCategories)}");
            });
            
            // Apply big mushroom chance upgrades (each adds 10%)
            float bigMushroomBonus = 0f;
            if (upgradeData.IsUpgradePurchased(TIER2_BIG_MUSHROOM_CHANCE)) bigMushroomBonus += 0.1f;
            if (upgradeData.IsUpgradePurchased(TIER3_BIG_MUSHROOM_CHANCE)) bigMushroomBonus += 0.1f;
            
            if (bigMushroomBonus > 0f)
            {
                _saveSystem.UpdateMushroomSettings(settings =>
                {
                    // Increase big mushroom chances
                    for (int i = 0; i < settings.mushroomTypeChances.Count; i++)
                    {
                        var entry = settings.mushroomTypeChances[i];
                        if (entry.type == MushroomType.BigShroom)
                        {
                            entry.value += (int)(bigMushroomBonus * 100);
                            settings.mushroomTypeChances[i] = entry;
                        }
                    }
                    
                    Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: Increased big mushroom chance by {bigMushroomBonus * 100}%");
                });
            }
        }

        /// <summary>
        /// Gets list of unlocked mushroom categories based on purchased upgrades
        /// </summary>
        public System.Collections.Generic.List<MushroomCategory> GetUnlockedCategories()
        {
            var upgradeData = _saveSystem.SaveData.upgradeData;
            var categories = new System.Collections.Generic.List<MushroomCategory>();
            
            // P1 is always unlocked by default
            categories.Add(MushroomCategory.P1);
            
            // Tier 1: Unlock P2
            if (upgradeData.IsUpgradePurchased(TIER1_UNLOCK_MUSHROOM))
            {
                categories.Add(MushroomCategory.P2);
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: P2 unlocked via {TIER1_UNLOCK_MUSHROOM}");
            }
            
            // Tier 2: Unlock P3
            if (upgradeData.IsUpgradePurchased(TIER2_UNLOCK_MUSHROOM))
            {
                categories.Add(MushroomCategory.P3);
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: P3 unlocked via {TIER2_UNLOCK_MUSHROOM}");
            }
            else
            {
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: P3 NOT unlocked. Looking for '{TIER2_UNLOCK_MUSHROOM}'. Purchased upgrades: {string.Join(", ", upgradeData.purchasedUpgradeIds)}");
            }
            
            // Tier 3: Unlock P4 and P5
            if (upgradeData.IsUpgradePurchased(TIER3_UNLOCK_MUSHROOM_1))
            {
                categories.Add(MushroomCategory.P4);
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: P4 unlocked via {TIER3_UNLOCK_MUSHROOM_1}");
            }
            
            if (upgradeData.IsUpgradePurchased(TIER3_UNLOCK_MUSHROOM_2))
            {
                categories.Add(MushroomCategory.P5);
                Debug.Log($"[{nameof(UpgradeApplicationSystem)}]: P5 unlocked via {TIER3_UNLOCK_MUSHROOM_2}");
            }
            
            return categories;
        }

        /// <summary>
        /// Gets the total speed multiplier from upgrades
        /// </summary>
        public float GetSpeedMultiplier()
        {
            var upgradeData = _saveSystem.SaveData.upgradeData;
            float multiplier = 1f;
            
            if (upgradeData.IsUpgradePurchased(TIER1_INCREASE_SPEED)) multiplier += 0.1f;
            if (upgradeData.IsUpgradePurchased(TIER2_INCREASE_SPEED)) multiplier += 0.1f;
            if (upgradeData.IsUpgradePurchased(TIER3_INCREASE_SPEED)) multiplier += 0.1f;
            
            return multiplier;
        }
    }
}
