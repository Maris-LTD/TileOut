using UnityEngine;

namespace GameModules.ResourceManager
{
    [CreateAssetMenu(fileName = "ResourceDefinition", menuName = "GameModules/Resource Manager/Resource Definition")]
    public class ResourceDefinition : ScriptableObject
    {
        [Header("Identification")]
        public string Id;
        public string DisplayName;
        
        [Header("Visual")]
        public Sprite Icon;
        
        [Header("Settings")]
        public ResourceCategory Category;
        public long DefaultAmount;
        public long MaxAmount;
        
        public virtual bool IsRegenerative => false;
        
        public virtual long ClampAmount(long amount)
        {
            if (MaxAmount > 0)
                return (long)Mathf.Clamp(amount, 0, MaxAmount);
            return (long)Mathf.Max(amount, 0);
        }
    }
}

