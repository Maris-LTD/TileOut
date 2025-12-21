namespace GameModules.CameraSystem.Installers
{
    using GameModules.CameraSystem.Core;
    using GameModules.Core;
    using VContainer;
    using VContainer.Unity;

    [AutoInstall(ModuleScope.Project)]
    public sealed class CameraSystemInstaller : IInstaller, IGameModuleInstaller
    {
        public string ModuleName => "CameraSystem";

        public void Install(IContainerBuilder builder)
        {
            builder.Register<ICameraRigRegistry, CameraRigRegistry>(Lifetime.Singleton);
            builder.Register<CameraDirectorService>(Lifetime.Singleton)
                .As<ICameraDirectorService>()
                .AsSelf();
        }
    }
}

