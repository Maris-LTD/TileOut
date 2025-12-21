# Module UI Base - Hướng dẫn sử dụng

Module UI Base cung cấp một kiến trúc UI dựa trên ZBase.UnityScreenNavigator với 3 tầng: Navigation Service, Logic/Presenter, và View.

## I. Setup cơ bản

### 1. Tạo Containers trong Scene

Trong Unity Editor, bạn cần tạo 3 containers cho Screen, Popup (Modal), và Sheet:

1. **Tạo ScreenContainer:**
   - Tạo GameObject mới, đặt tên "ScreenContainer"
   - **Add component `Canvas`** (quan trọng!)
   - Add component `GraphicRaycaster` (khuyến nghị)
   - Add component `ScreenContainer` (từ ZBase.UnityScreenNavigator)
   - ScreenContainer sẽ tự động thêm `CanvasGroup` và `RectMask2D`
   - Cấu hình Canvas: Render Mode = Screen Space - Overlay (hoặc Camera)
   - Cấu hình trong Inspector

2. **Tạo PopupContainer (ModalContainer):**
   - Tạo GameObject mới, đặt tên "PopupContainer"
   - **Add component `Canvas`** (quan trọng!)
   - Add component `GraphicRaycaster` (khuyến nghị)
   - Add component `ModalContainer` (từ ZBase.UnityScreenNavigator)
   - ModalContainer sẽ tự động thêm `CanvasGroup` và `RectMask2D`
   - Cấu hình Canvas: Render Mode = Screen Space - Overlay (hoặc Camera)
   - Cấu hình trong Inspector

3. **Tạo SheetContainer:**
   - Tạo GameObject mới, đặt tên "SheetContainer"
   - **Add component `Canvas`** (quan trọng!)
   - Add component `GraphicRaycaster` (khuyến nghị)
   - Add component `SheetContainer` (từ ZBase.UnityScreenNavigator)
   - SheetContainer sẽ tự động thêm `CanvasGroup` và `RectMask2D`
   - Cấu hình Canvas: Render Mode = Screen Space - Overlay (hoặc Camera)
   - Cấu hình trong Inspector

### 2. Setup UIContainerManager

1. Tạo GameObject mới, đặt tên "UIContainerManager"
2. Add component `UIContainerManager`
3. Kéo thả 3 containers vào các field tương ứng:
   - ScreenContainer → Screen Container field
   - PopupContainer → Popup Container field  
   - SheetContainer → Sheet Container field

### 3. Đảm bảo Scene có SceneLifetimeScope

Module UI sẽ tự động được đăng ký thông qua `UIInstaller` với `[AutoInstall(ModuleScope.Scene)]`.

Đảm bảo scene của bạn có `SceneLifetimeScope` component (từ GameModules.ModuleManager).

## II. Tạo UI mới

### 1. Tạo Screen

**Bước 1: Tạo Prefab**
- Tạo Prefab UI trong Unity
- **KHÔNG cần Canvas** - Container đã có Canvas sẵn
- Prefab chỉ cần:
  - Root GameObject với RectTransform (và component Screen/Popup/Sheet)
  - Các UI elements bên trong (Button, Text, Image, etc.)
- Đặt trong thư mục Resources hoặc cấu hình ResourcePath

**Bước 2: Tạo Data class**
```csharp
public class MyScreenData : IScreenData
{
    public string Title { get; set; }
}
```

**Bước 3: Tạo Result class (nếu cần)**
```csharp
public class MyScreenResult
{
    public bool Success { get; set; }
}
```

**Bước 4: Tạo Screen class**
```csharp
public class MyScreen : BaseScreen<MyScreenData, MyScreenResult>
{
    [SerializeField] private Text titleText;
    [SerializeField] private Button closeButton;

    protected override void OnInitialize(MyScreenData data)
    {
        if (data != null && titleText != null)
        {
            titleText.text = data.Title;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnClose);
        }
    }

    private void OnClose()
    {
        SetResult(new MyScreenResult { Success = true });
    }

    protected override void OnCleanup()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
```

**Bước 5: Gắn Script vào Prefab**
- Add component `MyScreen` vào Prefab
- Kéo thả các UI elements vào các SerializeField

### 2. Tạo Popup (tương tự Screen)

```csharp
public class MyPopupData : IPopupData
{
    public string Message { get; set; }
}

public class MyPopupResult
{
    public bool Confirmed { get; set; }
}

public class MyPopup : BasePopup<MyPopupData, MyPopupResult>
{
    // Implementation tương tự Screen
}
```

### 3. Tạo Sheet (tương tự Screen)

```csharp
public class MySheetData : ISheetData
{
    public string Content { get; set; }
}

public class MySheetResult
{
    public string SelectedItem { get; set; }
}

public class MySheet : BaseSheet<MySheetData, MySheetResult>
{
    // Implementation tương tự Screen
    // Lưu ý: Sheet dùng WillEnter/DidEnter thay vì WillPushEnter/DidPushEnter
}
```

