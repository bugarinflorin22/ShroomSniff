using System;

namespace ShroomSniff.App
{
    public interface IAudioSystem : IDisposable
    {
        void Initialize();
        void PlaySuction();
    }
}
