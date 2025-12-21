using System.Collections.Generic;
using GameModules.UI.DTOs;

namespace MateInN.UI.Reward
{
    public class RewardPopupData : IPopupData
    {
        public List<RewardData> Rewards { get; set; }
        public string Title { get; set; }
    }
}

