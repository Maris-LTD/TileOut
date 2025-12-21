using GameModules.Core;
using GameModules.Systems.Events;
using IAPModule.Data;
using IAPModule.Events;
using IAPModule.Interfaces;
using UnityEngine;
using UnityEngine.Purchasing;
using VContainer;

namespace IAPModule.Adapters
{
    [AutoInject]
    public class IAPGameAdapter : MonoBehaviour, IGameIAPAdapter
    {
        [Inject] private IIAPManager _iapManager;
        [Inject] private IGlobalEventBus _eventBus;
        [Inject] private IAPConfig _iapConfig;
        
        public bool IsInitialized => _iapManager?.IsInitialized ?? false;
        public bool IsDevMode => _iapConfig?.IsDevMode ?? false;

        private void Start()
        {
            if (_iapManager == null)
            {
                Debug.LogError("[IAPGameAdapter] IAPManager is null. Make sure IAPInstaller is registered.");
                return;
            }

            SubscribeToIAPEvents();
            InitializeIAP();
        }

        private void OnDestroy()
        {
            UnsubscribeFromIAPEvents();
        }

        private void SubscribeToIAPEvents()
        {
            if (_iapManager != null)
            {
                _iapManager.onInitializationComplete += OnIAPInitialized;
                _iapManager.onPurchaseComplete += OnPurchaseCompleted;
                _iapManager.onPurchaseFailed += OnPurchaseFailed;
                _iapManager.onRestoreComplete += OnRestoreCompleted;
            }
        }

        private void UnsubscribeFromIAPEvents()
        {
            if (_iapManager != null)
            {
                _iapManager.onInitializationComplete -= OnIAPInitialized;
                _iapManager.onPurchaseComplete -= OnPurchaseCompleted;
                _iapManager.onPurchaseFailed -= OnPurchaseFailed;
                _iapManager.onRestoreComplete -= OnRestoreCompleted;
            }
        }

        private void InitializeIAP()
        {
            if (_iapManager is { IsInitialized: false })
            {
                _iapManager.Initialize();
            }
        }

        public Product GetProduct(string productId)
        {
            if (IsDevMode)
            {
                return null;
            }

            if (_iapManager is null)
            {
                Debug.LogError("[IAPGameAdapter] IAPManager is null.");
                return null;
            }

            if (!_iapManager.IsInitialized)
            {
                Debug.Log("[IAPGameAdapter] IAPManager.IsInitialized");
                return null;
            }
            
            return _iapManager.GetProduct(productId);
        }

        public void PurchaseProduct(string productId)
        {
            if (IsDevMode)
            {
                Debug.Log($"[IAPGameAdapter] Dev Mode: Simulating purchase for {productId}");
                if (_iapManager != null)
                {
                    _iapManager.SimulatePurchaseInDevMode(productId);
                }
                return;
            }

            if (_iapManager == null)
            {
                Debug.LogError("[IAPGameAdapter] IAPManager is null. Cannot purchase product.");
                return;
            }

            if (!_iapManager.IsInitialized)
            {
                Debug.LogWarning("[IAPGameAdapter] IAPManager is not initialized yet. Purchase request will be ignored.");
                return;
            }

            _iapManager.PurchaseProduct(productId);
        }

        public void RestorePurchases()
        {
            if (_iapManager == null)
            {
                Debug.LogError("[IAPGameAdapter] IAPManager is null. Cannot restore purchases.");
                return;
            }

            if (!_iapManager.IsInitialized)
            {
                Debug.LogWarning("[IAPGameAdapter] IAPManager is not initialized yet. Restore request will be ignored.");
                return;
            }

            _iapManager.RestorePurchases();
        }

        public void OnIAPInitialized(bool success)
        {
            if (_eventBus != null)
            {
                _eventBus.Publish(new IAPInitializedEvent(success));
            }
        }

        public void OnPurchaseCompleted(string productId, bool success)
        {
            if (_eventBus != null)
            {
                _eventBus.Publish(new IAPPurchaseCompleteEvent(productId, success));
            }
        }

        public void OnPurchaseFailed(string errorMessage)
        {
            if (_eventBus != null)
            {
                var parts = errorMessage.Split(':');
                var productId = parts.Length > 0 ? parts[0].Trim() : "unknown";
                var reason = parts.Length > 1 ? parts[1].Trim() : errorMessage;
                _eventBus.Publish(new IAPPurchaseFailedEvent(productId, reason));
            }
        }

        public void OnRestoreCompleted(bool success, string error = null)
        {
            if (_eventBus != null)
            {
                _eventBus.Publish(new IAPRestoreCompleteEvent(success, error));
            }
        }
    }
}
