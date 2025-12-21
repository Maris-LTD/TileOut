using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using IAPModule.Interfaces;
using IAPModule.Data;
using IAPModule.Utilities;

namespace IAPModule.Core
{
    public class IAPManager : IIAPManager
    {
        private string _environment = "production";
        private bool _enableReceiptValidation = true;
        private List<IAPProductData> _productDataList = new List<IAPProductData>();
        
        public event Action<bool> onInitializationComplete;
        public event Action<string, bool> onPurchaseComplete;
        public event Action<string> onPurchaseFailed;
        public event Action<bool, string> onRestoreComplete;
        
        public bool IsInitialized { get; private set; }
        
        private IStoreService _storeService;
        private IProductService _productService;
        private IPurchaseService _purchasingService;
        private ICatalogProvider _catalogProvider;
        
        private Dictionary<string, IAPProductData> _productDataMap;
        private Dictionary<string, Product> _fetchedProducts;

        public IAPManager()
        {
            InitializeProductDataMap();
            CreateServices();
        }

        public IAPManager(string environment, bool enableReceiptValidation, List<IAPProductData> productDataList)
        {
            this._environment = environment;
            this._enableReceiptValidation = enableReceiptValidation;
            this._productDataList = productDataList ?? new List<IAPProductData>();
            
            InitializeProductDataMap();
            CreateServices();
        }

        public void Configure(string environment, bool enableReceiptValidation, List<IAPProductData> productDataList)
        {
            this._environment = environment;
            this._enableReceiptValidation = enableReceiptValidation;
            this._productDataList = productDataList ?? new List<IAPProductData>();
            
            InitializeProductDataMap();
        }

        private void InitializeProductDataMap()
        {
            _productDataMap = new Dictionary<string, IAPProductData>();
            _fetchedProducts = new Dictionary<string, Product>();
            
            foreach (var productData in _productDataList)
            {
                if (productData != null)
                {
                    _productDataMap[productData.ProductId] = productData;
                }
            }
        }

        private void CreateServices()
        {
            _storeService = UnityIAPServices.DefaultStore();
            _productService = UnityIAPServices.DefaultProduct();
            _purchasingService = UnityIAPServices.DefaultPurchase();
            _catalogProvider = new CatalogProvider();
            
            ConfigureServiceCallbacks();
        }

        private void ConfigureServiceCallbacks()
        {
            _productService.OnProductsFetched += OnProductsFetched;
            _productService.OnProductsFetchFailed += OnProductsFetchFailed;
            
            _purchasingService.OnPurchasesFetched += OnPurchasesFetched;
            _purchasingService.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            _purchasingService.OnPurchasePending += OnPurchasePending;
            _purchasingService.OnPurchaseConfirmed += OnPurchaseConfirmed;
            _purchasingService.OnPurchaseFailed += OnPurchaseFailed;
            _purchasingService.OnPurchaseDeferred += OnPurchaseDeferred;
        }

        public async void Initialize()
        {
            try
            {
                IAPLogger.LogInfo("Initializing IAP Manager...");
                
                await UnityServices.InitializeAsync();
                
                CreateCrossPlatformValidator();
                await ConnectToStore();
                
                IsInitialized = true;
                onInitializationComplete?.Invoke(true);
                IAPLogger.LogInitializationComplete(true);
            }
            catch (Exception exception)
            {
                IsInitialized = false;
                onInitializationComplete?.Invoke(false);
                IAPLogger.LogError($"Initialization failed: {exception.Message}");
            }
        }

        private void CreateCrossPlatformValidator()
        {
            if (!_enableReceiptValidation) return;
            
#if !UNITY_EDITOR
            try
            {
                if (CanCrossPlatformValidate())
                {
                    // CrossPlatformValidator may not be available in all Unity IAP versions
                    // m_CrossPlatformValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), Application.identifier);
                    IAPLogger.LogInfo("Cross-platform validation is disabled - not available in current Unity IAP version");
                }
            }
            catch (NotImplementedException exception)
            {
                IAPLogger.LogWarning($"Cross Platform Validator Not Implemented: {exception}");
            }
#endif
        }

