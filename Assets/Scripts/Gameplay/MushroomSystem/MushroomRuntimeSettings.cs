using System.Collections.Generic;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    public interface IMushroomRuntimeSettings
    {
        float HoldToCollectDuration { get; }
        AnimationCurve PullCurve { get; }
        Vector3 PullPositionOffset { get; }
        Vector3 PullRotationOffset { get; }
        float PullRotationSpeed { get; }
        Vector3 PullScaleMultiplier { get; }
        bool TryGetSizeMultiplier(MushroomType type, out float multiplier);
    }

    public class MushroomRuntimeSettings : IMushroomRuntimeSettings
    {
        private readonly Dictionary<MushroomType, float> _sizeMultipliers;

        public float HoldToCollectDuration { get; }
        public AnimationCurve PullCurve { get; }
        public Vector3 PullPositionOffset { get; }
        public Vector3 PullRotationOffset { get; }
        public float PullRotationSpeed { get; }
        public Vector3 PullScaleMultiplier { get; }

        public MushroomRuntimeSettings(
            float holdToCollectDuration,
            AnimationCurve pullCurve,
            Vector3 pullPositionOffset,
            Vector3 pullRotationOffset,
            float pullRotationSpeed,
            Vector3 pullScaleMultiplier,
            Dictionary<MushroomType, float> sizeMultipliers)
        {
            HoldToCollectDuration = holdToCollectDuration;
            PullCurve = pullCurve;
            PullPositionOffset = pullPositionOffset;
            PullRotationOffset = pullRotationOffset;
            PullRotationSpeed = pullRotationSpeed;
            PullScaleMultiplier = pullScaleMultiplier;
            _sizeMultipliers = sizeMultipliers ?? new Dictionary<MushroomType, float>();
        }

        public bool TryGetSizeMultiplier(MushroomType type, out float multiplier)
        {
            return _sizeMultipliers.TryGetValue(type, out multiplier);
        }
    }
}
