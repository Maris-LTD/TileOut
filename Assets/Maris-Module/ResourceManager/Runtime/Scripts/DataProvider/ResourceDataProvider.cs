using System.Collections.Generic;
using GameModules.DataManager;
using UnityEngine;

namespace GameModules.ResourceManager
{
    public class ResourceDataProvider : IDataProvider
    {
        private readonly IResourceStorage _storage;
        private Dictionary<string, ResourceData> _cachedData;
        
        public ResourceDataProvider()
        {
            _storage = new PlayerPrefsResourceStorage();
        }
        
        public ResourceDataProvider(IResourceStorage storage)
        {
            _storage = storage;
        }
        
        public bool LoadData(object data)
        {
            try
            {
                if (data is string json && !string.IsNullOrEmpty(json))
                {
                    _cachedData = JsonUtility.FromJson<SerializableResourceData>(json)?.ToDictionary();
                    return _cachedData != null;
                }
                
                _cachedData = _storage.Load();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public object GetData()
        {
            return _cachedData ?? _storage.Load();
        }
        
        public object SaveData()
        {
            _cachedData ??= _storage.Load();
            return JsonUtility.ToJson(SerializableResourceData.FromDictionary(_cachedData));
        }
        
        public bool HasData()
        {
            return _storage.HasData() || _cachedData != null;
        }
        
        public void ClearData()
        {
            _cachedData = null;
            _storage.Clear();
        }
        
        [System.Serializable]
        private class SerializableResourceData
        {
            public List<ResourceData> Resources = new();
            
            public Dictionary<string, ResourceData> ToDictionary()
            {
                var dict = new Dictionary<string, ResourceData>();
                foreach (var data in Resources)
                {
                    if (!string.IsNullOrEmpty(data.ResourceId))
                        dict[data.ResourceId] = data;
                }
                return dict;
            }
            
            public static SerializableResourceData FromDictionary(Dictionary<string, ResourceData> dict)
            {
                var result = new SerializableResourceData();
                if (dict != null)
                    result.Resources.AddRange(dict.Values);
                return result;
            }
        }
    }
}

