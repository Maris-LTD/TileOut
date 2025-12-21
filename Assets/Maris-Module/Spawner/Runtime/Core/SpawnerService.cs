using System.Collections.Generic;
using UnityEngine;
using uPools;

namespace GameModules.Spawner.Core
{
    public sealed class SpawnerService : ISpawner
    {
        private const string ROOT_NAME = "SpawnerRoot";
        
        private Transform _rootParent;
        private readonly Dictionary<GameObject, Transform> _prefabParents = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        public SpawnerService()
        {
            InitializeRoot();
        }

        private void InitializeRoot()
        {
            var rootObject = GameObject.Find(ROOT_NAME);
            if (rootObject == null)
            {
                rootObject = new GameObject(ROOT_NAME);
                Object.DontDestroyOnLoad(rootObject);
            }
            _rootParent = rootObject.transform;
        }

        private Transform GetOrCreatePrefabParent(GameObject prefab)
        {
            if (_prefabParents.TryGetValue(prefab, out var parent))
            {
                return parent;
            }

            var parentName = prefab.name;
            var parentObject = new GameObject(parentName);
            parentObject.transform.SetParent(_rootParent);
            parent = parentObject.transform;
            _prefabParents[prefab] = parent;
            return parent;
        }

        public GameObject Rent(GameObject prefab)
        {
            var instance = SharedGameObjectPool.Rent(prefab);
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public GameObject Rent(GameObject prefab, Transform parent)
        {
            var instance = SharedGameObjectPool.Rent(prefab, parent);
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation);
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation, parent);
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation);
            instance.transform.localScale = scale;
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation, parent);
            instance.transform.localScale = scale;
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        public T Rent<T>(T prefab) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab);
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public T Rent<T>(T prefab, Transform parent) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab, parent);
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public T Rent<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation);
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation, parent);
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation);
            instance.transform.localScale = scale;
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent) where T : Component
        {
            var instance = SharedGameObjectPool.Rent(prefab, position, rotation, parent);
            instance.transform.localScale = scale;
            _instanceToPrefab[instance.gameObject] = prefab.gameObject;
            return instance;
        }

        public void Return(GameObject instance)
        {
            if (instance == null) return;
            
            if (_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                var prefabParent = GetOrCreatePrefabParent(prefab);
                instance.transform.SetParent(prefabParent);
                _instanceToPrefab.Remove(instance);
            }
            
            SharedGameObjectPool.Return(instance);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            SharedGameObjectPool.Prewarm(prefab, count);
        }
    }
}

