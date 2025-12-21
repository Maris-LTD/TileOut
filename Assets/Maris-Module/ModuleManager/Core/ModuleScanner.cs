using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameModules.Core
{
    public static class ModuleScanner
    {
        private static List<Type> ScanAllInstaller()
        {
            var installerTypes = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a => !a.FullName.StartsWith("Unity"))
                .Where(a => !a.FullName.StartsWith("System"))
                .Where(a => !a.FullName.StartsWith("mscorlib"));

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t.IsClass)
                        .Where(t => !t.IsAbstract)
                        .Where(t => typeof(IGameModuleInstaller).IsAssignableFrom(t))
                        .Where(t => t.IsDefined(typeof(AutoInstallAttribute), false));

                    installerTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                }
            }

            return installerTypes;
        }

        private static List<Type> ScanAssembly(Assembly assembly)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass)
                    .Where(t => !t.IsAbstract)
                    .Where(t => typeof(IGameModuleInstaller).IsAssignableFrom(t))
                    .Where(t => t.IsDefined(typeof(AutoInstallAttribute), false));

                return types.ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return null;
            }
        }

        public static List<Type> ScanByScope(ModuleScope scope)
        {
            var allTypes = ScanAllInstaller();
            return allTypes.Where(t =>
            {
                var attr = t.GetCustomAttribute<AutoInstallAttribute>();
                return attr != null && attr.Scope == scope;
            }).ToList();
        }

        public static List<Type> ScanByScope(ModuleScope scope, Assembly assembly)
        {
            var allTypes = ScanAssembly(assembly);
            if (allTypes == null) return null;
            return allTypes.Where(t =>
            {
                var attr = t.GetCustomAttribute<AutoInstallAttribute>();
                return attr != null && attr.Scope == scope;
            }).ToList();
        }
    }
}