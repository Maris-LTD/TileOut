using UnityEngine;

namespace GameModules.DataManager.Examples
{
    /// <summary>
    /// Ví dụ DataProvider đơn giản để demo DataManager
    /// </summary>
    public class ExampleDataProvider
    {
        private ExampleData _data;
        
        public bool LoadData(object data)
        {
            try
            {
                if (data is string jsonData)
                {
                    _data = JsonUtility.FromJson<ExampleData>(jsonData);
                    Debug.Log($"[ExampleDataProvider] Đã load data: {_data.name}, Level: {_data.level}");
                    return true;
                }
                else if (data is ExampleData directData)
                {
                    _data = directData;
                    Debug.Log($"[ExampleDataProvider] Đã load data trực tiếp: {_data.name}, Level: {_data.level}");
                    return true;
                }
                
                Debug.LogWarning("[ExampleDataProvider] Không thể load data, format không hợp lệ");
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ExampleDataProvider] Lỗi khi load data: {ex.Message}");
                return false;
            }
        }
        
        public object GetData()
        {
            return _data;
        }
        
        public object SaveData()
        {
            if (_data == null)
            {
                Debug.LogWarning("[ExampleDataProvider] Không có data để lưu");
                return null;
            }
            
            string jsonData = JsonUtility.ToJson(_data);
            Debug.Log($"[ExampleDataProvider] Đã serialize data: {jsonData}");
            return jsonData;
        }
        
        public bool HasData()
        {
            return _data != null;
        }
        
        public void ClearData()
        {
            if (_data != null)
            {
                Debug.Log($"[ExampleDataProvider] Đã xóa data: {_data.name}");
                _data = null;
            }
        }
        
        /// <summary>
        /// Method bổ sung để demo - không bắt buộc
        /// </summary>
        public void UpdateData(string name, int level)
        {
            if (_data == null)
            {
                _data = new ExampleData();
            }
            
            _data.name = name;
            _data.level = level;
            Debug.Log($"[ExampleDataProvider] Đã cập nhật data: {_data.name}, Level: {_data.level}");
        }
    }
    
    /// <summary>
    /// Data structure cho ví dụ
    /// </summary>
    [System.Serializable]
    public class ExampleData
    {
        public string name = "Player";
        public int level = 1;
        public float health = 100f;
        public Vector3 position = Vector3.zero;
        
        public override string ToString()
        {
            return $"ExampleData: {name} (Level {level}) - Health: {health}, Position: {position}";
        }
    }
}
