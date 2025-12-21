using System;
// using BayatGames.SaveGameFree;
using UnityEngine;

namespace IAPModule.Adapters.Shop
{
    public class ShopPurchaseManager : MonoBehaviour
    {
        private const string SAVE_IDENTIFIER = "ShopPurchaseHistory";
        private ShopPurchaseHistory _purchaseHistory;

        public static ShopPurchaseManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadHistory();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadHistory()
        {
            // _purchaseHistory = SaveGame.Load<ShopPurchaseHistory>(SAVE_IDENTIFIER, new ShopPurchaseHistory());
            _purchaseHistory = new();
        }

        private void SaveHistory()
        {
            // SaveGame.Save(SAVE_IDENTIFIER, _purchaseHistory);
        }

        public void RecordPurchase(string productId)
        {
            _purchaseHistory.AddPurchase(productId);
            SaveHistory();
        }

        public int GetPurchaseCount(string productId)
        {
            return _purchaseHistory.GetPurchaseCount(productId);
        }

        public DateTime GetLastPurchaseTime(string productId)
        {
            return _purchaseHistory.GetLastPurchaseTime(productId);
        }

        public bool CanPurchase(ShopPackData packData)
        {
            if (packData.MaxPurchaseCount <= 0) return true;

            var purchaseCount = GetPurchaseCount(packData.ProductId);
            if (purchaseCount >= packData.MaxPurchaseCount) return false;

            if (packData.Resetable && packData.ResetHours > 0)
            {
                if (_purchaseHistory.ShouldReset(packData.ProductId, packData.ResetHours))
                {
                    _purchaseHistory.ResetPurchase(packData.ProductId);
                    SaveHistory();
                    return true;
                }
            }

            return purchaseCount < packData.MaxPurchaseCount;
        }

        public bool IsResetable(string productId, int resetHours)
        {
            return _purchaseHistory.ShouldReset(productId, resetHours);
        }
    }
}

