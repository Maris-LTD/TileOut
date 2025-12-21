using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModules.Core
{
    public class InstallerInfo
    {
        public Type Type { get; set; }
        public AutoInstallAttribute AutoInstallAttribute { get; set; }
        public int Priority => AutoInstallAttribute?.Priority ?? 0;
        public string SceneName => AutoInstallAttribute?.SceneName ?? string.Empty;
        public bool Enabled => AutoInstallAttribute?.Enabled ?? true;
    }
    
    public class ModuleRegistry
    {
        private static ModuleRegistry _instance;
        public static ModuleRegistry Instance => _instance ??= new ModuleRegistry();

        private Dictionary<ModuleScope, List<InstallerInfo>> _installers;
        private bool _isInitialized;
        
        public void Initialize()
        {
            _isInitialized = true;
            _installers = new();
            var enumValues = Enum.GetValues(typeof(ModuleScope));
            foreach (ModuleScope scope in enumValues)
            {
                _installers.Add(scope, new List<InstallerInfo>());
                var installerInfos = ModuleScanner.ScanByScope(scope);
                foreach (var installerInfo in installerInfos)
                {
                    _installers[scope].Add(new InstallerInfo()
                    {
                        Type = installerInfo
                        , AutoInstallAttribute
                            = (AutoInstallAttribute)Attribute.GetCustomAttribute(installerInfo
                                , typeof(AutoInstallAttribute))
                    });
                }
                _installers[scope].Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }
        
        public List<InstallerInfo> GetInstallers(ModuleScope scope)
        {
            return _installers.GetValueOrDefault(scope);
        }

        public List<InstallerInfo> GetInstallers(ModuleScope scope, string sceneName)
        {
            return _installers.GetValueOrDefault(scope).Where(x => x.SceneName == sceneName || x.SceneName == "").ToList();
        }
        
        public void Clear()
        {
            _installers.Clear();
        }
        
        public bool IsInitialized => _isInitialized;
    }
}