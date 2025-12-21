using IAPModule.Data;
using UnityEngine;

namespace IAPModule.Adapters.Shop
{
    [CreateAssetMenu(fileName = "ShopPackData", menuName = "IAP Module/Shop Pack Data")]
    public class ShopPackData : IAPProductData
    {
        [Header("Shop Display")]
        [SerializeField] private Sprite icon;
        [SerializeField] private string rewardAmountText;
        [SerializeField] private int maxPurchaseCount = -1;
        [SerializeField] private bool resetable;
        [SerializeField] private int resetHours = 24;
        [SerializeField] private float defaultPrice;

        [Header("Prefab & Priority")]
        [SerializeField] private string prefabPath;
        [SerializeField] private int categoryPriority;
        [SerializeField] private int packPriority;

        public Sprite Icon => icon;
        public string RewardAmountText => rewardAmountText;
        public int MaxPurchaseCount => maxPurchaseCount;
        public float DefaultPrice => defaultPrice;  
        public bool Resetable => resetable;
        public int ResetHours => resetHours;
        public string PrefabPath => prefabPath;
        public int CategoryPriority => categoryPriority;
        public int PackPriority => packPriority;
    }
}