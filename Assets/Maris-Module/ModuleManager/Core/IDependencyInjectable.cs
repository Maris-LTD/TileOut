using VContainer;

namespace GameModules.Core
{
    public interface IDependencyInjectable
    {
        void InjectDependencies(IObjectResolver resolver);
    }
}

