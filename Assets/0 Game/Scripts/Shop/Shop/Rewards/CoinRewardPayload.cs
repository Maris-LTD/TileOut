using Cysharp.Threading.Tasks;
using IAPModule.Adapters.Shop.Services;
using IAPModule.Data;
using UnityEngine;

namespace IAPModule.Adapters.Shop.Rewards
{
    [CreateAssetMenu(fileName = "CoinRewardPayload", menuName = "IAP Module/Rewards/Coin Reward Payload")]
    public class CoinRewardPayload : BaseRewardPayload
    {
        [Header("Coin Reward")]
        [SerializeField] private int coinAmount;
        [SerializeField] private Sprite coinIcon;

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
                _rewardHandler.AddCoin(coinAmount);
                _rewardHandler.ShowRewardPopupAsync(this).Forget();
            }
            else
            {
                Debug.LogError("[CoinRewardPayload] IRewardHandler is null. Cannot add coin reward.");
            }
        }

        public override void OnPurchaseFailed(string productId, string reason)
        {
            Debug.LogWarning($"[CoinRewardPayload] Purchase failed for {productId}: {reason}");
        }
    }
}
