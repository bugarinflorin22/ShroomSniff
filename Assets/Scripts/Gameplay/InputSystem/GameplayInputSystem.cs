using System;
using UnityEngine;
using VContainer.Unity;

namespace ShroomSniff.Gameplay
{
    /// <summary>
    /// Centralized input system to avoid polling Input in every mushroom controller
    /// </summary>
    public interface IGameplayInputSystem
    {
        bool IsMouseButtonDown { get; }
        bool IsMouseButtonHeld { get; }
        bool IsMouseButtonUp { get; }
        Vector3 MousePosition { get; }
        
        event Action MouseButtonPressed;
        event Action MouseButtonReleased;
    }

    public class GameplayInputSystem : IGameplayInputSystem, ITickable
    {
        public bool IsMouseButtonDown { get; private set; }
        public bool IsMouseButtonHeld { get; private set; }
        public bool IsMouseButtonUp { get; private set; }
        public Vector3 MousePosition { get; private set; }

        public event Action MouseButtonPressed;
        public event Action MouseButtonReleased;

        public void Tick()
        {
            IsMouseButtonDown = Input.GetMouseButtonDown(0);
            IsMouseButtonHeld = Input.GetMouseButton(0);
            IsMouseButtonUp = Input.GetMouseButtonUp(0);
            MousePosition = Input.mousePosition;

            if (IsMouseButtonDown)
                MouseButtonPressed?.Invoke();

            if (IsMouseButtonUp)
                MouseButtonReleased?.Invoke();
        }
    }
}
