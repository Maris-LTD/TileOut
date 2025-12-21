using GameModules.Core;
using GameModules.UI.Core;
using GameModules.UI.Services;
using VContainer;
using VContainer.Unity;

namespace GameModules.UI
{
    [AutoInstall(ModuleScope.Scene, priority: 5)]
    public class UIInstaller : IGameModuleInstaller
    {
        public string ModuleName => "UI";

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<UIContainerManager>();
            builder.Register<UINavigationService>(Lifetime.Scoped).As<IUINavigationService>();
        }
    }
}

