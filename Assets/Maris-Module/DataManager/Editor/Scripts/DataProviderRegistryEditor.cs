using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace GameModules.DataManager.Editor
{
    [CustomEditor(typeof(DataProviderRegistry))]
    public class DataProviderRegistryEditor : UnityEditor.Editor
    {
        private DataProviderRegistry registry;
        private Vector2 scrollPosition;
        private bool showAddProvider = false;
        private string newModuleName = "";
        private string newClassName = "";
        private string newDescription = "";
        private int newPriority = 0;
        
        private void OnEnable()
        {
            registry = (DataProviderRegistry)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data Provider Registry", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Settings
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDiscoverProviders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("validateClassNames"));
            
            EditorGUILayout.Space();
            
            // Add Provider Section
            DrawAddProviderSection();
            
            EditorGUILayout.Space();
            
            // Providers List
            DrawProvidersList();
            
            // Validation
            DrawValidationSection();
            
            // Buttons
            DrawActionButtons();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawAddProviderSection()
        {
            EditorGUILayout.LabelField("Add New Provider", EditorStyles.boldLabel);
            
            showAddProvider = EditorGUILayout.Foldout(showAddProvider, "Show Add Provider Form");
            
            if (showAddProvider)
            {
                EditorGUI.indentLevel++;
                
                newModuleName = EditorGUILayout.TextField("Module Name", newModuleName);
                newClassName = EditorGUILayout.TextField("Class Name", newClassName);
                newDescription = EditorGUILayout.TextField("Description", newDescription);
                newPriority = EditorGUILayout.IntField("Priority", newPriority);
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Add Provider"))
                {
                    AddNewProvider();
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawProvidersList()
        {
            EditorGUILayout.LabelField("Registered Providers", EditorStyles.boldLabel);
            
            var providersProperty = serializedObject.FindProperty("providers");
            
            if (providersProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Chưa có provider nào được đăng ký.", MessageType.Info);
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            for (int i = 0; i < providersProperty.arraySize; i++)
            {
                var providerProperty = providersProperty.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginVertical("box");
                
                // Header
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Provider {i + 1}", EditorStyles.boldLabel);
                
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    RemoveProvider(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
                
                // Fields
                EditorGUI.indentLevel++;
                
                var moduleNameProperty = providerProperty.FindPropertyRelative("moduleName");
                var classNameProperty = providerProperty.FindPropertyRelative("className");
                var descriptionProperty = providerProperty.FindPropertyRelative("description");
                var isEnabledProperty = providerProperty.FindPropertyRelative("isEnabled");
                var priorityProperty = providerProperty.FindPropertyRelative("priority");
                
                EditorGUILayout.PropertyField(moduleNameProperty);
                EditorGUILayout.PropertyField(classNameProperty);
                EditorGUILayout.PropertyField(descriptionProperty);
                EditorGUILayout.PropertyField(isEnabledProperty);
                EditorGUILayout.PropertyField(priorityProperty);
                
                EditorGUI.indentLevel--;
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawValidationSection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Validate Class Names"))
            {
                ValidateClassNames();
            }
            
            if (GUILayout.Button("Auto-Discover Providers"))
            {
                AutoDiscoverProviders();
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear All Providers"))
            {
                if (EditorUtility.DisplayDialog("Confirm", "Bạn có chắc muốn xóa tất cả providers?", "Yes", "No"))
                {
                    ClearAllProviders();
                }
            }
            
            if (GUILayout.Button("Sort by Priority"))
            {
                SortProvidersByPriority();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void AddNewProvider()
        {
            if (string.IsNullOrEmpty(newModuleName) || string.IsNullOrEmpty(newClassName))
            {
                EditorUtility.DisplayDialog("Error", "Module Name và Class Name không được để trống!", "OK");
                return;
            }
            
            // Kiểm tra xem class name đã tồn tại chưa
            var existingProvider = registry.providers.Find(p => p.className == newClassName);
            if (existingProvider != null)
            {
                EditorUtility.DisplayDialog("Error", $"Class {newClassName} đã được đăng ký trước đó!", "OK");
                return;
            }
            
            registry.AddProvider(newModuleName, newClassName, newDescription, newPriority);
            
            // Reset form
            newModuleName = "";
            newClassName = "";
            newDescription = "";
            newPriority = 0;
            
            // Mark as dirty
            EditorUtility.SetDirty(registry);
            
            Debug.Log($"Đã thêm provider: {newClassName} từ module {newModuleName}");
        }
        
        private void RemoveProvider(int index)
        {
            if (index >= 0 && index < registry.providers.Count)
            {
                var providerName = registry.providers[index].className;
                registry.providers.RemoveAt(index);
                EditorUtility.SetDirty(registry);
                Debug.Log($"Đã xóa provider: {providerName}");
            }
        }
        
        private void ClearAllProviders()
        {
            registry.providers.Clear();
            EditorUtility.SetDirty(registry);
            Debug.Log("Đã xóa tất cả providers");
        }
        
        private void ValidateClassNames()
        {
            var invalidClasses = registry.ValidateClassNames();
            
            if (invalidClasses.Count == 0)
            {
                EditorUtility.DisplayDialog("Validation Result", "Tất cả class names đều hợp lệ!", "OK");
            }
            else
            {
                string message = $"Có {invalidClasses.Count} class name không hợp lệ:\n\n";
                foreach (var className in invalidClasses)
                {
                    message += $"• {className}\n";
                }
                
                EditorUtility.DisplayDialog("Validation Result", message, "OK");
            }
        }
        
        private void AutoDiscoverProviders()
        {
            // Tìm tất cả các class có thể là DataProvider
            var potentialProviders = FindPotentialDataProviders();
            
            if (potentialProviders.Count == 0)
            {
                EditorUtility.DisplayDialog("Auto-Discovery", "Không tìm thấy DataProvider nào!", "OK");
                return;
            }
            
            string message = $"Tìm thấy {potentialProviders.Count} class có thể là DataProvider:\n\n";
            foreach (var provider in potentialProviders)
            {
                message += $"• {provider}\n";
            }
            message += "\nBạn có muốn thêm tất cả vào registry không?";
            
            if (EditorUtility.DisplayDialog("Auto-Discovery", message, "Add All", "Cancel"))
            {
                foreach (var provider in potentialProviders)
                {
                    if (!registry.providers.Exists(p => p.className == provider))
                    {
                        registry.AddProvider("Auto-Discovered", provider, "Auto-discovered provider", 999);
                    }
                }
                EditorUtility.SetDirty(registry);
                Debug.Log($"Đã thêm {potentialProviders.Count} providers từ auto-discovery");
            }
        }
        
        private List<string> FindPotentialDataProviders()
        {
            var potentialProviders = new List<string>();
            
            // Quét tất cả assembly
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (IsPotentialDataProvider(type))
                        {
                            potentialProviders.Add(type.FullName);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Bỏ qua assembly có vấn đề
                    Debug.LogError($"Error finding potential data providers: {ex.Message}");
                    continue;
                }
            }
            
            return potentialProviders;
        }
        
        private bool IsPotentialDataProvider(System.Type type)
        {
            if (type == null || type.IsAbstract || type.IsInterface || type.IsGenericType)
                return false;
            
            // Kiểm tra xem có đủ các method cần thiết không
            var hasLoadData = type.GetMethod("LoadData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
            var hasGetData = type.GetMethod("GetData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
            var hasSaveData = type.GetMethod("SaveData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
            var hasHasData = type.GetMethod("HasData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
            var hasClearData = type.GetMethod("ClearData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
            
            return hasLoadData && hasGetData && hasSaveData && hasHasData && hasClearData;
        }
        
        private void SortProvidersByPriority()
        {
            registry.providers.Sort((a, b) => a.priority.CompareTo(b.priority));
            EditorUtility.SetDirty(registry);
            Debug.Log("Đã sắp xếp providers theo priority");
        }
    }
}
