using System.Collections.Generic;
using IAPModule.Data;
using UnityEngine;

namespace IAPModule.Data
{
    [CreateAssetMenu(fileName = "IAPConfig", menuName = "IAP Module/Config")]
    public class IAPConfig : ScriptableObject
    {
        [Header("Environment Settings")]
        [SerializeField] private string environment = "production";
        [SerializeField] private bool enableReceiptValidation = false;
        
        [Header("Dev Mode")]
        [SerializeField] private bool isDevMode = false;
        [Tooltip("When enabled, allows purchases without IAP initialization and always succeeds")]
        
        [Header("Product Data")]
        [SerializeField] private List<IAPProductData> productDataList = new List<IAPProductData>();

        public string Environment => environment;
        public bool EnableReceiptValidation => enableReceiptValidation;
        public bool IsDevMode => isDevMode;
        public List<IAPProductData> ProductDataList => productDataList;
    }
}
