using GameModules.Core;
using VContainer;
using VContainer.Unity;

namespace GameModules.DataManager
{
    [AutoInstall(ModuleScope.Project, priority: -5)]
    public class DataManagerInstaller : IGameModuleInstaller
    {
        public string ModuleName => "DataManager";

        public void Install(IContainerBuilder builder)
        {
            // Tìm component DataManager có sẵn trên Scene và đưa nó vào DI Container
            builder.RegisterComponentInHierarchy<DataManager>().AsSelf();
        }
    }
}
