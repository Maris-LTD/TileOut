using GameModules.Core;
using GameModules.Systems.Events;
using UnityEngine;
using VContainer;

namespace GameModules.ResourceManager
{
    [AutoInstall(ModuleScope.Project, priority: 5)]
    public class ResourceInstaller : IGameModuleInstaller
    {
        public string ModuleName => "ResourceManager";
        
        public void Install(IContainerBuilder builder)
        {
            var registry = Resources.Load<ResourceRegistry>("ResourceRegistry");
            if (registry == null)
            {
                Debug.LogError("[ResourceInstaller] ResourceRegistry not found in Resources folder!");
                return;
            }
            
            builder.RegisterInstance(registry);
            builder.Register<IResourceStorage, PlayerPrefsResourceStorage>(Lifetime.Singleton);
            builder.Register<ResourceCache>(Lifetime.Singleton);
            builder.Register<IResourceService, ResourceService>(Lifetime.Singleton);
        }
    }
}

