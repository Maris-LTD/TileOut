# Game Modules - Data Manager

Module quản lý data trung tâm cho các game module, sử dụng reflection để tự động phát hiện và quản lý DataProvider.

## Tính năng chính

- **Quản lý trung tâm**: Quản lý tất cả DataProvider từ các module khác nhau
- **Reflection-based**: Tự động phát hiện DataProvider mà không cần sửa đổi code gốc
- **ScriptableObject Registry**: Quản lý danh sách provider qua ScriptableObject
- **Priority System**: Hỗ trợ thứ tự ưu tiên khi load/save data
- **Event System**: Cung cấp events để theo dõi trạng thái data
- **VContainer Integration**: Tích hợp sẵn với VContainer DI framework

## Cài đặt

1. Copy thư mục `GameModules/DataManager` vào project Unity của bạn
2. Đảm bảo đã cài đặt VContainer package
3. Tạo DataProviderRegistry asset trong Unity Editor

## Cách sử dụng

### 1. Tạo DataProviderRegistry

Trong Unity Editor:
- Right-click trong Project window
- Create → GameModules → Data Provider Registry
- Đặt tên cho asset (ví dụ: "DataProviderRegistry")

### 2. Tạo DataManager GameObject

- Tạo GameObject mới trong scene
- Thêm component `DataManager`
- Gán `DataProviderRegistry` asset vào field Registry

### 3. Đăng ký DataProvider

Trong DataProviderRegistry asset:
- Click "Show Add Provider Form"
- Điền thông tin:
  - **Module Name**: Tên module chứa provider
  - **Class Name**: Tên đầy đủ của class (bao gồm namespace)
  - **Description**: Mô tả chức năng
  - **Priority**: Thứ tự ưu tiên (số càng nhỏ càng ưu tiên)

### 4. Tạo DataProvider Class

Tạo class có các phương thức chuẩn:

```csharp
public class PlayerStatsProvider
{
    private PlayerStats _stats;
    
    public bool LoadData(object data)
    {
        try
        {
            if (data is string jsonData)
            {
                _stats = JsonUtility.FromJson<PlayerStats>(jsonData);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public object GetData()
    {
        return _stats;
    }
    
    public object SaveData()
    {
        return JsonUtility.ToJson(_stats);
    }
    
    public bool HasData()
    {
        return _stats != null;
    }
    
    public void ClearData()
    {
        _stats = null;
    }
}
```

### 5. Sử dụng DataManager

```csharp
public class GameController : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;
    
    private void Start()
    {
        // Load data vào tất cả provider
        string saveData = LoadSaveFile();
        dataManager.LoadAllData(saveData);
        
        // Lấy data từ provider cụ thể
        var playerStats = dataManager.GetData("PlayerStatsProvider") as PlayerStats;
        
        // Lưu data từ tất cả provider
        var allData = dataManager.SaveAllData();
        SaveToFile(allData);
    }
}
```

## API Reference

### DataManager Methods

- `RegisterProvider(string className)`: Đăng ký provider
- `UnregisterProvider(string className)`: Hủy đăng ký provider
- `LoadData(string className, object data)`: Load data vào provider cụ thể
- `LoadAllData(object data)`: Load data vào tất cả provider
- `GetData(string className)`: Lấy data từ provider
- `SaveData(string className)`: Lưu data từ provider
- `SaveAllData()`: Lưu data từ tất cả provider
- `HasData(string className)`: Kiểm tra provider có data
- `ClearData(string className)`: Xóa data của provider
- `ClearAllData()`: Xóa data của tất cả provider

### Events

- `OnProviderRegistered`: Khi provider được đăng ký
- `OnProviderUnregistered`: Khi provider bị hủy đăng ký
- `OnDataLoaded`: Khi data được load
- `OnDataSaved`: Khi data được lưu

## Auto-Discovery

DataManager có thể tự động phát hiện các class có thể là DataProvider:

1. Trong DataProviderRegistry Editor
2. Click "Auto-Discover Providers"
3. Chọn "Add All" để thêm tất cả provider được phát hiện

## Validation

- **Validate Class Names**: Kiểm tra tên class có tồn tại không
- **Runtime Validation**: Tự động validate khi khởi tạo

## Lưu ý quan trọng

1. **Tên phương thức**: DataProvider phải có đúng tên các phương thức:
   - `LoadData(object data)`
   - `GetData()`
   - `SaveData()`
   - `HasData()`
   - `ClearData()`

2. **Namespace**: Khi đăng ký provider, sử dụng tên đầy đủ bao gồm namespace

3. **Exception Handling**: DataManager có xử lý exception, nhưng provider nên handle lỗi một cách an toàn

4. **Performance**: Reflection có thể ảnh hưởng performance, nhưng chỉ được sử dụng khi khởi tạo

## Ví dụ tích hợp

### Inventory Module
```csharp
// InventoryDataProvider.cs
public class InventoryDataProvider
{
    private Inventory _inventory;
    
    public bool LoadData(object data) { /* implementation */ }
    public object GetData() { return _inventory; }
    public object SaveData() { /* implementation */ }
    public bool HasData() { return _inventory != null; }
    public void ClearData() { _inventory = null; }
}
```

### Player Module
```csharp
// PlayerStatsProvider.cs
public class PlayerStatsProvider
{
    private PlayerStats _stats;
    
    public bool LoadData(object data) { /* implementation */ }
    public object GetData() { return _stats; }
    public object SaveData() { /* implementation */ }
    public bool HasData() { return _stats != null; }
    public void ClearData() { _stats = null; }
}
```
## License

MIT License - Xem file LICENSE để biết thêm chi tiết.
