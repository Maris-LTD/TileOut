using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VContainer.Unity;
using VContainer;

namespace GameModules.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AutoInjectAttribute : Attribute
    {
        public ModuleScope Scope { get; }

        public AutoInjectAttribute(ModuleScope scope = ModuleScope.Project)
        {
            Scope = scope;
        }
    }

    public static class AutoInjectProcessor
    {
        public static void InjectTargets(IObjectResolver resolver, ModuleScope scope, string sceneName = null)
        {
            if (resolver == null)
            {
                Debug.LogError("[AutoInject] Resolver is null, skip injection.");
                return;
            }

            var targets = FindTargets(scope, sceneName);
            var injectedInstanceIds = new HashSet<int>();

            foreach (var behaviour in targets)
            {
                if (behaviour == null) continue;
                var go = behaviour.gameObject;
                if (go == null) continue;

                if (!injectedInstanceIds.Add(go.GetInstanceID()))
                {
                    continue;
                }

                try
                {
                    resolver.InjectGameObject(go);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[AutoInject] ✗ Failed to inject {go.name}: {exception}");
                }
            }
        }

        private static IEnumerable<MonoBehaviour> FindTargets(ModuleScope scope, string sceneName)
        {
            var behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var behaviour in behaviours)
            {
                var attr = behaviour.GetType().GetCustomAttribute<AutoInjectAttribute>();
                if (attr == null) continue;

                switch (scope)
                {
                    case ModuleScope.Project when attr.Scope == ModuleScope.Project:
                        yield return behaviour;
                        break;
                    case ModuleScope.Scene:
                        if (attr.Scope == ModuleScope.Project)
                        {
                            yield return behaviour;
                        }
                        else if (attr.Scope == ModuleScope.Scene)
                        {
                            if (!string.IsNullOrEmpty(sceneName) &&
                                behaviour.gameObject != null &&
                                behaviour.gameObject.scene.name == sceneName)
                            {
                                yield return behaviour;
                            }
                        }
                        break;
                }
            }
        }
    }
}