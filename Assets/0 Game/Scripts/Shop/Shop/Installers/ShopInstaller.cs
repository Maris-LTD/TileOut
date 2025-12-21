using GameModules.Core;
using GameModules.DataManager;
using GameModules.ResourceManager;
using GameModules.UI.Services;
using IAPModule.Adapters.Shop.Data;
using IAPModule.Adapters.Shop.Rewards;
using IAPModule.Adapters.Shop.Services;
using VContainer;

namespace IAPModule.Adapters.Shop.Installers
{
    [AutoInstall(ModuleScope.Scene, priority: 15)]
    public class ShopInstaller : IGameModuleInstaller
    {
        public string ModuleName => "Shop";

        public void Install(IContainerBuilder builder)
        {
            RegisterShopDataProvider(builder);
            RegisterRewardHandler(builder);
            RegisterShopPurchaseService(builder);
            RegisterDataProvider(builder);
            InitializeRewardHandlers(builder);
        }

        private void RegisterShopDataProvider(IContainerBuilder builder)
        {
            builder.Register(container =>
            {
                var dataProvider = new ShopDataProvider();
                return dataProvider;
            }, Lifetime.Singleton);
        }

        private void RegisterRewardHandler(IContainerBuilder builder)
        {
            builder.Register(container =>
            {
                var resourceService = container.Resolve<IResourceService>();
                var uiNavigationService = container.Resolve<IUINavigationService>();
                var handler = new RewardHandler(resourceService, uiNavigationService);
                return handler;
            }, Lifetime.Singleton).As<IRewardHandler>();
        }

        private void RegisterShopPurchaseService(IContainerBuilder builder)
        {
            builder.Register(container =>
            {
                var dataProvider = container.Resolve<ShopDataProvider>();
                var service = new ShopPurchaseService(dataProvider);
                return service;
            }, Lifetime.Singleton).As<IShopPurchaseService>();
        }

        private void RegisterDataProvider(IContainerBuilder builder)
        {
            builder.Register(container =>
            {
                var dataProvider = container.Resolve<ShopDataProvider>();
                return dataProvider;
            }, Lifetime.Singleton).As<IDataProvider<ShopPurchaseHistory>>();
        }

        private void InitializeRewardHandlers(IContainerBuilder builder)
        {
            builder.RegisterBuildCallback(container =>
            {
                var rewardHandler = container.Resolve<IRewardHandler>();
                CoinRewardPayload.SetRewardHandler(rewardHandler);
                BundleRewardPayload.SetRewardHandler(rewardHandler);
            });
        }
    }
}
