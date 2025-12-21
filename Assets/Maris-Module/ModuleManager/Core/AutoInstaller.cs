using System;
using UnityEngine;
using VContainer;

namespace GameModules.Core
{
    public static class AutoInstaller
    {
        public static void InstallModules(IContainerBuilder builder, ModuleScope scope)
        {
            if (!ModuleRegistry.Instance.IsInitialized)
            {
                ModuleRegistry.Instance.Initialize();
            }
            
            var installers = ModuleRegistry.Instance.GetInstallers(scope);
            foreach (var info in installers)
            {
                var installer = (IGameModuleInstaller)Activator.CreateInstance(info.Type);
                SafeInstall(builder, installer);
            }
        }

        public static void InstallModules(IContainerBuilder builder, ModuleScope scope, string sceneName)
        {
            if (!ModuleRegistry.Instance.IsInitialized)
            {
                ModuleRegistry.Instance.Initialize();
            }
            
            var installers = ModuleRegistry.Instance.GetInstallers(scope, sceneName);
            foreach (var info in installers)
            {
                var installer = (IGameModuleInstaller)Activator.CreateInstance(info.Type);
                SafeInstall(builder, installer);
            }
        }
        
        
        private static void SafeInstall(IContainerBuilder builder, IGameModuleInstaller installer)
        {
            try
            {
                installer.Install(builder);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoInstaller] ✗ Failed to install {installer.ModuleName}: {e}");
                throw; // Re-throw để caller biết có lỗi
            }
        }
    }
}