using System;
using VContainer.Unity;

namespace ShroomSniff
{
    public interface IGameStateBase : IStartable, IDisposable
    {
        void OnRun();
    }
}