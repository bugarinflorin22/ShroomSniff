using System.Collections.Generic;
using ShroomSniff.App;
using ShroomSniff.Configurations;
using ShroomSniff.Constants;
using ShroomSniff.Data;
using UnityEngine;
using VContainer;

namespace ShroomSniff.Gameplay
{
    public class MushroomSystem : IMushroomSystem
    {
        private readonly MushroomConfiguration _mushroomConfiguration;
        private readonly IAppSaveSystem _saveSystem;
        private readonly CoinRewardConfiguration _coinRewardConfig;
        private readonly UpgradeApplicationSystem _upgradeApplicationSystem;

        private Transform _levelRoot;
        private int _totalMushrooms;
        private int _collectedMushrooms;
        private int _coinsEarnedThisRound;

        public event System.Action AllMushroomsCollected;

        [Inject]
        public MushroomSystem(
            MushroomConfiguration mushroomConfiguration, 
            IAppSaveSystem saveSystem,
            CoinRewardConfiguration coinRewardConfiguration,
            UpgradeApplicationSystem upgradeApplicationSystem)
        {
            _mushroomConfiguration = mushroomConfiguration;
            _saveSystem = saveSystem;
            _coinRewardConfig = coinRewardConfiguration;
            _upgradeApplicationSystem = upgradeApplicationSystem;
        }

        public void InjectGameplayScene(GameplayScene gameplayScene)
        {
            _levelRoot = gameplayScene.GetLevelRoot();
        }

        public int GetTotalMushroomCount()
        {
            return _totalMushrooms;
        }

        public int GetCollectedMushroomCount()
        {
            return _collectedMushrooms;
        }

        public int GetCoinsEarnedThisRound()
        {
            return _coinsEarnedThisRound;
        }

        public void SpawnMushrooms()
        {
            // Reset counters
            _totalMushrooms = 0;
            _collectedMushrooms = 0;
            _coinsEarnedThisRound = 0;
            
            // Subscribe to mushroom collection event
            MushroomController.MushroomPulled += OnMushroomCollected;
            
            if (_levelRoot == null || _mushroomConfiguration == null)
            {
                Debug.LogError($"[{nameof(MushroomSystem)}]: Missing level root or configuration.");
                return;
            }

            var tiles = GetTiles();
            if (tiles.Count == 0)
            {
                Debug.LogError($"[{nameof(MushroomSystem)}]: No tiles found for spawning.");
                return;
            }

            if (_mushroomConfiguration.mushroomPrefabs == null || _mushroomConfiguration.mushroomPrefabs.Count == 0)
            {
                Debug.LogError($"[{nameof(MushroomSystem)}]: No mushroom prefabs configured.");
                return;
            }

            var settings = _saveSystem != null ? _saveSystem.SaveData?.mushroomSettings : null;
            var categoryChances = GetCategoryChances(settings);
            var typeChances = GetTypeChances(settings);
            var spawnPadding = GetSpawnPadding(settings);
            var minSpacing = GetMinimumSpawnSpacing(settings);

            var categoryOptions = BuildCategoryOptions(categoryChances);
            var typeOptions = BuildTypeOptions(typeChances);

            if (categoryOptions.Count == 0 || typeOptions.Count == 0)
            {
                Debug.LogError($"[{nameof(MushroomSystem)}]: Missing spawn chances for categories or types.");
                return;
            }

            var mushroomsPerTile = Mathf.Max(0, settings != null ? settings.mushroomsPerTile : _mushroomConfiguration.mushroomsPerTile);
            if (mushroomsPerTile <= 0)
                return;

            var maxAttempts = settings != null && settings.maxSpawnAttemptsPerMushroom > 0
                ? settings.maxSpawnAttemptsPerMushroom
                : _mushroomConfiguration.maxSpawnAttemptsPerMushroom;
            var runtimeSettings = BuildRuntimeSettings(settings);

            ShuffleTiles(tiles);
            var spawnedPositions = new List<Vector3>();

            foreach (var tile in tiles)
            {
                for (var i = 0; i < mushroomsPerTile; i++)
                    SpawnMushroomOnTile(tile, categoryOptions, typeOptions, spawnedPositions, minSpacing, spawnPadding, maxAttempts, runtimeSettings);
            }
        }

