using UnityEngine;

namespace GameModules.DataManager.Examples
{
    /// <summary>
    /// Demo script để test DataManager
    /// </summary>
    public class DataManagerDemo : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DataManager dataManager;
        
        [Header("Demo Settings")]
        [SerializeField] private bool autoTestOnStart = true;
        [SerializeField] private float testInterval = 2f;
        
        private float _lastTestTime;
        private int _testStep;
        
        private void Start()
        {
            if (dataManager == null)
            {
                dataManager = FindAnyObjectByType<DataManager>();
                if (dataManager == null)
                {
                    Debug.LogError("[DataManagerDemo] Không tìm thấy DataManager trong scene!");
                    return;
                }
            }
            
            // Subscribe to events
            dataManager.OnProviderRegistered += OnProviderRegistered;
            dataManager.OnProviderUnregistered += OnProviderUnregistered;
            dataManager.OnDataLoaded += OnDataLoaded;
            dataManager.OnDataSaved += OnDataSaved;
            
            if (autoTestOnStart)
            {
                Debug.Log("[DataManagerDemo] Bắt đầu demo tự động...");
                _lastTestTime = Time.time;
            }
        }
        
        private void Update()
        {
            if (autoTestOnStart && Time.time - _lastTestTime >= testInterval)
            {
                RunNextTest();
                _lastTestTime = Time.time;
            }
        }
        
        private void RunNextTest()
        {
            switch (_testStep)
            {
                case 0:
                    TestLoadData();
                    break;
                case 1:
                    TestGetData();
                    break;
                case 2:
                    TestSaveData();
                    break;
                case 3:
                    TestClearData();
                    break;
                case 4:
                    TestLoadAllData();
                    break;
                case 5:
                    TestSaveAllData();
                    break;
                default:
                    _testStep = 0; // Reset
                    Debug.Log("[DataManagerDemo] Demo hoàn thành, bắt đầu lại...");
                    return;
            }
            
            _testStep++;
        }
        
        private void TestLoadData()
        {
            Debug.Log("[DataManagerDemo] === Test LoadData ===");
            
            // Tạo data mẫu
            var exampleData = new ExampleData
            {
                name = "Hero",
                level = 5,
                health = 150f,
                position = new Vector3(10, 5, 0)
            };
            
            // Load data vào provider
            bool success = dataManager.LoadData("GameModules.DataManager.Examples.ExampleDataProvider", exampleData);
            Debug.Log($"[DataManagerDemo] LoadData result: {success}");
        }
        
        private void TestGetData()
        {
            Debug.Log("[DataManagerDemo] === Test GetData ===");
            
            var data = dataManager.GetData("GameModules.DataManager.Examples.ExampleDataProvider");
            if (data != null)
            {
                Debug.Log($"[DataManagerDemo] Retrieved data: {data}");
            }
            else
            {
                Debug.LogWarning("[DataManagerDemo] Không lấy được data");
            }
        }
        
        private void TestSaveData()
        {
            Debug.Log("[DataManagerDemo] === Test SaveData ===");
            
            var savedData = dataManager.SaveData("GameModules.DataManager.Examples.ExampleDataProvider");
            if (savedData != null)
            {
                Debug.Log($"[DataManagerDemo] Saved data: {savedData}");
            }
            else
            {
                Debug.LogWarning("[DataManagerDemo] Không lưu được data");
            }
        }
        
        private void TestClearData()
        {
            Debug.Log("[DataManagerDemo] === Test ClearData ===");
            
            dataManager.ClearData("GameModules.DataManager.Examples.ExampleDataProvider");
            Debug.Log("[DataManagerDemo] Đã clear data");
            
            // Kiểm tra xem data đã bị xóa chưa
            bool hasData = dataManager.HasData("GameModules.DataManager.Examples.ExampleDataProvider");
            Debug.Log($"[DataManagerDemo] HasData after clear: {hasData}");
        }
        
        private void TestLoadAllData()
        {
            Debug.Log("[DataManagerDemo] === Test LoadAllData ===");
            
            // Tạo data mẫu cho tất cả provider
            var allData = new ExampleData
            {
                name = "AllPlayers",
                level = 10,
                health = 200f,
                position = Vector3.zero
            };
            
            dataManager.LoadAllData(allData);
            Debug.Log("[DataManagerDemo] Đã load data vào tất cả provider");
        }
        
        private void TestSaveAllData()
        {
            Debug.Log("[DataManagerDemo] === Test SaveAllData ===");
            
            var allSavedData = dataManager.SaveAllData();
            Debug.Log($"[DataManagerDemo] Đã lưu data từ {allSavedData.Count} provider:");
            
            foreach (var kvp in allSavedData)
            {
                Debug.Log($"  - {kvp.Key}: {kvp.Value}");
            }
        }
        
        private void OnProviderRegistered(string className)
        {
            Debug.Log($"[DataManagerDemo] Provider đã đăng ký: {className}");
        }
        
        private void OnProviderUnregistered(string className)
        {
            Debug.Log($"[DataManagerDemo] Provider đã hủy đăng ký: {className}");
        }
        
        private void OnDataLoaded(string className)
        {
            Debug.Log($"[DataManagerDemo] Data đã được load: {className}");
        }
        
        private void OnDataSaved(string className)
        {
            Debug.Log($"[DataManagerDemo] Data đã được lưu: {className}");
        }
        
        private void OnDestroy()
        {
            if (dataManager != null)
            {
                dataManager.OnProviderRegistered -= OnProviderRegistered;
                dataManager.OnProviderUnregistered -= OnProviderUnregistered;
                dataManager.OnDataLoaded -= OnDataLoaded;
                dataManager.OnDataSaved -= OnDataSaved;
            }
        }
        
        // Manual test buttons
        [ContextMenu("Test LoadData")]
        private void TestLoadDataManual()
        {
            TestLoadData();
        }
        
        [ContextMenu("Test GetData")]
        private void TestGetDataManual()
        {
            TestGetData();
        }
        
        [ContextMenu("Test SaveData")]
        private void TestSaveDataManual()
        {
            TestSaveData();
        }
        
        [ContextMenu("Test ClearData")]
        private void TestClearDataManual()
        {
            TestClearData();
        }
        
        [ContextMenu("Test LoadAllData")]
        private void TestLoadAllDataManual()
        {
            TestLoadAllData();
        }
        
        [ContextMenu("Test SaveAllData")]
        private void TestSaveAllDataManual()
        {
            TestSaveAllData();
        }
    }
}
