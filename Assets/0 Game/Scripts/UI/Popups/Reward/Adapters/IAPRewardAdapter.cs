using System.Collections.Generic;
using IAPModule.Adapters.Shop.Rewards;
using IAPModule.Data;
using IAPModule.Interfaces;

namespace MateInN.UI.Reward.Adapters
{
    public class IAPRewardAdapter : IRewardDataAdapter
    {
        public List<RewardData> ConvertToRewardData(object source)
        {
            var rewards = new List<RewardData>();

            if (source == null)
                return rewards;

            if (source is IBaseRewardPayload singlePayload)
            {
                var reward = ConvertSinglePayload(singlePayload);
                if (reward != null)
                    rewards.Add(reward);
            }
            else if (source is List<IBaseRewardPayload> payloadList)
            {
                foreach (var payload in payloadList)
                {
                    var reward = ConvertSinglePayload(payload);
                    if (reward != null)
                        rewards.Add(reward);
                }
            }
            else if (source is BaseRewardPayload basePayload)
            {
                var reward = ConvertSinglePayload(basePayload);
                if (reward != null)
                    rewards.Add(reward);
            }
            else if (source is List<BaseRewardPayload> basePayloadList)
            {
                foreach (var payload in basePayloadList)
                {
                    var reward = ConvertSinglePayload(payload);
                    if (reward != null)
                        rewards.Add(reward);
                }
            }

            return rewards;
        }

        private RewardData ConvertSinglePayload(IBaseRewardPayload payload)
        {
            if (payload == null)
                return null;

            if (payload is CoinRewardPayload coinPayload)
            {
                return new RewardData
                {
                    Icon = coinPayload.CoinIcon,
                    Amount = FormatAmount(coinPayload.CoinAmount),
                    Type = "Coin",
                    Name = "Coin"
                };
            }

            if (payload is BundleRewardPayload bundlePayload)
            {
                return new RewardData
                {
                    Icon = bundlePayload.CoinIcon,
                    Amount = FormatAmount(bundlePayload.CoinAmount),
                    Type = "Coin",
                    Name = "Coin"
                };
            }

            return new RewardData
            {
                Icon = null,
                Amount = "",
                Type = payload.GetType().Name,
                Name = payload.GetRewardDescription()
            };
        }

        private string FormatAmount(int amount)
        {
            return amount.ToString("N0");
        }
    }
}

