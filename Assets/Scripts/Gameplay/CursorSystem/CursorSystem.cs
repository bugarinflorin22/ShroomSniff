using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ShroomSniff.Gameplay
{
    public class CursorSystem : ICursorSystem, IStartable
    {
        private readonly CursorConfiguration _cursorConfig;
        
        [Inject]
        public CursorSystem(CursorConfiguration cursorConfig)
        {
            _cursorConfig = cursorConfig;
        }

        public void Start()
        {
            SetCursor();
        }

        private void SetCursor()
        {
            Cursor.SetCursor(_cursorConfig.texture, _cursorConfig.hotspot, _cursorConfig.cursorMode);
        }
    }
}