## III. Sử dụng Navigation Service

### 1. Inject IUINavigationService

```csharp
public class GameManager : MonoBehaviour
{
    private IUINavigationService _uiService;

    [Inject]
    public void Construct(IUINavigationService uiService)
    {
        _uiService = uiService;
    }
}
```

Hoặc qua constructor injection:

```csharp
public class GameManager
{
    private readonly IUINavigationService _uiService;

    public GameManager(IUINavigationService uiService)
    {
        _uiService = uiService;
    }
}
```

### 2. Mở Screen

```csharp
// Mở screen với data
var screenData = new MyScreenData { Title = "Main Menu" };
var result = await _uiService.PushScreenAsync<MyScreenData, MyScreenResult>(
    "Prefabs/Screens/MyScreen", 
    screenData
);

if (result != null && result.Success)
{
    Debug.Log("Screen closed successfully");
}
```

### 3. Mở Popup

```csharp
// Mở popup và chờ kết quả
var popupData = new MyPopupData { Message = "Are you sure?" };
var result = await _uiService.ShowPopupAsync<MyPopupData, MyPopupResult>(
    "Prefabs/Popups/MyPopup",
    popupData
);

if (result != null && result.Confirmed)
{
    Debug.Log("User confirmed");
}
else
{
    Debug.Log("User cancelled");
}
```

### 4. Mở Sheet

```csharp
// Mở sheet
var sheetData = new MySheetData { Content = "Sheet Content" };
var result = await _uiService.ShowSheetAsync<MySheetData, MySheetResult>(
    "Prefabs/Sheets/MySheet",
    sheetData
);
```

### 5. Đóng UI

```csharp
// Đóng screen hiện tại
await _uiService.PopScreenAsync();

// Đóng popup hiện tại
await _uiService.ClosePopupAsync();

// Đóng sheet hiện tại
await _uiService.CloseSheetAsync();
```

## IV. Dependency Injection trong UI

Các BaseScreen/BasePopup/BaseSheet đã được tự động inject dependencies qua `Resolver` property:

```csharp
public class MyScreen : BaseScreen<MyScreenData, MyScreenResult>
{
    private IPlayerDataService _playerService;

    protected override void OnDependenciesInjected()
    {
        _playerService = Resolver.Resolve<IPlayerDataService>();
    }

    protected override void OnInitialize(MyScreenData data)
    {
        // Sử dụng _playerService
        var playerName = _playerService.GetPlayerName();
    }
}
```

## V. Lifecycle Hooks

### Screen và Popup có các hooks:
- `OnInitialize(TData data)` - Khi được khởi tạo
- `OnWillPushEnter()` - Trước khi push vào
- `OnDidPushEnter()` - Sau khi push vào
- `OnWillPushExit()` - Trước khi push ra
- `OnDidPushExit()` - Sau khi push ra
- `OnWillPopEnter()` - Trước khi pop vào
- `OnDidPopEnter()` - Sau khi pop vào
- `OnWillPopExit()` - Trước khi pop ra
- `OnDidPopExit()` - Sau khi pop ra
- `OnCleanup()` - Khi cleanup

### Sheet có các hooks:
- `OnInitialize(TData data)` - Khi được khởi tạo
- `OnWillEnter()` - Trước khi enter
- `OnDidEnter()` - Sau khi enter
- `OnWillExit()` - Trước khi exit
- `OnDidExit()` - Sau khi exit
- `OnCleanup()` - Khi cleanup

## VI. Ví dụ hoàn chỉnh

Xem các file trong `Assets/Maris-Module/UI/Runtime/Scripts/Examples/`:
- `ExampleScreen.cs` - Ví dụ Screen
- `ExamplePopup.cs` - Ví dụ Popup

## Lưu ý

1. **Prefab Structure:**
   - **KHÔNG cần Canvas** trong Prefab - Container đã có Canvas sẵn
   - Root GameObject cần có RectTransform và component Screen/Popup/Sheet tương ứng
   - RectTransform sẽ tự động fill parent (Container) khi được load
   - Các UI elements (Button, Text, Image...) đặt bên trong root GameObject

2. **Resource Path:** Đảm bảo resourceKey trùng với đường dẫn prefab trong Resources hoặc cấu hình đúng trong ZBase settings

3. **Result:** Result chỉ có giá trị sau khi UI được đóng. Để lấy result ngay sau khi mở, cần implement cơ chế khác (ví dụ: event, callback)

4. **Sheet:** Sheet sử dụng `RegisterAsync` → `ShowAsync` → `HideAsync`, khác với Screen/Popup dùng `PushAsync` → `PopAsync`

5. **Container Setup:**
   - **Containers CẦN có Canvas component** - phải thêm thủ công khi tạo trong Editor
   - Containers sẽ tự động thêm `CanvasGroup` và `RectMask2D` khi add component
   - Containers sẽ tự động quản lý việc load và hiển thị Prefab
   - Prefab được load như child của Container (KHÔNG cần Canvas trong Prefab)