        private void SpawnMushroomOnTile(
            Transform tile,
            List<WeightedOption<MushroomCategory>> categoryOptions,
            List<WeightedOption<MushroomType>> typeOptions,
            List<Vector3> spawnedPositions,
            Dictionary<MushroomType, float> minSpacing,
            Dictionary<MushroomType, float> spawnPadding,
            int maxAttempts,
            IMushroomRuntimeSettings runtimeSettings)
        {
            var category = GetWeightedRandom(categoryOptions);
            if (!_mushroomConfiguration.mushroomPrefabs.TryGetValue(category, out var prefab) || prefab == null)
                return;

            var mushroomType = GetWeightedRandom(typeOptions);
            if (!TryGetSpawnPoint(tile, mushroomType, spawnedPositions, minSpacing, spawnPadding, maxAttempts, out var spawnPosition))
                return;

            var mushroom = Object.Instantiate(prefab, _levelRoot, true);
            mushroom.transform.localPosition = spawnPosition;
            mushroom.Initialize(mushroomType, runtimeSettings);
            spawnedPositions.Add(spawnPosition);
            _totalMushrooms++;
        }

        private bool TryGetSpawnPoint(
            Transform tile,
            MushroomType mushroomType,
            List<Vector3> spawnedPositions,
            Dictionary<MushroomType, float> minSpacing,
            Dictionary<MushroomType, float> spawnPadding,
            int maxAttempts,
            out Vector3 spawnPosition)
        {
            var spacing = GetMinimumSpawnSpacing(mushroomType, minSpacing);
            var attempts = Mathf.Max(1, maxAttempts);

            for (var i = 0; i < attempts; i++)
            {
                var candidate = GetRandomPointOnTile(tile, mushroomType, spawnPadding);
                if (spacing <= 0f || IsPositionClear(candidate, spawnedPositions, spacing))
                {
                    spawnPosition = candidate;
                    return true;
                }
            }

            spawnPosition = Vector3.zero;
            return false;
        }

        private static bool IsPositionClear(Vector3 candidate, List<Vector3> spawnedPositions, float minSpacing)
        {
            var minSpacingSqr = minSpacing * minSpacing;
            for (var i = 0; i < spawnedPositions.Count; i++)
            {
                if ((spawnedPositions[i] - candidate).sqrMagnitude < minSpacingSqr)
                    return false;
            }

            return true;
        }

        private List<Transform> GetTiles()
        {
            var tiles = new List<Transform>();
            for (var i = 0; i < _levelRoot.childCount; i++)
            {
                var child = _levelRoot.GetChild(i);
                if (child.name.StartsWith(Texts.TILE_PREFIX))
                    tiles.Add(child);
            }

            return tiles;
        }

        private Vector3 GetRandomPointOnTile(Transform tile, MushroomType mushroomType, Dictionary<MushroomType, float> spawnPadding)
        {
            var tileScale = tile.localScale;
            var padding = GetSpawnPadding(mushroomType, spawnPadding);
            var halfX = Mathf.Max(0f, tileScale.x * 0.5f - padding);
            var halfZ = Mathf.Max(0f, tileScale.z * 0.5f - padding);
            var x = Random.Range(-halfX, halfX);
            var z = Random.Range(-halfZ, halfZ);
            var pos = tile.position;
            return new Vector3(pos.x + x, pos.y, pos.z + z);
        }

        private float GetSpawnPadding(MushroomType mushroomType, Dictionary<MushroomType, float> spawnPadding)
        {
            if (spawnPadding != null && spawnPadding.TryGetValue(mushroomType, out var padding))
                return Mathf.Max(0f, padding);

            if (_mushroomConfiguration.mushroomTypeSpawnPadding != null &&
                _mushroomConfiguration.mushroomTypeSpawnPadding.TryGetValue(mushroomType, out padding))
                return Mathf.Max(0f, padding);

            return 0f;
        }