        private bool CanCrossPlatformValidate()
        {
            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer ||
                   Application.platform == RuntimePlatform.OSXPlayer ||
                   Application.platform == RuntimePlatform.tvOS;
        }

        private async System.Threading.Tasks.Task ConnectToStore()
        {
            await _storeService.Connect();
            IAPLogger.LogInfo("Store Connected");
            FetchProducts();
        }

        private void FetchProducts()
        {
            var productDefinitions = new List<ProductDefinition>();
            var storeSpecificIds = new Dictionary<string, StoreSpecificIds>();
            
            foreach (var productData in _productDataList)
            {
                if (productData == null) continue;
                
                var productDef = new ProductDefinition(
                    productData.ProductId,
                    productData.ProductType
                );
                productDefinitions.Add(productDef);
                
                var storeIds = new StoreSpecificIds
                {
                    {productData.GooglePlayId, GooglePlay.Name},
                    {productData.AppleStoreId, AppleAppStore.Name}
                };
                storeSpecificIds[productData.ProductId] = storeIds;
            }
            
            _catalogProvider.AddProducts(productDefinitions, storeSpecificIds);
            _catalogProvider.FetchProducts(_productService.FetchProductsWithNoRetries, DefaultStoreHelper.GetDefaultStoreName());
        }

        public void PurchaseProduct(string productId)
        {
            if (!IsInitialized)
            {
                IAPLogger.LogError("IAP Manager not initialized");
                onPurchaseFailed?.Invoke("IAP Manager not initialized");
                return;
            }
            
            var product = GetProduct(productId);
            if (product == null)
            {
                IAPLogger.LogError($"Product not found: {productId}");
                onPurchaseFailed?.Invoke($"Product not found: {productId}");
                return;
            }
            
            IAPLogger.LogInfo($"Initiating purchase for: {productId}");
            _purchasingService.PurchaseProduct(product);
        }

        public void RestorePurchases()
        {
            if (!IsInitialized)
            {
                IAPLogger.LogError("IAP Manager not initialized");
                return;
            }
            
            IAPLogger.LogInfo("Restoring purchases...");
            _purchasingService.RestoreTransactions(OnRestoreComplete);
        }

        public Product GetProduct(string productId)
        {
            return _fetchedProducts.GetValueOrDefault(productId);
        }

        public IAPProductData GetProductData(string productId)
        {
            return _productDataMap.GetValueOrDefault(productId);
        }

        public void SimulatePurchaseInDevMode(string productId)
        {
            var productData = GetProductData(productId);
            if (productData == null)
            {
                IAPLogger.LogError($"[Dev Mode] Product data not found: {productId}");
                onPurchaseFailed?.Invoke($"{productId}: Product data not found");
                return;
            }

            IAPLogger.LogInfo($"[Dev Mode] Simulating purchase for: {productId}");
            
            if (productData.RewardPayload != null)
            {
                productData.RewardPayload.OnPurchaseSuccess(productId);
            }
            
            onPurchaseComplete?.Invoke(productId, true);
        }

        private void OnRestoreComplete(bool success, string error)
        {
            IAPLogger.LogRestoreComplete(success, error);
            onRestoreComplete?.Invoke(success, error);
        }

        private void OnProductsFetched(List<Product> products)
        {
            IAPLogger.LogInfo($"Products fetched: {products.Count}");
            
            foreach (var product in products)
            {
                _fetchedProducts[product.definition.id] = product;
                IAPLogger.LogProductFetched(product);
            }
            
            _purchasingService.FetchPurchases();
        }

        private void OnProductsFetchFailed(ProductFetchFailed failure)
        {
            IAPLogger.LogError($"Products fetch failed: {failure.FailureReason}");
        }

        private void OnPurchasesFetched(Orders existingOrders)
        {
            IAPLogger.LogInfo("Existing purchases fetched");
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            IAPLogger.LogError($"Purchases fetch failed: {failure.Message}");
        }

        private void OnPurchasePending(PendingOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;
                var productData = GetProductData(product.definition.id);
                
                IAPLogger.LogPurchaseSuccess(product);
                
                if (productData?.RewardPayload != null)
                {
                    productData.RewardPayload.OnPurchaseSuccess(product.definition.id);
                }
                
                ValidatePurchaseIfPossible(order.Info);
            }
            
