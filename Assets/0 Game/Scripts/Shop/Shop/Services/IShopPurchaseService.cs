using System;
using IAPModule.Adapters.Shop;

namespace IAPModule.Adapters.Shop.Services
{
    public interface IShopPurchaseService
    {
        void RecordPurchase(string productId);
        int GetPurchaseCount(string productId);
        DateTime GetLastPurchaseTime(string productId);
        bool CanPurchase(ShopPackData packData);
        bool IsResetable(string productId, int resetHours);
    }
}
