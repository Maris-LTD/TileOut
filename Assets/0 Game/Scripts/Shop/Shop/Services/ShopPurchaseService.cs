using System;
using IAPModule.Adapters.Shop;
using IAPModule.Adapters.Shop.Data;

namespace IAPModule.Adapters.Shop.Services
{
    public class ShopPurchaseService : IShopPurchaseService
    {
        private readonly ShopDataProvider _dataProvider;

        public ShopPurchaseService(ShopDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public void RecordPurchase(string productId)
        {
            var history = _dataProvider.GetPurchaseHistory();
            history.AddPurchase(productId);
            _dataProvider.SavePurchaseHistory();
        }

        public int GetPurchaseCount(string productId)
        {
            var history = _dataProvider.GetPurchaseHistory();
            return history.GetPurchaseCount(productId);
        }

        public DateTime GetLastPurchaseTime(string productId)
        {
            var history = _dataProvider.GetPurchaseHistory();
            return history.GetLastPurchaseTime(productId);
        }

        public bool CanPurchase(ShopPackData packData)
        {
            if (packData == null) return false;
            if (packData.MaxPurchaseCount <= 0) return true;

            var history = _dataProvider.GetPurchaseHistory();
            var purchaseCount = history.GetPurchaseCount(packData.ProductId);
            
            if (purchaseCount >= packData.MaxPurchaseCount) return false;

            if (packData.Resetable && packData.ResetHours > 0)
            {
                if (history.ShouldReset(packData.ProductId, packData.ResetHours))
                {
                    history.ResetPurchase(packData.ProductId);
                    _dataProvider.SavePurchaseHistory();
                    return true;
                }
            }

            return purchaseCount < packData.MaxPurchaseCount;
        }

        public bool IsResetable(string productId, int resetHours)
        {
            var history = _dataProvider.GetPurchaseHistory();
            return history.ShouldReset(productId, resetHours);
        }
    }
}
