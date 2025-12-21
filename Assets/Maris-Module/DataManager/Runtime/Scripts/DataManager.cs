using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameModules.DataManager
{
    /// <summary>
    /// DataManager - Central management of DataProviders
    /// Use reflection to automatically discover and manage providers
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private DataProviderRegistry registry;

        [Header("Debug")] [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool validateProvidersOnStart = true;

        // Dictionary lưu trữ các provider instance
        private readonly Dictionary<string, object> _providers = new();

        // Dictionary lưu trữ các method info của provider
        private readonly Dictionary<string, ProviderMethodInfo> _providerMethods = new();

        // Events
        public event Action<string> OnProviderRegistered;
        public event Action<string> OnProviderUnregistered;
        public event Action<string> OnDataLoaded;
        public event Action<string> OnDataSaved;

        /// <summary>
        /// Thông tin về các method của provider
        /// </summary>
        private class ProviderMethodInfo
        {
            public MethodInfo LoadDataMethod;
            public MethodInfo GetDataMethod;
            public MethodInfo SaveDataMethod;
            public MethodInfo HasDataMethod;
            public MethodInfo ClearDataMethod;
            public Type ProviderType;
        }

        private void Awake()
        {
            if (registry == null)
            {
                Debug.LogError("[DataManager] Registry not assigned! Please create and assign DataProviderRegistry.");
                return;
            }

            if (validateProvidersOnStart)
            {
                ValidateRegistry();
            }
        }

        private void Start() { InitializeProviders(); }

        /// <summary>
        /// Khởi tạo tất cả các provider từ registry
        /// </summary>
        private void InitializeProviders()
        {
            if (registry == null) return;

            var enabledProviders = registry.GetEnabledProviders();

            foreach (var entry in enabledProviders)
            {
                try
                {
                    RegisterProvider(entry.className);
                }
                catch (Exception ex)
                {
                    LogError($"Unable to register provider {entry.className}: {ex.Message}");
                }
            }

            LogInfo($"{_providers.Count} provider initialized.");
        }

        private bool RegisterProvider(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                LogError("Class name cannot be empty");
                return false;
            }

            if (_providers.ContainsKey(className))
            {
                LogWarning($"Provider {className} has been registered before");
                return true;
            }

            try
            {
                var providerType = Type.GetType(className) ?? FindTypeByName(className);

                if (providerType == null)
                {
                    LogError($"Could not find type {className}");
                    return false;
                }

                var methodInfo = ValidateProviderType(providerType);
                if (methodInfo == null)
                {
                    LogError($"Type {className} does not have all the required methods");
                    return false;
                }

                object providerInstance = Activator.CreateInstance(providerType);

                _providers[className] = providerInstance;
                _providerMethods[className] = methodInfo;

                LogInfo($"Registered provider: {className}");
                OnProviderRegistered?.Invoke(className);

                return true;
            }
            catch (Exception ex)
            {
                LogError($"Error registering provider {className}: {ex.Message}");
                return false;
            }
        }
        
        public bool UnregisterProvider(string className)
        {
            if (_providers.Remove(className))
            {
                _providerMethods.Remove(className);
                LogInfo($"Provider registration has been cancelled.: {className}");
                OnProviderUnregistered?.Invoke(className);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Load data vào một provider cụ thể
        /// </summary>
        public bool LoadData(string className, object data)
        {
            if (!_providers.TryGetValue(className, out var provider))
            {
                LogError($"Provider {className} is not registered");
                return false;
            }

            try
            {
                var methodInfo = _providerMethods[className];

                bool result = (bool)methodInfo.LoadDataMethod.Invoke(provider, new[] { data });

                if (result)
                {
                    LogInfo($"Đã load data vào provider: {className}");
                    OnDataLoaded?.Invoke(className);
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError($"Lỗi khi load data vào provider {className}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load data vào tất cả các provider
        /// </summary>
        public void LoadAllData(object data)
        {
            foreach (var className in _providers.Keys.ToList())
            {
                LoadData(className, data);
            }
        }

        /// <summary>
        /// Lấy data từ một provider
        /// </summary>
        public object GetData(string className)
        {
            if (!_providers.TryGetValue(className, out var provider))
            {
                LogError($"Provider {className} not registered");
                return null;
            }

            try
            {
                var methodInfo = _providerMethods[className];

                return methodInfo.GetDataMethod.Invoke(provider, null);
            }
            catch (Exception ex)
            {
                LogError($"Error when getting data from provider {className}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lưu data từ một provider
        /// </summary>
        public object SaveData(string className)
        {
            if (!_providers.TryGetValue(className, out var provider))
            {
                LogError($"Provider {className} is not registered");
                return null;
            }

            try
            {
                var methodInfo = _providerMethods[className];

                var result = methodInfo.SaveDataMethod.Invoke(provider, null);

                LogInfo($"Đã lưu data từ provider: {className}");
                OnDataSaved?.Invoke(className);

                return result;
            }
            catch (Exception ex)
            {
                LogError($"Lỗi khi lưu data từ provider {className}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lưu data từ tất cả các provider
        /// </summary>
        public Dictionary<string, object> SaveAllData()
        {
            var results = new Dictionary<string, object>();

            foreach (var className in _providers.Keys.ToList())
            {
                var data = SaveData(className);
                if (data != null)
                {
                    results[className] = data;
                }
            }

            return results;
        }
        
        public bool HasData(string className)
        {
            if (!_providers.TryGetValue(className, out var provider))
            {
                return false;
            }

            try
            {
                var methodInfo = _providerMethods[className];

                return (bool)methodInfo.HasDataMethod.Invoke(provider, null);
            }
            catch (Exception ex)
            {
                LogError($"Error checking provider data {className}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa data của một provider
        /// </summary>
        public void ClearData(string className)
        {
            if (!_providers.TryGetValue(className, out var provider))
            {
                return;
            }

            try
            {
                var methodInfo = _providerMethods[className];

                methodInfo.ClearDataMethod.Invoke(provider, null);
                LogInfo($"Đã xóa data của provider: {className}");
            }
            catch (Exception ex)
            {
                LogError($"Lỗi khi xóa data của provider {className}: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa data của tất cả các provider
        /// </summary>
        public void ClearAllData()
        {
            foreach (var className in _providers.Keys.ToList())
            {
                ClearData(className);
            }
        }

        public List<string> GetRegisteredProviders() { return _providers.Keys.ToList(); }

        public bool IsProviderRegistered(string className) { return _providers.ContainsKey(className); }

        private ProviderMethodInfo ValidateProviderType(Type type)
        {
            var methodInfo = new ProviderMethodInfo
            {
                ProviderType = type
                , LoadDataMethod = type.GetMethod("LoadData", BindingFlags.Public | BindingFlags.Instance)
                , GetDataMethod = type.GetMethod("GetData", BindingFlags.Public | BindingFlags.Instance)
                , SaveDataMethod = type.GetMethod("SaveData", BindingFlags.Public | BindingFlags.Instance)
                , HasDataMethod = type.GetMethod("HasData", BindingFlags.Public | BindingFlags.Instance)
                , ClearDataMethod = type.GetMethod("ClearData", BindingFlags.Public | BindingFlags.Instance)
            };

            if (methodInfo.LoadDataMethod == null || methodInfo.GetDataMethod == null ||
                methodInfo.SaveDataMethod == null || methodInfo.HasDataMethod == null ||
                methodInfo.ClearDataMethod == null)
            {
                return null;
            }

            return methodInfo;
        }

        /// <summary>
        /// Tìm type theo tên trong tất cả assembly
        /// </summary>
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

        /// <summary>
        /// Validate registry
        /// </summary>
        private void ValidateRegistry()
        {
            if (registry == null) return;

            var invalidClasses = registry.ValidateClassNames();
            if (invalidClasses.Count > 0)
            {
                LogWarning($"There are invalid {invalidClasses.Count} class names in the registry:");
                foreach (var className in invalidClasses)
                {
                    LogWarning($"  - {className}");
                }
            }
        }

        #region Logging Methods

        private void LogInfo(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[DataManager] {message}");
            }
        }

        private void LogWarning(string message)
        {
            if (enableLogging)
            {
                Debug.LogWarning($"[DataManager] {message}");
            }
        }

        private void LogError(string message)
        {
            if (enableLogging)
            {
                Debug.LogError($"[DataManager] {message}");
            }
        }

        #endregion

        private void OnDestroy()
        {
            // Cleanup
            _providers.Clear();
            _providerMethods.Clear();
        }
    }
}