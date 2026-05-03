using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules.DataManager
{
    [CreateAssetMenu(fileName = "DataProviderRegistry", menuName = "GameModules/Data Provider Registry")]
    public class DataProviderRegistry : ScriptableObject
    {
        [Serializable]
        public class ProviderEntry
        {
            [Header("Module Information")]
            [Tooltip("Name of the module containing this provider")]
            public string moduleName;
            
            [Tooltip("The full name of the provider class (including namespace))")]
            public string className;
            
            [Tooltip("Description of provider function")]
            public string description;
            
            [Header("Settings")]
            [Tooltip("Is the provider enabled?")]
            public bool isEnabled = true;
            
            [Tooltip("Priority when loading/saving data (smaller number, higher priority)")]
            public int priority;
        }
        
        [Header("Data Provider Configuration")]
        [Tooltip("List of registered DataProviders")]
        public List<ProviderEntry> providers = new List<ProviderEntry>();
        
        [Header("Settings")]
        [Tooltip("Does it automatically scan and detect new providers?")]
        public bool autoDiscoverProviders = true;
        
        [Tooltip("Is there a class name validation on initialization?")]
        public bool validateClassNames = true;
        
        public void AddProvider(string moduleName, string className, string description = "", int priority = 0)
        {
            var entry = new ProviderEntry
            {
                moduleName = moduleName,
                className = className,
                description = description,
                priority = priority,
                isEnabled = true
            };
            
            providers.Add(entry);
        }
        
        public bool RemoveProvider(string className)
        {
            for (int i = 0; i < providers.Count; i++)
            {
                if (providers[i].className == className)
                {
                    providers.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        
        public ProviderEntry FindProvider(string className)
        {
            return providers.Find(p => p.className == className);
        }
        
        public List<ProviderEntry> GetEnabledProviders()
        {
            var enabledProviders = providers.FindAll(p => p.isEnabled);
            enabledProviders.Sort((a, b) => a.priority.CompareTo(b.priority));
            return enabledProviders;
        }
        
        public List<string> ValidateClassNames()
        {
            var invalidClasses = new List<string>();
            
            foreach (var provider in providers)
            {
                if (!string.IsNullOrEmpty(provider.className))
                {
                    var type = Type.GetType(provider.className) ?? FindTypeByName(provider.className);
                    if (type == null)
                    {
                        invalidClasses.Add(provider.className);
                    }
                }
            }
            
            return invalidClasses;
        }

        private Type FindTypeByName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
