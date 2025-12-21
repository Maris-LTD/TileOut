using UnityEngine;
using UnityEngine.Purchasing;

namespace IAPModule.Utilities
{
    public static class IAPLogger
    {
        private static readonly bool _enableLogging = false;
        
        public static void LogInfo(string message)
        {
            if (_enableLogging)
            {
                Debug.Log($"[IAP Module] {message}");
            }
        }
        
        public static void LogWarning(string message)
        {
            if (_enableLogging)
            {
                Debug.LogWarning($"[IAP Module] {message}");
            }
        }
        
        public static void LogError(string message)
        {
            if (_enableLogging)
            {
                Debug.LogError($"[IAP Module] {message}");
            }
        }
        
        public static void LogPurchaseSuccess(Product product)
        {
            LogInfo($"Purchase Success: {product.definition.id} - {product.metadata.localizedTitle}");
        }
        
        public static void LogPurchaseFailed(string productId, PurchaseFailureReason reason)
        {
            LogError($"Purchase Failed: {productId} - {reason}");
        }
        
        public static void LogInitializationComplete(bool success)
        {
            LogInfo($"Initialization Complete: {(success ? "Success" : "Failed")}");
        }
        
        public static void LogProductFetched(Product product)
        {
            LogInfo($"Product Fetched: {product.definition.id} - {product.metadata.localizedPriceString}");
        }
        
        public static void LogRestoreComplete(bool success, string error = null)
        {
            if (success)
            {
                LogInfo("Restore Purchases: Success");
            }
            else
            {
                LogError($"Restore Purchases Failed: {error}");
            }
        }
    }
}
