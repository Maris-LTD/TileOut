using System.Collections.Generic;
using System.Linq;
using GameModules.DataManager;
using IAPModule.Adapters.Shop;
using UnityEngine;

namespace IAPModule.Adapters.Shop.Data
{
    public class ShopDataProvider : IDataProvider<ShopPurchaseHistory>
    {
        private const string PURCHASE_HISTORY_KEY = "ShopPurchaseHistory";
        private const string ES3_FILE_PATH = "ShopData.es3";


        private List<ShopPackData> _shopPackDataList;
        private ShopPurchaseHistory _purchaseHistory;

        public ShopDataProvider()
        {
            LoadShopPackData();
            LoadPurchaseHistory();
        }

        private void LoadShopPackData()
        {
            var allPackData = Resources.LoadAll<ShopPackData>("");
            _shopPackDataList = allPackData.ToList();
        }

        public List<ShopPackData> GetShopPackDataList()
        {
            return _shopPackDataList ?? new List<ShopPackData>();
        }

        public ShopPackData GetShopPackData(string productId)
        {
            return _shopPackDataList?.FirstOrDefault(p => p.ProductId == productId);
        }

        public ShopPurchaseHistory GetPurchaseHistory()
        {
            return _purchaseHistory ?? new ShopPurchaseHistory();
        }

        public bool LoadData(object data)
        {
            if (data is ShopPurchaseHistory history)
            {
                _purchaseHistory = history;
                SavePurchaseHistory();
                return true;
            }

            if (data is string jsonString)
            {
                try
                {
                    _purchaseHistory = JsonUtility.FromJson<ShopPurchaseHistory>(jsonString);
                    if (_purchaseHistory != null)
                    {
                        SavePurchaseHistory();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool LoadData(ShopPurchaseHistory data)
        {
            if (data != null)
            {
                _purchaseHistory = data;
                SavePurchaseHistory();
                return true;
            }
            return false;
        }

        object IDataProvider.GetData()
        {
            return _purchaseHistory ?? new ShopPurchaseHistory();
        }

        public ShopPurchaseHistory GetData()
        {
            return _purchaseHistory ?? new ShopPurchaseHistory();
        }

        object IDataProvider.SaveData()
        {
            SavePurchaseHistory();
            return _purchaseHistory ?? new ShopPurchaseHistory();
        }

        public ShopPurchaseHistory SaveData()
        {
            SavePurchaseHistory();
            return _purchaseHistory ?? new ShopPurchaseHistory();
        }

        public bool HasData()
        {
            return _purchaseHistory != null;
        }

        public void ClearData()
        {
            _purchaseHistory = new ShopPurchaseHistory();
            SavePurchaseHistory();
        }

        public void SavePurchaseHistory()
        {
            if (_purchaseHistory == null)
            {
                _purchaseHistory = new ShopPurchaseHistory();
            }

            ES3.Save(PURCHASE_HISTORY_KEY, _purchaseHistory, ES3_FILE_PATH);
        }

        private void LoadPurchaseHistory()
        {
            if(ES3.KeyExists(PURCHASE_HISTORY_KEY, ES3_FILE_PATH)){
                try{
                    _purchaseHistory = ES3.Load<ShopPurchaseHistory>(PURCHASE_HISTORY_KEY, ES3_FILE_PATH);
                    if(_purchaseHistory == null){
                        _purchaseHistory = new ShopPurchaseHistory();
                    }
                }
                catch (System.Exception e){
                    Debug.LogError($"Error loading purchase history: {e}");
                    _purchaseHistory = new ShopPurchaseHistory();
                }
            }
            else{
                _purchaseHistory = new ShopPurchaseHistory();
            }
        }
    }
}
