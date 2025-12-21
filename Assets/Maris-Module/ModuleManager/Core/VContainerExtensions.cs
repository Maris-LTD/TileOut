using VContainer;

namespace GameModules.Core
{
    public static class VContainerExtensions
    {
        public static IContainerBuilder AutoInstallModules(this IContainerBuilder builder, ModuleScope scope)
        {
            AutoInstaller.InstallModules(builder: builder, scope: scope);
            return builder;
        }

        public static IContainerBuilder AutoInstallModules(this IContainerBuilder builder
            , ModuleScope scope
            , string sceneName)
        {
            AutoInstaller.InstallModules(builder: builder, scope: scope, sceneName: sceneName);
            return builder;
        }
    }
}