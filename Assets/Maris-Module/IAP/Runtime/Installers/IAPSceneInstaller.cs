using GameModules.Core;
using IAPModule.Adapters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace IAPModule.Installers
{
    [AutoInstall(ModuleScope.Scene, priority: 10)]
    public class IAPSceneInstaller : IGameModuleInstaller
    {
        public string ModuleName => "IAP Scene";

        public void Install(IContainerBuilder builder)
        {
            var iapGameAdapter = Object.FindAnyObjectByType<IAPGameAdapter>();
            if (iapGameAdapter != null)
                builder.RegisterComponentInHierarchy<IAPGameAdapter>();
        }
    }
}