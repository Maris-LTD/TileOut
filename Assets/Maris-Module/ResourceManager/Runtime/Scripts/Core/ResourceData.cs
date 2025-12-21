using System;

namespace GameModules.ResourceManager
{
    [Serializable]
    public class ResourceData
    {
        public string ResourceId;
        public long Amount;
        public long LastRegenTimestamp;
        
        public ResourceData() { }
        
        public ResourceData(string resourceId, long amount = 0)
        {
            ResourceId = resourceId;
            Amount = amount;
            LastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}

