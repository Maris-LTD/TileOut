using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules.ResourceManager
{
    public class PlayerPrefsResourceStorage : IResourceStorage
    {
        private const string StorageKey = "GameModules_Resources";
        
        [Serializable]
        private class StorageWrapper
        {
            public List<ResourceData> Resources = new();
        }
        
        public void Save(Dictionary<string, ResourceData> resources)
        {
            var wrapper = new StorageWrapper();
            wrapper.Resources.AddRange(resources.Values);
            
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(StorageKey, json);
            PlayerPrefs.Save();
        }
        
        public Dictionary<string, ResourceData> Load()
        {
            var result = new Dictionary<string, ResourceData>();
            
            if (!PlayerPrefs.HasKey(StorageKey)) return result;
            
            var json = PlayerPrefs.GetString(StorageKey);
            if (string.IsNullOrEmpty(json)) return result;
            
            try
            {
                var wrapper = JsonUtility.FromJson<StorageWrapper>(json);
                if (wrapper?.Resources != null)
                {
                    foreach (var data in wrapper.Resources)
                    {
                        if (!string.IsNullOrEmpty(data.ResourceId))
                        {
                            result[data.ResourceId] = data;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsResourceStorage] Failed to load resources: {e.Message}");
            }
            
            return result;
        }
        
        public void Clear()
        {
            PlayerPrefs.DeleteKey(StorageKey);
            PlayerPrefs.Save();
        }
        
        public bool HasData()
        {
            return PlayerPrefs.HasKey(StorageKey);
        }
    }
}

