using GameModules.Core;
using MessagePipe;

namespace GameModules.Systems.Events
{
    using VContainer;
    using VContainer.Unity;

    [AutoInstall]
    public class EventSystemInstaller : IInstaller, IGameModuleInstaller
    {
        public string ModuleName => "EventSystem";
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.Register(container => new EventBus(container), Lifetime.Singleton)
                .As<IEventBus, IGlobalEventBus>();
        }
    }
}
