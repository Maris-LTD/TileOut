# IAP Module Documentation

## Tổng quan
IAP Module là một hệ thống In-App Purchasing độc lập và linh hoạt cho Unity, được thiết kế để dễ dàng tích hợp vào các game hiện có thông qua Adapter Pattern.

## Cấu trúc Module

### 1. Core Components
- **IAPManager**: Class chính quản lý toàn bộ hệ thống IAP
- **IAPProductData**: ScriptableObject chứa thông tin sản phẩm
- **BaseRewardPayload**: ScriptableObject base cho reward system

### 2. Interfaces
- **IIAPManager**: Interface cho IAP Manager
- **IIAPProductData**: Interface cho Product Data
- **IBaseRewardPayload**: Interface cho Reward Payload

### 3. Adapters
- **IAPGameAdapter**: Adapter để tích hợp vào game
- **IGameIAPAdapter**: Interface cho game integration

### 4. Utilities
- **IAPLogger**: Hệ thống logging

## Cách sử dụng

### Bước 1: Setup Assembly Definition
Module đã có Assembly Definition riêng (`IAPModule.asmdef`) với các dependencies:
- Unity.Services.Core
- Unity.Purchasing

### Bước 2: Tạo Product Data
1. Tạo ScriptableObject từ menu: `Create > IAP Module > Product Data`
2. Điền thông tin:
   - Product ID
   - Display Name
   - Description
   - Product Type (Consumable/Non-Consumable/Subscription)
   - Store Specific IDs (Google Play, Apple Store)
   - Reward Payload

### Bước 3: Tạo Reward Payload
Tạo các reward payload cụ thể:
- `CurrencyRewardPayload`: Thêm currency vào game
- `ItemRewardPayload`: Thêm items vào inventory
- `FeatureRewardPayload`: Enable/disable features

### Bước 4: Setup IAP Manager
1. Tạo GameObject với IAPGameAdapter component
2. Assign các Product Data vào Product Data List trong IAPGameAdapter
3. Configure environment và validation settings trong IAPGameAdapter
4. IAPGameAdapter sẽ tự động tạo IAPManager instance

### Bước 5: Tích hợp vào Game
1. Implement IGameIAPAdapter interface trong game logic
2. Subscribe vào các events từ IAPGameAdapter
3. Sử dụng các phương thức purchase và restore

## Example Usage

```csharp
public class MyGameIAP : MonoBehaviour, IGameIAPAdapter
{
    [SerializeField] private IAPGameAdapter iapAdapter;
    
    void Start()
    {
        iapAdapter.OnInitializationComplete += OnIAPInitialized;
        iapAdapter.OnPurchaseComplete += OnPurchaseCompleted;
    }
    
    public void BuyCoins()
    {
        iapAdapter.PurchaseProduct("com.mygame.coins.100");
    }
    
    public void OnIAPInitialized(bool success)
    {
        Debug.Log($"IAP Initialized: {success}");
    }
    
    public void OnPurchaseCompleted(string productId, bool success)
    {
        if (success)
        {
            Debug.Log($"Purchase successful: {productId}");
        }
    }
}
```

## Tính năng chính

### 1. Plain C# Object Design
IAPManager là plain C# class, không phụ thuộc vào MonoBehaviour, dễ test và maintain.

### 2. Không sử dụng Singleton
Module được thiết kế để có thể có nhiều instance, phù hợp cho các game phức tạp.

### 3. Adapter Pattern
Dễ dàng tích hợp vào game hiện có mà không cần thay đổi code game.

### 4. Flexible Reward System
Hệ thống reward có thể mở rộng dễ dàng thông qua inheritance.

### 5. Cross-Platform Support
Hỗ trợ cả Google Play Store và Apple App Store.

### 6. Receipt Validation
Receipt validation is currently disabled due to Unity IAP version compatibility. Can be enabled when CrossPlatformValidator becomes available.

### 7. Dispose Pattern
Proper resource management với Dispose pattern.

## Configuration

### Environment Settings
- `production`: Cho build release
- `development`: Cho testing

### Receipt Validation
- Currently disabled due to Unity IAP version compatibility
- Can be enabled when CrossPlatformValidator becomes available
- Basic receipt logging is still available

### Logging
- Enable/disable debug logs
- Detailed logging cho debugging

## Best Practices

1. **Luôn check IsInitialized** trước khi gọi purchase methods
2. **Handle tất cả events** để có UX tốt nhất
3. **Implement restore purchases** cho iOS
4. **Test trên device thật** với sandbox accounts
5. **Validate receipts** trong production

## Troubleshooting

### Common Issues
1. **Initialization failed**: Check Unity Services configuration
2. **Products not found**: Verify Product IDs match store configuration
3. **Purchase failed**: Check store configuration và network connection
4. **Receipt validation failed**: Check Google Play/Apple Store keys

### Debug Tips
- Enable IAPLogger để xem detailed logs
- Test với Fake Store trong Editor
- Use sandbox accounts cho testing
