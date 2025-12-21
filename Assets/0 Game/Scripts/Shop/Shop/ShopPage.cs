using System.Collections.Generic;
using GameModules.Systems.Events;
using GameModules.UI.NavBar;
using IAPModule.Adapters.Shop.Data;
using IAPModule.Adapters.Shop.Services;
using IAPModule.Events;
using UnityEngine;
using VContainer;

namespace IAPModule.Adapters.Shop
{
    public class ShopPage : BasePage
    {
        [SerializeField] private Transform _shopPackParent;

        private readonly List<ShopPack> _shopPacks = new();

        private ShopDataProvider _dataProvider;
        private IAPGameAdapter _iapAdapter;
        private IShopPurchaseService _purchaseService;
        private IGlobalEventBus _eventBus;
        
        protected override void OnDependencyInjected()
        {
            base.OnDependencyInjected();
            _dataProvider = _objectResolver.Resolve<ShopDataProvider>();
            _iapAdapter = _objectResolver.Resolve<IAPGameAdapter>();
            _purchaseService = _objectResolver.Resolve<IShopPurchaseService>();
            _eventBus = _objectResolver.Resolve<IGlobalEventBus>();
        }

        protected override void Awake()
        {
            base.Awake();
            SubscribeToIAPEvents();
        }

        protected override void OnPageWillEnter()
        {
            base.OnPageWillEnter();
            CreateShopPacks();
        }

        protected override void OnPageDidEnter()
        {
            base.OnPageDidEnter();
            UpdateAllShopPacks();
        }

        private void OnDestroy() { UnsubscribeFromIAPEvents(); }

        private void SubscribeToIAPEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<IAPInitializedEvent>(OnIAPInitialized);
                _eventBus.Subscribe<IAPPurchaseCompleteEvent>(OnPurchaseComplete);
                _eventBus.Subscribe<IAPPurchaseFailedEvent>(OnPurchaseFailed);
            }
        }

        private void UnsubscribeFromIAPEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<IAPInitializedEvent>(OnIAPInitialized);
                _eventBus.Unsubscribe<IAPPurchaseCompleteEvent>(OnPurchaseComplete);
                _eventBus.Unsubscribe<IAPPurchaseFailedEvent>(OnPurchaseFailed);
            }
        }

        private void CreateShopPacks()
        {
            ClearShopPacks();

            if (_dataProvider == null || _shopPackParent == null)
            {
                Debug.LogError($"[ShopPage] Missing required references: {(_dataProvider == null ? "DataProvider" : "ShopDataProvider")}");
                return;
            }

            var packDataList = _dataProvider.GetShopPackDataList();
            var packDataWithInstances = new List<(ShopPackData data, ShopPack instance)>();

            foreach (var packData in packDataList)
            {
                if (packData == null) continue;

                if (string.IsNullOrEmpty(packData.PrefabPath))
                {
                    Debug.LogWarning($"[ShopPage] PrefabPath is empty for {packData.ProductId}. Skipping.");
                    continue;
                }

                var prefab = Resources.Load<GameObject>(packData.PrefabPath);
                if (prefab == null)
                {
                    Debug.LogError(
                        $"[ShopPage] Prefab not found at path: {packData.PrefabPath} for {packData.ProductId}");
                    continue;
                }

                var packObj = Instantiate(prefab, _shopPackParent);
                var shopPack = packObj.GetComponent<ShopPack>();
                if (shopPack != null)
                {
                    shopPack.Setup(packData, _iapAdapter, _purchaseService);
                    packDataWithInstances.Add((packData, shopPack));
                    _shopPacks.Add(shopPack);
                }
                else
                {
                    Debug.LogWarning(
                        $"[ShopPage] ShopPack component not found on prefab {packData.PrefabPath} for {packData.ProductId}");
                    Destroy(packObj);
                }
            }

            SortShopPacksByPriority(packDataWithInstances);
        }

        private void SortShopPacksByPriority(List<(ShopPackData data, ShopPack instance)> packDataWithInstances)
        {
            packDataWithInstances.Sort((a, b) =>
            {
                var categoryCompare = a.data.CategoryPriority.CompareTo(b.data.CategoryPriority);
                if (categoryCompare != 0)
                    return categoryCompare;

                return a.data.PackPriority.CompareTo(b.data.PackPriority);
            });

            for (int i = 0; i < packDataWithInstances.Count; i++)
            {
                packDataWithInstances[i].instance.transform.SetSiblingIndex(i);
            }
        }

        private void ClearShopPacks()
        {
            foreach (var pack in _shopPacks)
            {
                if (pack != null)
                {
                    Destroy(pack.gameObject);
                }
            }

            _shopPacks.Clear();
        }

        private void UpdateAllShopPacks()
        {
            foreach (var pack in _shopPacks)
            {
                if (pack != null)
                {
                    pack.OnIAPInitialized();
                }
            }
        }

        private void OnIAPInitialized(IAPInitializedEvent evt)
        {
            if (evt.Success)
            {
                UpdateAllShopPacks();
            }
        }

        private void OnPurchaseComplete(IAPPurchaseCompleteEvent evt)
        {
            foreach (var pack in _shopPacks)
            {
                if (pack != null)
                {
                    pack.OnPurchaseComplete(evt.ProductId, evt.Success);
                }
            }
        }

        private void OnPurchaseFailed(IAPPurchaseFailedEvent evt)
        {
            Debug.LogWarning($"[ShopPage] Purchase failed for {evt.ProductId}: {evt.Reason}");
        }
    }
}