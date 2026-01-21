using System;
using System.IO;
using ShroomSniff.Data;
using ShroomSniff.Gameplay;
using SerializableDictionary;
using UnityEngine;
using VContainer;

namespace ShroomSniff.App
{
    public interface IAppSaveSystem
    {
        AppSaveData SaveData { get; }
        event Action<AppSaveData> SaveDataChanged;
        void Load();
        void Save();
        void ResetToDefaults();
        void UpdateLevelSettings(Action<LevelSettingsData> update);
        void UpdateMushroomSettings(Action<MushroomSettingsData> update);
        void UpdateUpgradeData(Action<UpgradeSaveData> update);
        void AddCoins(int amount);
        void SpendCoins(int amount);
    }

    public class AppSaveSystem : IAppSaveSystem
    {
        private const string SaveFileName = "app-save.json";

        private readonly AppConfiguration _appConfiguration;
        private AppSaveData _saveData;

        public AppSaveData SaveData => _saveData;

        public event Action<AppSaveData> SaveDataChanged;

        [Inject]
        public AppSaveSystem(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public void Load()
        {
            var path = GetSavePath();
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    _saveData = JsonUtility.FromJson<AppSaveData>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[{nameof(AppSaveSystem)}]: Failed to load save: {ex.Message}");
                }
            }

            if (_saveData == null)
                _saveData = BuildDefaultSaveData();

            SaveDataChanged?.Invoke(_saveData);
        }

        public void Save()
        {
            if (_saveData == null)
                _saveData = BuildDefaultSaveData();

            var path = GetSavePath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonUtility.ToJson(_saveData, true);
            File.WriteAllText(path, json);
        }

        public void ResetToDefaults()
        {
            _saveData = BuildDefaultSaveData();
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        public void UpdateLevelSettings(Action<LevelSettingsData> update)
        {
            if (update == null)
                return;

            if (_saveData == null)
                _saveData = BuildDefaultSaveData();

            update(_saveData.levelSettings);
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        public void UpdateMushroomSettings(Action<MushroomSettingsData> update)
        {
            if (update == null)
                return;

            if (_saveData == null)
                _saveData = BuildDefaultSaveData();

            update(_saveData.mushroomSettings);
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        public void UpdateUpgradeData(Action<UpgradeSaveData> update)
        {
            if (update == null)
                return;

            if (_saveData == null)
                _saveData = BuildDefaultSaveData();

            update(_saveData.upgradeData);
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        private AppSaveData BuildDefaultSaveData()
        {
            var data = new AppSaveData();
            var gameplayConfig = GetGameplayConfiguration();
            if (gameplayConfig == null)
                return data;

            ApplyLevelDefaults(data.levelSettings, gameplayConfig.levelConfiguration);
            ApplyMushroomDefaults(data.mushroomSettings, gameplayConfig.mushroomConfiguration);
            return data;
        }

        private void ApplyLevelDefaults(LevelSettingsData settings, LevelConfiguration config)
        {
            if (settings == null || config == null)
                return;

            settings.tileCount = config.tileCount;
            settings.tileSize = config.tileSize;
        }

        private void ApplyMushroomDefaults(MushroomSettingsData settings, MushroomConfiguration config)
        {
            if (settings == null || config == null)
                return;

            settings.mushroomsPerTile = config.mushroomsPerTile;
            settings.maxSpawnAttemptsPerMushroom = config.maxSpawnAttemptsPerMushroom;
            settings.holdToCollectDuration = config.holdToCollectDuration;
            settings.pullPositionOffset = config.pullPositionOffset;
            settings.pullRotationOffset = config.pullRotationOffset;
            settings.pullRotationSpeed = config.pullRotationSpeed;
            settings.pullScaleMultiplier = config.pullScaleMultiplier;

            settings.mushroomCategoryChances = BuildCategoryChanceEntries(config);
            settings.mushroomTypeChances = BuildTypeChanceEntries(config);
            settings.mushroomTypeMinimumSpawnSpacing = BuildTypeFloatEntries(config.mushroomTypeMinimumSpawnSpacing);
            settings.mushroomTypeSpawnPadding = BuildTypeFloatEntries(config.mushroomTypeSpawnPadding);
            settings.mushroomTypeSizeMultipliers = BuildTypeFloatEntries(config.mushroomTypeSizeMultipliers);
        }

        private GameplayScopeConfiguration GetGameplayConfiguration()
        {
            if (_appConfiguration == null || _appConfiguration.GameplayScopePrefab == null)
                return null;

            return _appConfiguration.GameplayScopePrefab.Configuration;
        }

        private static System.Collections.Generic.List<MushroomCategoryChanceEntry> BuildCategoryChanceEntries(MushroomConfiguration config)
        {
            if (config == null || config.mushroomCategoryChances == null)
                return new System.Collections.Generic.List<MushroomCategoryChanceEntry>();

            var entries = new System.Collections.Generic.List<MushroomCategoryChanceEntry>(config.mushroomCategoryChances.Count);
            foreach (var pair in config.mushroomCategoryChances)
            {
                entries.Add(new MushroomCategoryChanceEntry
                {
                    category = pair.Key,
                    value = pair.Value.value
                });
            }

            return entries;
        }

        private static System.Collections.Generic.List<MushroomTypeChanceEntry> BuildTypeChanceEntries(MushroomConfiguration config)
        {
            if (config == null || config.mushroomTypeChances == null)
                return new System.Collections.Generic.List<MushroomTypeChanceEntry>();

            var entries = new System.Collections.Generic.List<MushroomTypeChanceEntry>(config.mushroomTypeChances.Count);
            foreach (var pair in config.mushroomTypeChances)
            {
                entries.Add(new MushroomTypeChanceEntry
                {
                    type = pair.Key,
                    value = pair.Value.value
                });
            }

            return entries;
        }

        private static System.Collections.Generic.List<MushroomTypeFloatEntry> BuildTypeFloatEntries(SerializableDictionaryBase<MushroomType, float> dictionary)
        {
            if (dictionary == null)
                return new System.Collections.Generic.List<MushroomTypeFloatEntry>();

            var entries = new System.Collections.Generic.List<MushroomTypeFloatEntry>(dictionary.Count);
            foreach (var pair in dictionary)
            {
                entries.Add(new MushroomTypeFloatEntry
                {
                    type = pair.Key,
                    value = pair.Value
                });
            }

            return entries;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            _saveData.coins += amount;
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        public void SpendCoins(int amount)
        {
            if (amount <= 0 || _saveData.coins < amount)
                return;

            _saveData.coins -= amount;
            Save();
            SaveDataChanged?.Invoke(_saveData);
        }

        private static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }
    }
}
