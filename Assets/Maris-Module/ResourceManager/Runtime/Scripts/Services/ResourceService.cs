using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameModules.Systems.Events;
using VContainer;

namespace GameModules.ResourceManager
{
    public class ResourceService : IResourceService, IDisposable
    {
        private readonly ResourceRegistry _registry;
        private readonly IResourceStorage _storage;
        private readonly ResourceCache _cache;
        private readonly IGlobalEventBus _eventBus;
        
        private readonly Dictionary<string, ResourceData> _resources = new();
        private readonly Dictionary<string, float> _regenTimers = new();
        
        private bool _isInitialized;
        
        [Inject]
        public ResourceService(
            ResourceRegistry registry,
            IResourceStorage storage,
            ResourceCache cache,
            IGlobalEventBus eventBus)
        {
            _registry = registry;
            _storage = storage;
            _cache = cache;
            _eventBus = eventBus;
            
            Initialize();
        }
        
        private void Initialize()
        {
            if (_isInitialized) return;
            
            _registry.Initialize();
            Load();
            InitializeDefaultResources();
            ProcessOfflineRegeneration();
            StartRegenTimers().Forget();
            
            _isInitialized = true;
            
            var allResources = _resources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Amount);
            _eventBus.Publish(new ResourceInitializedEvent(allResources));
        }
        
        private void InitializeDefaultResources()
        {
            foreach (var definition in _registry.GetAllDefinitions())
            {
                if (!_resources.ContainsKey(definition.Id))
                {
                    _resources[definition.Id] = new ResourceData(definition.Id, definition.DefaultAmount);
                }
            }
        }
        
        private void ProcessOfflineRegeneration()
        {
            foreach (var regenDef in _registry.GetRegenerativeDefinitions())
            {
                if (!_resources.TryGetValue(regenDef.Id, out var data)) continue;
                
                var oldAmount = data.Amount;
                var newAmount = regenDef.CalculateOfflineRegen(data.Amount, data.LastRegenTimestamp);
                
                if (newAmount != oldAmount)
                {
                    data.Amount = newAmount;
                    data.LastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    
                    _eventBus.Publish(new ResourceChangedEvent(regenDef.Id, oldAmount, newAmount));
                }
            }
            
            Save();
        }
        
