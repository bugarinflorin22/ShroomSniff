using UnityEngine;

namespace ShroomSniff.Gameplay
{
    [CreateAssetMenu(fileName = "CursorConfiguration", menuName = "ShroomSniff/Cursor Configuration")]
    public class CursorConfiguration : ScriptableObject
    {
        public Texture2D texture;
        public Vector2 hotspot;
        public CursorMode cursorMode = CursorMode.Auto;
    }
}
