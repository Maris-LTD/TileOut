using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModules.ResourceManager
{
    [CreateAssetMenu(fileName = "ResourceRegistry", menuName = "GameModules/Resource Manager/Resource Registry")]
    public class ResourceRegistry : ScriptableObject
    {
        [Header("Resource Definitions")]
        public List<ResourceDefinition> Definitions = new();
        
        private Dictionary<string, ResourceDefinition> _definitionCache;
        
        public void Initialize()
        {
            _definitionCache = new Dictionary<string, ResourceDefinition>();
            foreach (var def in Definitions)
            {
                if (def != null && !string.IsNullOrEmpty(def.Id))
                {
                    _definitionCache[def.Id] = def;
                }
            }
        }
        
        public ResourceDefinition GetDefinition(string resourceId)
        {
            if (_definitionCache == null) Initialize();
            return _definitionCache.GetValueOrDefault(resourceId);
        }
        
        public IReadOnlyList<ResourceDefinition> GetAllDefinitions()
        {
            return Definitions.AsReadOnly();
        }
        
        public IReadOnlyList<ResourceDefinition> GetDefinitionsByCategory(ResourceCategory category)
        {
            return Definitions.Where(d => d != null && d.Category == category).ToList().AsReadOnly();
        }
        
        public IReadOnlyList<RegenerativeResourceDefinition> GetRegenerativeDefinitions()
        {
            return Definitions
                .Where(d => d is RegenerativeResourceDefinition)
                .Cast<RegenerativeResourceDefinition>()
                .ToList()
                .AsReadOnly();
        }
        
        public bool HasDefinition(string resourceId)
        {
            if (_definitionCache == null) Initialize();
            return _definitionCache.ContainsKey(resourceId);
        }
    }
}

