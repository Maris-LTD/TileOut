using System.Collections.Generic;

namespace MateInN.UI.Reward.Adapters
{
    public interface IRewardDataAdapter
    {
        List<RewardData> ConvertToRewardData(object source);
    }
}