        private async UniTaskVoid StartRegenTimers()
        {
            while (_isInitialized)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                ProcessRegeneration();
            }
        }
        
        private void ProcessRegeneration()
        {
            foreach (var regenDef in _registry.GetRegenerativeDefinitions())
            {
                if (!_resources.TryGetValue(regenDef.Id, out var data)) continue;
                if (!regenDef.CanRegenerate(data.Amount)) continue;
                
                var secondsUntilRegen = regenDef.GetSecondsUntilNextRegen(data.LastRegenTimestamp);
                
                if (secondsUntilRegen <= 0)
                {
                    var oldAmount = data.Amount;
                    data.Amount = Math.Min(data.Amount + regenDef.RegenAmount, regenDef.MaxCapacity);
                    data.LastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    
                    var nextRegenTime = DateTime.UtcNow.AddSeconds(regenDef.RegenIntervalSeconds);
                    _eventBus.Publish(new EnergyRegeneratedEvent(regenDef.Id, data.Amount, regenDef.MaxCapacity, nextRegenTime));
                    _eventBus.Publish(new ResourceChangedEvent(regenDef.Id, oldAmount, data.Amount));
                    
                    Save();
                }
            }
        }
        
        public void Add(string resourceId, long amount)
        {
            if (amount <= 0) return;
            
            var definition = _registry.GetDefinition(resourceId);
            if (definition == null) return;
            
            if (!_resources.TryGetValue(resourceId, out var data))
            {
                data = new ResourceData(resourceId, 0);
                _resources[resourceId] = data;
            }
            
            var oldAmount = data.Amount;
            data.Amount = definition.ClampAmount(data.Amount + amount);
            
            if (definition.IsRegenerative)
            {
                data.LastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            
            _eventBus.Publish(new ResourceChangedEvent(resourceId, oldAmount, data.Amount));
            Save();
        }
        
        public bool Remove(string resourceId, long amount)
        {
            if (amount <= 0) return true;
            if (!Has(resourceId, amount)) return false;
            
            var definition = _registry.GetDefinition(resourceId);
            if (definition == null) return false;
            
            var data = _resources[resourceId];
            var oldAmount = data.Amount;
            data.Amount = definition.ClampAmount(data.Amount - amount);
            
            _eventBus.Publish(new ResourceChangedEvent(resourceId, oldAmount, data.Amount));
            Save();
            
            return true;
        }
        
        public void Set(string resourceId, long amount)
        {
            var definition = _registry.GetDefinition(resourceId);
            if (definition == null) return;
            
            if (!_resources.TryGetValue(resourceId, out var data))
            {
                data = new ResourceData(resourceId, 0);
                _resources[resourceId] = data;
            }
            
            var oldAmount = data.Amount;
            data.Amount = definition.ClampAmount(amount);
            
            if (definition.IsRegenerative)
            {
                data.LastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            
            _eventBus.Publish(new ResourceChangedEvent(resourceId, oldAmount, data.Amount));
            Save();
        }
        
        public long Get(string resourceId)
        {
            return _resources.TryGetValue(resourceId, out var data) ? data.Amount : 0;
        }
        
        public bool Has(string resourceId, long amount)
        {
            return Get(resourceId) >= amount;
        }
        
        public void CacheAdd(string resourceId, long amount)
        {
            _cache.Add(resourceId, amount);
            _eventBus.Publish(new ResourceCacheChangedEvent(resourceId, _cache.Get(resourceId)));
        }
        
        public void CacheRemove(string resourceId, long amount)
        {
            _cache.Remove(resourceId, amount);
            _eventBus.Publish(new ResourceCacheChangedEvent(resourceId, _cache.Get(resourceId)));
        }
        
        public long GetCached(string resourceId)
        {
            return _cache.Get(resourceId);
        }
        
        public Dictionary<string, long> GetAllCached()
        {
            return _cache.GetAll();
        }
        
        public void CommitCache()
        {
            var cachedData = _cache.GetAll();
            
            foreach (var kvp in cachedData)
            {
                if (kvp.Value > 0)
                    Add(kvp.Key, kvp.Value);
                else if (kvp.Value < 0)
                    Remove(kvp.Key, Math.Abs(kvp.Value));
            }
            
            _eventBus.Publish(new ResourceCacheCommittedEvent(cachedData));
            _cache.Clear();
        }
        
        public void ClearCache()
        {
            var cachedData = _cache.GetAll();
            _cache.Clear();
            _eventBus.Publish(new ResourceCacheClearedEvent(cachedData));
        }
        
        public bool HasCachedData()
        {
            return _cache.HasData();
        }
        
        public DateTime GetNextRegenTime(string resourceId)
        {
            var definition = _registry.GetDefinition(resourceId);
            if (definition is not RegenerativeResourceDefinition regenDef) return DateTime.MaxValue;
            
            if (!_resources.TryGetValue(resourceId, out var data)) return DateTime.MaxValue;
            
            var secondsUntilRegen = regenDef.GetSecondsUntilNextRegen(data.LastRegenTimestamp);
            return DateTime.UtcNow.AddSeconds(secondsUntilRegen);
        }
        
        public float GetSecondsUntilNextRegen(string resourceId)
        {
            var definition = _registry.GetDefinition(resourceId);
            if (definition is not RegenerativeResourceDefinition regenDef) return float.MaxValue;
            
            if (!_resources.TryGetValue(resourceId, out var data)) return float.MaxValue;
            
            return regenDef.GetSecondsUntilNextRegen(data.LastRegenTimestamp);
        }
        
        public ResourceDefinition GetDefinition(string resourceId)
        {
            return _registry.GetDefinition(resourceId);
        }
        
        public IReadOnlyList<ResourceDefinition> GetAllDefinitions()
        {
            return _registry.GetAllDefinitions();
        }
        
        public IReadOnlyList<ResourceDefinition> GetDefinitionsByCategory(ResourceCategory category)
        {
            return _registry.GetDefinitionsByCategory(category);
        }
        
        public void Save()
        {
            _storage.Save(_resources);
        }
        
        public void Load()
        {
            var loadedData = _storage.Load();
            if (loadedData != null)
            {
                foreach (var kvp in loadedData)
                {
                    _resources[kvp.Key] = kvp.Value;
                }
            }
        }
        
        public void Dispose()
        {
            _isInitialized = false;
            Save();
        }
    }
}

