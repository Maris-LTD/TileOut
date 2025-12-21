using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameModules.UI.Core;
using GameModules.UI.NavBar;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Sheets;
using System.IO;

namespace GameModules.UI.Editor
{
    public class UIAutoSetupWindow : EditorWindow
    {
        private Camera _selectedCamera;
        private bool _createEventSystem = true;
        private bool _createNavBarConfig = true;
        private int _screenSortingOrder = 0;
        private int _popupSortingOrder = 100;
        private int _sheetSortingOrder = 50;

        [MenuItem("Tools/Maris Module/UI/AutoSetup")]
        public static void ShowWindow()
        {
            var window = GetWindow<UIAutoSetupWindow>("UI Auto Setup");
            window.minSize = new Vector2(400, 350);
            window.Show();
        }

        private void OnEnable()
        {
            _selectedCamera = Camera.main;
        }

        private void OnGUI()
        {
            GUILayout.Label("UI Module Auto Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Công cụ này sẽ tự động tạo:\n" +
                "• ScreenContainer\n" +
                "• PopupContainer (ModalContainer)\n" +
                "• SheetContainer\n" +
                "• UIContainerManager\n" +
                "• UnityScreenNavigatorSettings (nếu chưa có)\n" +
                "• NavBarConfig (nếu chưa có)",
                MessageType.Info);

            EditorGUILayout.Space(10);

            GUILayout.Label("Cấu hình", EditorStyles.boldLabel);
            
            _selectedCamera = (Camera)EditorGUILayout.ObjectField(
                "Camera (cho Canvas)", 
                _selectedCamera, 
                typeof(Camera), 
                true);

            _createEventSystem = EditorGUILayout.Toggle("Tạo EventSystem", _createEventSystem);
            _createNavBarConfig = EditorGUILayout.Toggle("Tạo NavBarConfig", _createNavBarConfig);

            EditorGUILayout.Space(5);
            GUILayout.Label("Sorting Order", EditorStyles.boldLabel);
            _screenSortingOrder = EditorGUILayout.IntField("Screen Container", _screenSortingOrder);
            _sheetSortingOrder = EditorGUILayout.IntField("Sheet Container", _sheetSortingOrder);
            _popupSortingOrder = EditorGUILayout.IntField("Popup Container", _popupSortingOrder);

            EditorGUILayout.Space(20);

            if (_selectedCamera == null)
            {
                EditorGUILayout.HelpBox("Vui lòng chọn Camera cho Canvas!", MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(_selectedCamera == null);
            if (GUILayout.Button("Setup UI Containers", GUILayout.Height(40)))
            {
                SetupUIContainers();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void SetupUIContainers()
        {
            var existingManager = Object.FindFirstObjectByType<UIContainerManager>();
            if (existingManager != null)
            {
                if (!EditorUtility.DisplayDialog(
                    "UIContainerManager đã tồn tại",
                    "Scene đã có UIContainerManager. Bạn có muốn tạo lại không?",
                    "Tạo lại",
                    "Hủy"))
                {
                    return;
                }
                Undo.DestroyObjectImmediate(existingManager.gameObject);
            }

            var settings = GetOrCreateSettings();

            if (_createEventSystem)
            {
                CreateEventSystemIfNeeded();
            }

            if (_createNavBarConfig)
            {
                GetOrCreateNavBarConfig();
            }

            var screenContainer = CreateContainer<ScreenContainer>("ScreenContainer", _screenSortingOrder, settings);
            var popupContainer = CreateContainer<ModalContainer>("PopupContainer", _popupSortingOrder, settings);
            var sheetContainer = CreateContainer<SheetContainer>("SheetContainer", _sheetSortingOrder, settings);

            var managerGO = new GameObject("UIContainerManager");
            Undo.RegisterCreatedObjectUndo(managerGO, "Create UIContainerManager");

            var manager = managerGO.AddComponent<UIContainerManager>();
            
            var serializedManager = new SerializedObject(manager);
            serializedManager.FindProperty("screenContainer").objectReferenceValue = screenContainer;
            serializedManager.FindProperty("popupContainer").objectReferenceValue = popupContainer;
            serializedManager.FindProperty("sheetContainer").objectReferenceValue = sheetContainer;
            serializedManager.FindProperty("settings").objectReferenceValue = settings;
            serializedManager.ApplyModifiedProperties();

            Selection.activeGameObject = managerGO;

            string message = "UI Containers đã được tạo thành công!\n\n" +
                "• ScreenContainer\n" +
                "• PopupContainer\n" +
                "• SheetContainer\n" +
                "• UIContainerManager";

            if (_createNavBarConfig)
            {
                message += "\n• NavBarConfig";
            }

            EditorUtility.DisplayDialog("Setup hoàn tất", message, "OK");
        }

        private T CreateContainer<T>(string name, int sortingOrder, UnityScreenNavigatorSettings settings) where T : Component
        {
            var containerGO = new GameObject(name);
            containerGO.SetActive(false);
            Undo.RegisterCreatedObjectUndo(containerGO, $"Create {name}");

            var canvas = containerGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _selectedCamera;
            canvas.sortingOrder = sortingOrder;

            var scaler = containerGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            containerGO.AddComponent<GraphicRaycaster>();

            var container = containerGO.AddComponent<T>();

            var serializedContainer = new SerializedObject(container);
            var settingsProperty = serializedContainer.FindProperty("_settings");
            if (settingsProperty != null)
            {
                settingsProperty.objectReferenceValue = settings;
                serializedContainer.ApplyModifiedPropertiesWithoutUndo();
            }

            var rectTransform = containerGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            containerGO.SetActive(true);

            return container;
        }

        private void CreateEventSystemIfNeeded()
        {
            var existingEventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (existingEventSystem != null)
            {
                return;
            }

            var eventSystemGO = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");

            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }

        private UnityScreenNavigatorSettings GetOrCreateSettings()
        {
            const string settingsFolder = "Assets/Maris-Module/UI/Resources/Settings";
            const string settingsPath = settingsFolder + "/UnityScreenNavigatorSettings.asset";

            var settings = AssetDatabase.LoadAssetAtPath<UnityScreenNavigatorSettings>(settingsPath);
            if (settings != null)
            {
                return settings;
            }

            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
                AssetDatabase.Refresh();
            }

            settings = ScriptableObject.CreateInstance<UnityScreenNavigatorSettings>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return settings;
        }

        private NavBarConfig GetOrCreateNavBarConfig()
        {
            const string settingsFolder = "Assets/Maris-Module/UI/Resources/Settings";
            const string configPath = settingsFolder + "/NavBarConfig.asset";

            var config = AssetDatabase.LoadAssetAtPath<NavBarConfig>(configPath);
            if (config != null)
            {
                return config;
            }

            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
                AssetDatabase.Refresh();
            }

            config = ScriptableObject.CreateInstance<NavBarConfig>();
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return config;
        }
    }
}


