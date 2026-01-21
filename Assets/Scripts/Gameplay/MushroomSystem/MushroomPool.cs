using System.Collections.Generic;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    public class MushroomPool
    {
        private readonly Dictionary<MushroomCategory, Queue<MushroomController>> _pools;
        private readonly Dictionary<MushroomCategory, MushroomController> _prefabs;
        private readonly Transform _parent;

        public MushroomPool(Dictionary<MushroomCategory, MushroomController> prefabs, Transform parent)
        {
            _prefabs = prefabs;
            _parent = parent;
            _pools = new Dictionary<MushroomCategory, Queue<MushroomController>>();

            foreach (var category in prefabs.Keys)
            {
                _pools[category] = new Queue<MushroomController>();
            }
        }

        public MushroomController Get(MushroomCategory category)
        {
            if (!_pools.TryGetValue(category, out var pool))
            {
                Debug.LogWarning($"[{nameof(MushroomPool)}]: No pool for category {category}");
                return null;
            }

            MushroomController mushroom;
            
            if (pool.Count > 0)
            {
                mushroom = pool.Dequeue();
                mushroom.gameObject.SetActive(true);
            }
            else
            {
                if (!_prefabs.TryGetValue(category, out var prefab) || prefab == null)
                {
                    Debug.LogWarning($"[{nameof(MushroomPool)}]: No prefab for category {category}");
                    return null;
                }

                mushroom = Object.Instantiate(prefab, _parent, true);
            }

            return mushroom;
        }

        public void Return(MushroomController mushroom, MushroomCategory category)
        {
            if (mushroom == null)
                return;

            if (!_pools.TryGetValue(category, out var pool))
            {
                Object.Destroy(mushroom.gameObject);
                return;
            }

            mushroom.gameObject.SetActive(false);
            mushroom.transform.SetParent(_parent, true);
            pool.Enqueue(mushroom);
        }

        public void Clear()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var mushroom = pool.Dequeue();
                    if (mushroom != null)
                        Object.Destroy(mushroom.gameObject);
                }
            }

            _pools.Clear();
        }
    }
}
