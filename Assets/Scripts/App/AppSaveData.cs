using System;
using System.Collections.Generic;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.App
{
    [Serializable]
    public class AppSaveData
    {
        public int coins = 0;
        public LevelSettingsData levelSettings = new LevelSettingsData();
        public MushroomSettingsData mushroomSettings = new MushroomSettingsData();
        public UpgradeSaveData upgradeData = new UpgradeSaveData();
    }

    [Serializable]
    public class LevelSettingsData
    {
        public int tileCount;
        public Vector3 tileSize;
    }

    [Serializable]
    public class MushroomSettingsData
    {
        public int mushroomsPerTile;
        public int maxSpawnAttemptsPerMushroom;
        public float holdToCollectDuration;
        public Vector3 pullPositionOffset;
        public Vector3 pullRotationOffset;
        public float pullRotationSpeed;
        public Vector3 pullScaleMultiplier;

        public List<MushroomCategoryChanceEntry> mushroomCategoryChances = new List<MushroomCategoryChanceEntry>();
        public List<MushroomTypeChanceEntry> mushroomTypeChances = new List<MushroomTypeChanceEntry>();
        public List<MushroomTypeFloatEntry> mushroomTypeMinimumSpawnSpacing = new List<MushroomTypeFloatEntry>();
        public List<MushroomTypeFloatEntry> mushroomTypeSpawnPadding = new List<MushroomTypeFloatEntry>();
        public List<MushroomTypeFloatEntry> mushroomTypeSizeMultipliers = new List<MushroomTypeFloatEntry>();

        public Dictionary<MushroomCategory, int> BuildCategoryChanceMap()
        {
            var map = new Dictionary<MushroomCategory, int>();
            if (mushroomCategoryChances == null)
                return map;

            for (var i = 0; i < mushroomCategoryChances.Count; i++)
            {
                var entry = mushroomCategoryChances[i];
                map[entry.category] = entry.value;
            }

            return map;
        }

        public Dictionary<MushroomType, int> BuildTypeChanceMap()
        {
            var map = new Dictionary<MushroomType, int>();
            if (mushroomTypeChances == null)
                return map;

            for (var i = 0; i < mushroomTypeChances.Count; i++)
            {
                var entry = mushroomTypeChances[i];
                map[entry.type] = entry.value;
            }

            return map;
        }

        public Dictionary<MushroomType, float> BuildMinimumSpacingMap()
        {
            return BuildTypeFloatMap(mushroomTypeMinimumSpawnSpacing);
        }

        public Dictionary<MushroomType, float> BuildSpawnPaddingMap()
        {
            return BuildTypeFloatMap(mushroomTypeSpawnPadding);
        }

        public Dictionary<MushroomType, float> BuildSizeMultiplierMap()
        {
            return BuildTypeFloatMap(mushroomTypeSizeMultipliers);
        }

        private static Dictionary<MushroomType, float> BuildTypeFloatMap(List<MushroomTypeFloatEntry> entries)
        {
            var map = new Dictionary<MushroomType, float>();
            if (entries == null)
                return map;

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                map[entry.type] = entry.value;
            }

            return map;
        }
    }

    [Serializable]
    public struct MushroomCategoryChanceEntry
    {
        public MushroomCategory category;
        public int value;
    }

    [Serializable]
    public struct MushroomTypeChanceEntry
    {
        public MushroomType type;
        public int value;
    }

    [Serializable]
    public struct MushroomTypeFloatEntry
    {
        public MushroomType type;
        public float value;
    }
}
