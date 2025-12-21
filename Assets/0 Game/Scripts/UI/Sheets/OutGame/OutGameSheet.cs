using GameModules.Core;
using GameModules.UI.Base;

namespace Game.UI
{
    public class OutGameSheet : BaseSheet<OutGameSheetData, OutGameSheetResult>
    {
        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            var injectables = GetComponentsInChildren<IDependencyInjectable>(true);
            foreach (var injectable in injectables)
            {
                if (this as IDependencyInjectable == injectable)
                {
                    continue;
                }
                injectable.InjectDependencies(Resolver);
            }
        }
    }
}