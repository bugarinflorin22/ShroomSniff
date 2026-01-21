using System.Collections.Generic;
using ShroomSniff.App;
using ShroomSniff.Constants;
using UnityEngine;
using VContainer;

namespace ShroomSniff.Gameplay
{
    public class LevelSystem : ILevelSystem
    {
        private readonly LevelConfiguration _levelConfiguration;
        private readonly IAppSaveSystem _saveSystem;

        private Transform _levelRoot;
        
        [Inject]
        public LevelSystem(LevelConfiguration levelConfiguration, IAppSaveSystem saveSystem)
        {
            _levelConfiguration = levelConfiguration;
            _saveSystem = saveSystem;
        }

        public void InjectGameplayScene(GameplayScene gameplayScene)
        {
            _levelRoot = gameplayScene.GetLevelRoot();
        }

        public void GenerateLevel()
        {
            if (_levelRoot == null)
            {
                Debug.LogError($"[{nameof(LevelSystem)}]: Cannot generate level - level root is null. Call InjectGameplayScene first.");
                return;
            }

            if (_levelConfiguration == null)
            {
                Debug.LogError($"[{nameof(LevelSystem)}]: Cannot generate level - configuration is null.");
                return;
            }

            var cells = GenerateUniformGrid();
            SpawnTiles(cells);

            Debug.Log($"[{nameof(LevelSystem)}]: Generated uniform map: {cells.Count} tiles.");
        }

        private List<Vector2Int> GenerateUniformGrid()
        {
            var count = Mathf.Max(1, GetTileCount());
            var width = Mathf.CeilToInt(Mathf.Sqrt(count));
            var height = Mathf.CeilToInt((float)count / width);

            var cells = new List<Vector2Int>(count);
            var startX = -(width / 2);
            var startY = -(height / 2);

            for (var y = 0; y < height && cells.Count < count; y++)
            {
                for (var x = 0; x < width && cells.Count < count; x++)
                {
                    cells.Add(new Vector2Int(startX + x, startY + y));
                }
            }

            return cells;
        }

        private void SpawnTiles(List<Vector2Int> cells)
        {
            if (_levelConfiguration.grassPilePrefab == null)
            {
                Debug.LogError($"[{nameof(LevelSystem)}]: Grass pile prefab is null in configuration.");
                return;
            }

            foreach (var cell in cells)
            {
                var tileSize = GetTileSize();
                var worldPos = new Vector3(cell.x * tileSize.x, 0f, cell.y * tileSize.z);
                var tile = Object.Instantiate(_levelConfiguration.grassPilePrefab, worldPos, Quaternion.identity);
                tile.name = $"{Texts.TILE_PREFIX}_{cell.x}_{cell.y}";
                tile.transform.localScale = tileSize;
                tile.transform.SetParent(_levelRoot, true);
            }
        }

        private int GetTileCount()
        {
            var settings = _saveSystem != null ? _saveSystem.SaveData?.levelSettings : null;
            if (settings != null && settings.tileCount > 0)
                return settings.tileCount;

            return _levelConfiguration != null ? _levelConfiguration.tileCount : 1;
        }

        private Vector3 GetTileSize()
        {
            var settings = _saveSystem != null ? _saveSystem.SaveData?.levelSettings : null;
            if (settings != null)
            {
                var size = settings.tileSize;
                if (size.x > 0f && size.y > 0f && size.z > 0f)
                    return size;
            }

            return _levelConfiguration != null ? _levelConfiguration.tileSize : Vector3.one;
        }
    }
}
