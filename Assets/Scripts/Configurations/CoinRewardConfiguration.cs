using ShroomSniff.Data;
using SerializableDictionary;
using UnityEngine;

namespace ShroomSniff.Configurations
{
    [System.Serializable]
    public class CoinRewardRange
    {
        public int minCoins = 1;
        public int maxCoins = 5;

        public int GetRandomAmount()
        {
            return Random.Range(minCoins, maxCoins + 1);
        }
    }

    [CreateAssetMenu(fileName = "CoinRewardConfiguration", menuName = "ShroomSniff/Coin Reward Configuration")]
    public class CoinRewardConfiguration : ScriptableObject
    {
        [Header("Rewards by Mushroom Category")]
        public SerializableDictionaryBase<MushroomCategory, CoinRewardRange> categoryRewards;

        [Header("Rewards by Mushroom Type")]
        public SerializableDictionaryBase<MushroomType, CoinRewardRange> typeRewards;

        [Header("Default Reward")]
        public CoinRewardRange defaultReward = new CoinRewardRange { minCoins = 1, maxCoins = 3 };

        public int GetCoinReward(MushroomCategory category, MushroomType type)
        {
            if (typeRewards != null && typeRewards.TryGetValue(type, out var typeReward))
                return typeReward.GetRandomAmount();

            if (categoryRewards != null && categoryRewards.TryGetValue(category, out var categoryReward))
                return categoryReward.GetRandomAmount();

            return defaultReward.GetRandomAmount();
        }
    }
}
