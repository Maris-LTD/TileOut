using System.Collections.Generic;
using System.Linq;

namespace GameModules.ResourceManager
{
    public class ResourceCache
    {
        private readonly Dictionary<string, long> _cachedResources = new();
        
        public void Add(string resourceId, long amount)
        {
            if (!_cachedResources.ContainsKey(resourceId))
                _cachedResources[resourceId] = 0;
            
            _cachedResources[resourceId] += amount;
        }
        
        public void Remove(string resourceId, long amount)
        {
            if (!_cachedResources.ContainsKey(resourceId))
                _cachedResources[resourceId] = 0;
            
            _cachedResources[resourceId] -= amount;
        }
        
        public long Get(string resourceId)
        {
            return _cachedResources.GetValueOrDefault(resourceId, 0);
        }
        
        public Dictionary<string, long> GetAll()
        {
            return new Dictionary<string, long>(_cachedResources);
        }
        
        public void Clear()
        {
            _cachedResources.Clear();
        }
        
        public bool HasData()
        {
            return _cachedResources.Count > 0 && _cachedResources.Values.Any(v => v != 0);
        }
        
        public IEnumerable<string> GetResourceIds()
        {
            return _cachedResources.Keys;
        }
    }
}