        private float GetMinimumSpawnSpacing(MushroomType mushroomType, Dictionary<MushroomType, float> minSpacing)
        {
            if (minSpacing != null && minSpacing.TryGetValue(mushroomType, out var spacing))
                return Mathf.Max(0f, spacing);

            if (_mushroomConfiguration.mushroomTypeMinimumSpawnSpacing != null &&
                _mushroomConfiguration.mushroomTypeMinimumSpawnSpacing.TryGetValue(mushroomType, out spacing))
                return Mathf.Max(0f, spacing);

            return 0f;
        }

        private List<WeightedOption<MushroomCategory>> BuildCategoryOptions(Dictionary<MushroomCategory, int> categoryChances)
        {
            var options = new List<WeightedOption<MushroomCategory>>();
            foreach (var pair in _mushroomConfiguration.mushroomPrefabs)
            {
                var weight = GetWeight(categoryChances, pair.Key);
                if (pair.Value != null && weight > 0)
                    options.Add(new WeightedOption<MushroomCategory>(pair.Key, weight));
            }

            return options;
        }

        private List<WeightedOption<MushroomType>> BuildTypeOptions(Dictionary<MushroomType, int> typeChances)
        {
            var options = new List<WeightedOption<MushroomType>>();
            if (typeChances == null || typeChances.Count == 0)
                return options;

            foreach (var pair in typeChances)
            {
                if (pair.Value > 0)
                    options.Add(new WeightedOption<MushroomType>(pair.Key, pair.Value));
            }

            return options;
        }

        private static int GetWeight(Dictionary<MushroomCategory, int> chances, MushroomCategory category)
        {
            if (chances == null)
                return 0;

            return chances.TryGetValue(category, out var weight) ? weight : 0;
        }

        private static T GetWeightedRandom<T>(List<WeightedOption<T>> options)
        {
            var total = 0f;
            foreach (var option in options)
            {
                total += option.Weight;
            }

            var roll = Random.Range(0f, total);
            var cumulative = 0f;
            foreach (var option in options)
            {
                cumulative += option.Weight;
                if (roll <= cumulative)
                {
                    return option.Value;
                }
            }

            return options[options.Count - 1].Value;
        }

        private void OnMushroomCollected(MushroomCategory category, MushroomType type)
        {
            _collectedMushrooms++;
            
            // Award coins based on mushroom type and category
            if (_coinRewardConfig != null)
            {
                int coinReward = _coinRewardConfig.GetCoinReward(category, type);
                _coinsEarnedThisRound += coinReward;
                _saveSystem.AddCoins(coinReward);
                
                Debug.Log($"[{nameof(MushroomSystem)}]: Collected {category} {type} - Earned {coinReward} coins! Total this round: {_coinsEarnedThisRound}");
            }
            
            Debug.Log($"[{nameof(MushroomSystem)}]: Mushroom collected! {_collectedMushrooms}/{_totalMushrooms}");
            
            if (_collectedMushrooms >= _totalMushrooms && _totalMushrooms > 0)
            {
                Debug.Log($"[{nameof(MushroomSystem)}]: All mushrooms collected! Total coins earned: {_coinsEarnedThisRound}");
                MushroomController.MushroomPulled -= OnMushroomCollected;
                AllMushroomsCollected?.Invoke();
            }
        }

        private static void ShuffleTiles(List<Transform> tiles)
        {
            for (var i = tiles.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
            }
        }

        private Dictionary<MushroomCategory, int> GetCategoryChances(MushroomSettingsData settings)
        {
            if (settings == null)
                return BuildCategoryChancesFromConfig();

            var map = settings.BuildCategoryChanceMap();
            return map.Count > 0 ? map : BuildCategoryChancesFromConfig();
        }

