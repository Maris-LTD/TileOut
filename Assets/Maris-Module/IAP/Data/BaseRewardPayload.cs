using IAPModule.Interfaces;
using UnityEngine;

namespace IAPModule.Data
{
    [CreateAssetMenu(fileName = "BaseRewardPayload", menuName = "IAP Module/Base Reward Payload")]
    public abstract class BaseRewardPayload : ScriptableObject, IBaseRewardPayload
    {
        public abstract void OnPurchaseSuccess(string productId);
        public abstract void OnPurchaseFailed(string productId, string reason);
        
        public virtual string GetRewardDescription()
        {
            return "";
        }
    }
}
