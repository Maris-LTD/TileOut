using System;
using System.Collections.Generic;
using IAPModule.Data;
using UnityEngine.Purchasing;

namespace IAPModule.Interfaces
{
    public interface IGameIAPAdapter
    {
        void OnIAPInitialized(bool success);
        void OnPurchaseCompleted(string productId, bool success);
        void OnPurchaseFailed(string errorMessage);
        void OnRestoreCompleted(bool success, string error = null);
        
    }

    
    public interface IIAPManager
    {
        event Action<bool> onInitializationComplete;
        event Action<string, bool> onPurchaseComplete;
        event Action<string> onPurchaseFailed;
        event Action<bool, string> onRestoreComplete;
        
        bool IsInitialized { get; }
        void Initialize();
        void Configure(string environment, bool enableReceiptValidation, List<IAPProductData> productDataList);
        void PurchaseProduct(string productId);
        void RestorePurchases();
        Product GetProduct(string productId);
        IAPProductData GetProductData(string productId);
        void SimulatePurchaseInDevMode(string productId);
        void Dispose();
    }

    public interface IIAPProductData
    {
        string ProductId { get; }
        string DisplayName { get; }
        string Description { get; }
        ProductType ProductType { get; }
        IBaseRewardPayload RewardPayload { get; }
    }

    public interface IBaseRewardPayload
    {
        void OnPurchaseSuccess(string productId);
        void OnPurchaseFailed(string productId, string reason);
        string GetRewardDescription();
    }
}
