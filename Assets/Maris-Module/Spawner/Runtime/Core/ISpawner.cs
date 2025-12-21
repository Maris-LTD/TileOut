using UnityEngine;

namespace GameModules.Spawner.Core
{
    public interface ISpawner
    {
        GameObject Rent(GameObject prefab);
        GameObject Rent(GameObject prefab, Transform parent);
        GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation);
        GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
        GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale);
        GameObject Rent(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent);
        
        T Rent<T>(T prefab) where T : Component;
        T Rent<T>(T prefab, Transform parent) where T : Component;
        T Rent<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;
        T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component;
        T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component;
        T Rent<T>(T prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent) where T : Component;
        
        void Return(GameObject instance);
        void Prewarm(GameObject prefab, int count);
    }
}

