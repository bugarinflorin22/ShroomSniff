using SerializableDictionary;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    [CreateAssetMenu(fileName = "MushroomConfiguration", menuName = "ShroomSniff/Mushroom Configuration")]
    public class MushroomConfiguration : ScriptableObject
    {
        [Header("Spawn Chances")]
        public SerializableDictionaryBase<MushroomCategory, ChanceValue> mushroomCategoryChances;
        public SerializableDictionaryBase<MushroomType, ChanceValue> mushroomTypeChances;

        [Header("Spawn Counts")]
        [Min(0)] public int mushroomsPerTile;

        [Header("Spawn Spacing")]
        [Min(1)] public int maxSpawnAttemptsPerMushroom = 12;
        public SerializableDictionaryBase<MushroomType, float> mushroomTypeMinimumSpawnSpacing;

        [Header("Spawn Area Padding")]
        public SerializableDictionaryBase<MushroomType, float> mushroomTypeSpawnPadding;

        [Header("Size")]
        public SerializableDictionaryBase<MushroomType, float> mushroomTypeSizeMultipliers;

        [Header("Collect")]
        [Min(0.01f)] public float holdToCollectDuration = 1f;
        public AnimationCurve pullCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public Vector3 pullPositionOffset = new Vector3(0f, 0.2f, 0f);
        public Vector3 pullRotationOffset = new Vector3(0f, 0f, 10f);
        [Min(0f)] public float pullRotationSpeed = 1f;
        public Vector3 pullScaleMultiplier = new Vector3(0.7f, 0.7f, 0.7f);

        [Header("Prefabs")]
        public SerializableDictionaryBase<MushroomCategory, MushroomController> mushroomPrefabs;
    }

    [System.Serializable]
    public struct ChanceValue
    {
        [Range(0, 100)] public int value;
    }
}