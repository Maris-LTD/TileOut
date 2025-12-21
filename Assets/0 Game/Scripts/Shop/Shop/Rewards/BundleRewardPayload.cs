using Cysharp.Threading.Tasks;
using IAPModule.Adapters.Shop.Services;
using IAPModule.Data;
using IAPModule.Interfaces;
using UnityEngine;

namespace IAPModule.Adapters.Shop.Rewards
{
    [CreateAssetMenu(fileName = "BundleRewardPayload", menuName = "IAP Module/Rewards/Bundle Reward Payload")]
    public class BundleRewardPayload : BaseRewardPayload
    {
        [Header("Coin Reward")]
        [SerializeField] private int coinAmount;
        [SerializeField] private Sprite coinIcon;

        [Header("Booster Rewards")]
        [Tooltip("Booster rewards will be added here later")]

        public int CoinAmount => coinAmount;
        public Sprite CoinIcon => coinIcon;

        private static IRewardHandler _rewardHandler;

        public static void SetRewardHandler(IRewardHandler handler)
        {
            _rewardHandler = handler;
        }

        public override void OnPurchaseSuccess(string productId)
        {
            if (_rewardHandler != null)
            {
                if (coinAmount > 0)
                {
                    _rewardHandler.AddCoin(coinAmount);
                }

                OnBoosterRewardsGranted(productId);
                if (this is IBaseRewardPayload payload)
                {
                    _rewardHandler.ShowRewardPopupAsync(payload).Forget();
                }
            }
            else
            {
                Debug.LogError("[BundleRewardPayload] IRewardHandler is null. Cannot add rewards.");
            }
        }

        public override void OnPurchaseFailed(string productId, string reason)
        {
            Debug.LogWarning($"[BundleRewardPayload] Purchase failed for {productId}: {reason}");
        }

        protected virtual void OnBoosterRewardsGranted(string productId)
        {
        }
    }
}