        private Dictionary<MushroomType, int> GetTypeChances(MushroomSettingsData settings)
        {
            if (settings == null)
                return BuildTypeChancesFromConfig();

            var map = settings.BuildTypeChanceMap();
            return map.Count > 0 ? map : BuildTypeChancesFromConfig();
        }

        private Dictionary<MushroomType, float> GetMinimumSpawnSpacing(MushroomSettingsData settings)
        {
            if (settings == null)
                return BuildTypeFloatMap(_mushroomConfiguration.mushroomTypeMinimumSpawnSpacing);

            var map = settings.BuildMinimumSpacingMap();
            return map.Count > 0 ? map : BuildTypeFloatMap(_mushroomConfiguration.mushroomTypeMinimumSpawnSpacing);
        }

        private Dictionary<MushroomType, float> GetSpawnPadding(MushroomSettingsData settings)
        {
            if (settings == null)
                return BuildTypeFloatMap(_mushroomConfiguration.mushroomTypeSpawnPadding);

            var map = settings.BuildSpawnPaddingMap();
            return map.Count > 0 ? map : BuildTypeFloatMap(_mushroomConfiguration.mushroomTypeSpawnPadding);
        }

        private IMushroomRuntimeSettings BuildRuntimeSettings(MushroomSettingsData settings)
        {
            var holdDuration = settings != null && settings.holdToCollectDuration > 0f
                ? settings.holdToCollectDuration
                : _mushroomConfiguration.holdToCollectDuration;
            
            // Apply speed multiplier - higher speed = lower duration (faster collection)
            float speedMultiplier = _upgradeApplicationSystem.GetSpeedMultiplier();
            holdDuration /= speedMultiplier;

            var rotationSpeed = settings != null ? settings.pullRotationSpeed : _mushroomConfiguration.pullRotationSpeed;
            var pullPosition = settings != null ? settings.pullPositionOffset : _mushroomConfiguration.pullPositionOffset;
            var pullRotation = settings != null ? settings.pullRotationOffset : _mushroomConfiguration.pullRotationOffset;
            var pullScale = settings != null ? settings.pullScaleMultiplier : _mushroomConfiguration.pullScaleMultiplier;

            var sizeMultipliers = settings != null && settings.mushroomTypeSizeMultipliers != null && settings.mushroomTypeSizeMultipliers.Count > 0
                ? settings.BuildSizeMultiplierMap()
                : BuildTypeFloatMap(_mushroomConfiguration.mushroomTypeSizeMultipliers);

            return new MushroomRuntimeSettings(
                holdDuration,
                _mushroomConfiguration.pullCurve,
                pullPosition,
                pullRotation,
                rotationSpeed,
                pullScale,
                sizeMultipliers);
        }

        private Dictionary<MushroomCategory, int> BuildCategoryChancesFromConfig()
        {
            var map = new Dictionary<MushroomCategory, int>();
            if (_mushroomConfiguration.mushroomCategoryChances == null)
                return map;

            foreach (var pair in _mushroomConfiguration.mushroomCategoryChances)
                map[pair.Key] = pair.Value.value;

            return map;
        }

        private Dictionary<MushroomType, int> BuildTypeChancesFromConfig()
        {
            var map = new Dictionary<MushroomType, int>();
            if (_mushroomConfiguration.mushroomTypeChances == null)
                return map;

            foreach (var pair in _mushroomConfiguration.mushroomTypeChances)
                map[pair.Key] = pair.Value.value;

            return map;
        }

        private static Dictionary<MushroomType, float> BuildTypeFloatMap(SerializableDictionary.SerializableDictionaryBase<MushroomType, float> dictionary)
        {
            var map = new Dictionary<MushroomType, float>();
            if (dictionary == null)
                return map;

            foreach (var pair in dictionary)
                map[pair.Key] = pair.Value;

            return map;
        }

        private readonly struct WeightedOption<T>
        {
            public T Value { get; }
            public float Weight { get; }

            public WeightedOption(T value, float weight)
            {
                Value = value;
                Weight = weight;
            }
        }
    }
}