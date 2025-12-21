using System;
using System.Collections.Generic;

namespace GameModules.ResourceManager
{
    public interface IResourceService
    {
        void Add(string resourceId, long amount);
        bool Remove(string resourceId, long amount);
        void Set(string resourceId, long amount);
        long Get(string resourceId);
        bool Has(string resourceId, long amount);
        
        void CacheAdd(string resourceId, long amount);
        void CacheRemove(string resourceId, long amount);
        long GetCached(string resourceId);
        Dictionary<string, long> GetAllCached();
        void CommitCache();
        void ClearCache();
        bool HasCachedData();
        
        DateTime GetNextRegenTime(string resourceId);
        float GetSecondsUntilNextRegen(string resourceId);
        
        ResourceDefinition GetDefinition(string resourceId);
        IReadOnlyList<ResourceDefinition> GetAllDefinitions();
        IReadOnlyList<ResourceDefinition> GetDefinitionsByCategory(ResourceCategory category);
        
        void Save();
        void Load();
    }
}

