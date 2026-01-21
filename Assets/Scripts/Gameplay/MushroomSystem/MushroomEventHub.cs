using System;
using ShroomSniff.Data;

namespace ShroomSniff.Gameplay
{
    public class MushroomEventArgs : EventArgs
    {
        public MushroomController Mushroom { get; }
        public MushroomCategory Category { get; }
        public MushroomType Type { get; }

        public MushroomEventArgs(MushroomController mushroom, MushroomCategory category, MushroomType type)
        {
            Mushroom = mushroom;
            Category = category;
            Type = type;
        }
    }
    public interface IMushroomEventHub
    {
        event Action<float> ChargeProgressChanged;
        event Action<MushroomEventArgs> MushroomPulled;
        event Action<MushroomEventArgs> MushroomCollected;

        void NotifyChargeProgress(float normalized);
        void NotifyMushroomPulled(MushroomController mushroom, MushroomCategory category, MushroomType type);
        void NotifyMushroomCollected(MushroomController mushroom, MushroomCategory category, MushroomType type);
    }

    public class MushroomEventHub : IMushroomEventHub
    {
        public event Action<float> ChargeProgressChanged;
        public event Action<MushroomEventArgs> MushroomPulled;
        public event Action<MushroomEventArgs> MushroomCollected;

        public void NotifyChargeProgress(float normalized)
        {
            ChargeProgressChanged?.Invoke(normalized);
        }

        public void NotifyMushroomPulled(MushroomController mushroom, MushroomCategory category, MushroomType type)
        {
            MushroomPulled?.Invoke(new MushroomEventArgs(mushroom, category, type));
        }

        public void NotifyMushroomCollected(MushroomController mushroom, MushroomCategory category, MushroomType type)
        {
            MushroomCollected?.Invoke(new MushroomEventArgs(mushroom, category, type));
        }
    }
}
