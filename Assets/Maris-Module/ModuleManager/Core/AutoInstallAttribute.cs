using System;

namespace GameModules.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AutoInstallAttribute : Attribute
    {
        public ModuleScope Scope { get; set; }
        
        /// <summary>
        /// Smaller numbers have higher priority
        /// </summary>
        public int Priority { get; set; }
        
        public bool Enabled { get; set; }
        
        public string SceneName { get; set; }
        
        public AutoInstallAttribute(ModuleScope scope = ModuleScope.Project, int priority = 0, bool enabled = true, string sceneName = null)
        {
            Scope = scope;
            Priority = priority;
            Enabled = enabled;
            SceneName = sceneName;
        }
    }
}