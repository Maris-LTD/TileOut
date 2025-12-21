using GameModules.Core;
using GameModules.Systems.Events;
using IAPModule.Core;
using IAPModule.Data;
using IAPModule.Events;
using IAPModule.Interfaces;
using UnityEngine;
using VContainer;

namespace IAPModule.Installers
{
    [AutoInstall(ModuleScope.Project, priority: 10)]
    public class IAPInstaller : IGameModuleInstaller
    {
        public string ModuleName => "IAP";

        public void Install(IContainerBuilder builder)
        {
            var config = LoadIAPConfig();
            if (config == null)
            {
                Debug.LogWarning("[IAPInstaller] IAPConfig not found in Resources folder. IAP will use default settings.");
            }
            else
            {
                builder.RegisterInstance(config);
            }

            RegisterIAPManager(builder, config);
        }

        private IAPConfig LoadIAPConfig()
        {
            return Resources.Load<IAPConfig>("IAPConfig");
        }

        private void RegisterIAPManager(IContainerBuilder builder, IAPConfig config)
        {
            builder.Register(container =>
            {
                var environment = config?.Environment ?? "production";
                var enableReceiptValidation = config?.EnableReceiptValidation ?? false;
                var productDataList = config?.ProductDataList ?? new System.Collections.Generic.List<IAPProductData>();

                var iapManager = new IAPManager(environment, enableReceiptValidation, productDataList);
                return iapManager;
            }, Lifetime.Singleton).As<IIAPManager>();
        }
    }
}
