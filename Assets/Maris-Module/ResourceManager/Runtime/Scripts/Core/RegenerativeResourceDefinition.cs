using UnityEngine;

namespace GameModules.ResourceManager
{
    [CreateAssetMenu(fileName = "RegenerativeResourceDefinition", menuName = "GameModules/Resource Manager/Regenerative Resource Definition")]
    public class RegenerativeResourceDefinition : ResourceDefinition
    {
        [Header("Regeneration Settings")]
        public int MaxCapacity = 60;
        public int OverflowCapacity = 100;
        public float RegenIntervalSeconds = 300f;
        public int RegenAmount = 1;
        
        public override bool IsRegenerative => true;
        
        public override long ClampAmount(long amount)
        {
            return (long)Mathf.Clamp(amount, 0, OverflowCapacity);
        }
        
        public bool CanRegenerate(long currentAmount)
        {
            return currentAmount < MaxCapacity;
        }
        
        public long CalculateOfflineRegen(long currentAmount, long lastRegenTimestamp)
        {
            if (currentAmount >= MaxCapacity) return currentAmount;
            
            var now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var elapsedSeconds = now - lastRegenTimestamp;
            
            if (elapsedSeconds <= 0) return currentAmount;
            
            var regenCycles = (int)(elapsedSeconds / RegenIntervalSeconds);
            var regenTotal = regenCycles * RegenAmount;
            
            var newAmount = currentAmount + regenTotal;
            return (long)Mathf.Min(newAmount, MaxCapacity);
        }
        
        public float GetSecondsUntilNextRegen(long lastRegenTimestamp)
        {
            var now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var elapsedSeconds = now - lastRegenTimestamp;
            var remainder = elapsedSeconds % RegenIntervalSeconds;
            return RegenIntervalSeconds - remainder;
        }
    }
}

