# Event System Documentation

## Tổng quan
Hệ thống Event Bus đơn giản với 3 hành động chính:
- **Subscribe**: Đăng ký nhận event
- **Unsubscribe**: Hủy đăng ký event
- **Publish**: Gửi event

## Tính năng
- Tự động đăng ký signal: Khi Subscribe lần đầu, signal sẽ tự động được đăng ký vào MessagePipe
- Không cần đăng ký signal thủ công
- Đơn giản và dễ sử dụng

## Cách sử dụng

### 1. Setup VContainer

EventSystemInstaller sẽ tự động được đăng ký vào Scope Root (nhờ `[AutoInstall]` attribute).

Nếu cần đăng ký thủ công:

```csharp
public class ProjectLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // EventSystemInstaller đã tự động được đăng ký
        // Hoặc có thể đăng ký thủ công:
        builder.Register<EventSystemInstaller>(Lifetime.Singleton);
        
        // Các dependency khác...
    }
}
```

### 2. Sử dụng EventBus

#### Inject và Subscribe:
```csharp
public class MyComponent : MonoBehaviour
{
    [Inject] private IEventBus _eventBus;
    
    private void Start()
    {
        // Subscribe event - signal sẽ tự động được đăng ký
        _eventBus.Subscribe<StartGameEvent>(OnStartGame);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe khi destroy
        _eventBus.Unsubscribe<StartGameEvent>(OnStartGame);
    }
    
    private void OnStartGame(StartGameEvent startEvent)
    {
        Debug.Log("Game started!");
    }
}
```

#### Publish Event:
```csharp
public void PublishMyEvent()
{
    var myEvent = new StartGameEvent(true);
    _eventBus.Publish(myEvent);
}
```

### 3. Tạo Event Signal

Chỉ cần tạo struct hoặc class cho event:

```csharp
public struct StartGameEvent
{
    public bool IsNewGame;
    
    public StartGameEvent(bool isNewGame)
    {
        IsNewGame = isNewGame;
    }
}
```

**Lưu ý**: Không cần đăng ký signal thủ công. Khi Subscribe hoặc Publish lần đầu, signal sẽ tự động được đăng ký vào MessagePipe.

## Best Practices

### 1. Memory Management:
- Luôn unsubscribe events trong OnDestroy
- Sử dụng ClearAll() khi cần xóa tất cả subscriptions

### 2. Performance:
- Events được cache để tránh resolve lại từ container
- Sử dụng struct cho event data khi có thể
- Tránh publish events quá thường xuyên

### 3. Event Naming:
- Đặt tên event rõ ràng với suffix "Event" (ví dụ: StartGameEvent, PlayerDiedEvent)

## Troubleshooting

### Common Issues:

1. **Event không được nhận**:
   - Kiểm tra đã Subscribe chưa
   - Kiểm tra đã Publish đúng event type chưa
   - Kiểm tra EventSystemInstaller đã được đăng ký chưa

2. **Event được nhận nhiều lần**:
   - Kiểm tra duplicate subscription
   - Đảm bảo unsubscribe đúng cách

3. **Performance issues**:
   - Kiểm tra số lượng events được publish
   - Đảm bảo unsubscribe khi không cần thiết

### Debug Logs:
Hệ thống có logging chi tiết để debug:
- Subscription errors
- Publish failures
- Container resolution errors
