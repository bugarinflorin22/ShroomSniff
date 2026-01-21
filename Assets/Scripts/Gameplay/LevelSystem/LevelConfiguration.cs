using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ShroomSniff/Level Configuration")]
public class LevelConfiguration : ScriptableObject
{
    [Header("Tile")]
    public GameObject grassPilePrefab;
    public Vector3 tileSize;

    [Tooltip("How many tiles to add (rough size of the island)")] [Min(1)]
    public int tileCount;
}
