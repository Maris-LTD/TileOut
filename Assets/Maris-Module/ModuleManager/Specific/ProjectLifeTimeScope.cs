using GameModules.Core;
using VContainer;
using VContainer.Unity;

namespace GameModules.ModuleManager.Specific
{
    public class ProjectLifeTimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            AutoInjectProcessor.InjectTargets(Container, ModuleScope.Project);
            DontDestroyOnLoad(gameObject);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.AutoInstallModules(ModuleScope.Project);
        }
    }
}