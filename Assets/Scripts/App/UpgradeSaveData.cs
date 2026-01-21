using System.Collections.Generic;

namespace ShroomSniff.App
{
    [System.Serializable]
    public class UpgradeSaveData
    {
        public List<string> purchasedUpgradeIds = new List<string>();
        public int currentCurrency = 0;

        public bool IsUpgradePurchased(string upgradeId)
        {
            return purchasedUpgradeIds.Contains(upgradeId);
        }

        public void PurchaseUpgrade(string upgradeId, int cost)
        {
            if (!purchasedUpgradeIds.Contains(upgradeId))
            {
                purchasedUpgradeIds.Add(upgradeId);
                currentCurrency -= cost;
            }
        }

        public void AddCurrency(int amount)
        {
            currentCurrency += amount;
        }
    }
}
