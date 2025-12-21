using Cysharp.Threading.Tasks;
using GameModules.ResourceManager;
using GameModules.UI.Services;
using IAPModule.Interfaces;
using MateInN.UI.Reward;
using MateInN.UI.Reward.Adapters;
using VContainer;

namespace IAPModule.Adapters.Shop.Services
{
    public class RewardHandler : IRewardHandler
    {
        private readonly IResourceService _resourceService;
        private readonly IUINavigationService _uiNavigationService;

        private const string RewardPopupResourceKey = "GetRewardPopup";

        public RewardHandler(IResourceService resourceService, IUINavigationService uiNavigationService)
        {
            _resourceService = resourceService;
            _uiNavigationService = uiNavigationService;
        }

        public void AddCoin(int amount)
        {
            if (_resourceService != null && amount > 0)
            {
                _resourceService.Add("gold", amount);
            }
        }

        public async UniTask ShowRewardPopupAsync(IBaseRewardPayload rewardPayload, string title = "REWARDS")
        {
            if (rewardPayload == null || _uiNavigationService == null)
            {
                return;
            }

            var adapter = new IAPRewardAdapter();
            var rewards = adapter.ConvertToRewardData(rewardPayload);

            if (rewards is { Count: > 0 })
            {
                var popupData = new RewardPopupData
                {
                    Rewards = rewards, Title = title
                };

                await _uiNavigationService.ShowPopupAsync<RewardPopupData, RewardPopupResult>(
                    RewardPopupResourceKey,
                    popupData
                );
            }
        }
    }
}