            ConfirmOrderIfAutomatic(order);
        }

        private void OnPurchaseConfirmed(Order order)
        {
            switch (order)
            {
                case ConfirmedOrder confirmedOrder:
                    OnPurchaseConfirmedInternal(confirmedOrder);
                    break;
                case FailedOrder failedOrder:
                    OnConfirmationFailed(failedOrder);
                    break;
            }
        }

        private void OnPurchaseConfirmedInternal(ConfirmedOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var product = cartItem.Product;
                var productData = GetProductData(product.definition.id);
                
                IAPLogger.LogInfo($"Purchase confirmed: {product.definition.id}");
                
                if (productData?.RewardPayload != null)
                {
                    productData.RewardPayload.OnPurchaseSuccess(product.definition.id);
                }
                
                onPurchaseComplete?.Invoke(product.definition.id, true);
            }
        }

        private void OnConfirmationFailed(FailedOrder failedOrder)
        {
            foreach (var cartItem in failedOrder.CartOrdered.Items())
            {
                var product = cartItem.Product;
                var productData = GetProductData(product.definition.id);
                
                IAPLogger.LogError($"Purchase confirmation failed: {product.definition.id}");
                
                if (productData?.RewardPayload != null)
                {
                    productData.RewardPayload.OnPurchaseFailed(product.definition.id, failedOrder.FailureReason.ToString());
                }
                
                onPurchaseComplete?.Invoke(product.definition.id, false);
            }
        }

        private void OnPurchaseFailed(FailedOrder failedOrder)
        {
            foreach (var cartItem in failedOrder.CartOrdered.Items())
            {
                var product = cartItem.Product;
                var productData = GetProductData(product.definition.id);
                
                IAPLogger.LogPurchaseFailed(product.definition.id, failedOrder.FailureReason);
                
                if (productData?.RewardPayload != null)
                {
                    productData.RewardPayload.OnPurchaseFailed(product.definition.id, failedOrder.FailureReason.ToString());
                }
                
                onPurchaseFailed?.Invoke($"{product.definition.id}: {failedOrder.FailureReason}");
            }
        }

        private void OnPurchaseDeferred(DeferredOrder deferredOrder)
        {
            foreach (var cartItem in deferredOrder.CartOrdered.Items())
            {
                var product = cartItem.Product;
                IAPLogger.LogInfo($"Purchase deferred: {product.definition.id}");
            }
        }

        private void ConfirmOrderIfAutomatic(PendingOrder order)
        {
            foreach (var cartItem in order.CartOrdered.Items())
            {
                var productData = GetProductData(cartItem.Product.definition.id);
                if (productData?.ProductType == ProductType.Consumable)
                {
                    ConfirmOrder(order);
                    break;
                }
            }
        }

        private void ConfirmOrder(PendingOrder pendingOrder)
        {
            _purchasingService.ConfirmPurchase(pendingOrder);
        }

        private void ValidatePurchaseIfPossible(IOrderInfo orderInfo)
        {
            // Receipt validation is disabled in current implementation
            // CrossPlatformValidator is not available in current Unity IAP version
            if (!string.IsNullOrEmpty(orderInfo.Receipt))
            {
                IAPLogger.LogInfo("Receipt validation skipped - not available in current Unity IAP version");
            }
        }

        public void Dispose()
        {
            if (_productService != null)
            {
                _productService.OnProductsFetched -= OnProductsFetched;
                _productService.OnProductsFetchFailed -= OnProductsFetchFailed;
            }
            
            if (_purchasingService != null)
            {
                _purchasingService.OnPurchasesFetched -= OnPurchasesFetched;
                _purchasingService.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
                _purchasingService.OnPurchasePending -= OnPurchasePending;
                _purchasingService.OnPurchaseConfirmed -= OnPurchaseConfirmed;
                _purchasingService.OnPurchaseFailed -= OnPurchaseFailed;
                _purchasingService.OnPurchaseDeferred -= OnPurchaseDeferred;
            }
        }
    }
}
