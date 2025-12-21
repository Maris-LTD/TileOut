using GameModules.Core;
using VContainer;
using VContainer.Unity;

namespace GameModules.ModuleManager.Specific
{
    public class SceneLifetimeScope : LifetimeScope
    {
        private string _sceneName;

        protected override void Awake()
        {
            base.Awake();
            AutoInjectProcessor.InjectTargets(Container, ModuleScope.Scene, _sceneName);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            _sceneName = gameObject.scene.name;
            builder.AutoInstallModules(ModuleScope.Scene, _sceneName);
        }
    }
}