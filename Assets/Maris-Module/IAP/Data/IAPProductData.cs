using IAPModule.Interfaces;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IAPModule.Data
{
    [CreateAssetMenu(fileName = "IAPProductData", menuName = "IAP Module/Product Data")]
    public class IAPProductData : ScriptableObject, IIAPProductData
    {
        [Header("Product Information")]
        [SerializeField] private string productId;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private ProductType productType = ProductType.Consumable;
        
        [Header("Store Specific IDs")]
        [SerializeField] private string googlePlayId;
        [SerializeField] private string appleStoreId;
        
        [Header("Reward")]
        [SerializeField] private BaseRewardPayload rewardPayload;

        public string ProductId => productId;
        public string DisplayName => displayName;
        public string Description => description;
        public ProductType ProductType => productType;
        public IBaseRewardPayload RewardPayload => rewardPayload;
        
        public string GooglePlayId => googlePlayId;
        public string AppleStoreId => appleStoreId;

        public string GetStoreSpecificId(string storeName)
        {
            switch (storeName)
            {
                case "GooglePlay":
                    return !string.IsNullOrEmpty(googlePlayId) ? googlePlayId : productId;
                case "AppleAppStore":
                    return !string.IsNullOrEmpty(appleStoreId) ? appleStoreId : productId;
                default:
                    return productId;
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(googlePlayId))
                googlePlayId = productId;
            if (string.IsNullOrEmpty(appleStoreId))
                appleStoreId = productId;
        }
    }
}
