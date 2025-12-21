using GameModules.Core;
using GameModules.Spawner.Core;
using VContainer;

namespace GameModules.Spawner.Installers
{
    [AutoInstall(ModuleScope.Scene)]
    public sealed class SpawnerInstaller : IGameModuleInstaller
    {
        public string ModuleName => "Spawner";

        public void Install(IContainerBuilder builder)
        {
            builder.Register<SpawnerService>(Lifetime.Scoped)
                .As<ISpawner>()
                .AsSelf();
        }
    }
}

