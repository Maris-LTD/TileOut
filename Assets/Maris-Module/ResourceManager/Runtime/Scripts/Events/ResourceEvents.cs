using System;
using System.Collections.Generic;

namespace GameModules.ResourceManager
{
    public struct ResourceChangedEvent
    {
        public string ResourceId;
        public long OldAmount;
        public long NewAmount;
        public long Delta;
        
        public ResourceChangedEvent(string resourceId, long oldAmount, long newAmount)
        {
            ResourceId = resourceId;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            Delta = newAmount - oldAmount;
        }
    }
    
    public struct ResourceCacheChangedEvent
    {
        public string ResourceId;
        public long CachedAmount;
        
        public ResourceCacheChangedEvent(string resourceId, long cachedAmount)
        {
            ResourceId = resourceId;
            CachedAmount = cachedAmount;
        }
    }
    
    public struct ResourceCacheCommittedEvent
    {
        public Dictionary<string, long> CommittedResources;
        
        public ResourceCacheCommittedEvent(Dictionary<string, long> committedResources)
        {
            CommittedResources = committedResources;
        }
    }
    
    public struct ResourceCacheClearedEvent
    {
        public Dictionary<string, long> ClearedResources;
        
        public ResourceCacheClearedEvent(Dictionary<string, long> clearedResources)
        {
            ClearedResources = clearedResources;
        }
    }
    
    public struct EnergyRegeneratedEvent
    {
        public string ResourceId;
        public long CurrentAmount;
        public int MaxCapacity;
        public DateTime NextRegenTime;
        
        public EnergyRegeneratedEvent(string resourceId, long currentAmount, int maxCapacity, DateTime nextRegenTime)
        {
            ResourceId = resourceId;
            CurrentAmount = currentAmount;
            MaxCapacity = maxCapacity;
            NextRegenTime = nextRegenTime;
        }
    }
    
    public struct ResourceInitializedEvent
    {
        public IReadOnlyDictionary<string, long> AllResources;
        
        public ResourceInitializedEvent(IReadOnlyDictionary<string, long> allResources)
        {
            AllResources = allResources;
        }
    }
}

