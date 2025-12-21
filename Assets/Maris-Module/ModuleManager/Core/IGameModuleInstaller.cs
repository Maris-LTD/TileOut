using VContainer;

namespace GameModules.Core
{
    public interface IGameModuleInstaller
    {
        string ModuleName { get; }
        void Install(IContainerBuilder builder);
    }
}