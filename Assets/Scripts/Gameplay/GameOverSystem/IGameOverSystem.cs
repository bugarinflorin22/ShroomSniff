using System;

namespace ShroomSniff.Gameplay
{
    public interface IGameOverSystem
    {
        event Action TimeIsUp;
        void ShowTimeUpScreen();
    }
}