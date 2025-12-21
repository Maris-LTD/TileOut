using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameModules.UI.Services;
using IAPModule.Interfaces;
using MateInN.UI.Reward.Adapters;
using VContainer;

namespace MateInN.UI.Reward
{
    public class RewardPopupService
    {
        private readonly IUINavigationService _uiNavigationService;
        private const string RewardPopupResourceKey = "GetRewardPopup";

        [Inject]
        public RewardPopupService(IUINavigationService uiNavigationService)
        {
            _uiNavigationService = uiNavigationService;
        }

        public async UniTask<RewardPopupResult> ShowRewardPopupAsync(List<RewardData> rewards, string title = null)
        {
            var popupData = new RewardPopupData
            {
                Rewards = rewards,
                Title = title
            };

            return await _uiNavigationService.ShowPopupAsync<RewardPopupData, RewardPopupResult>(
                RewardPopupResourceKey,
                popupData
            );
        }

        public async UniTask<RewardPopupResult> ShowRewardPopupAsync<T>(T source, IRewardDataAdapter adapter, string title = null)
        {
            var rewards = adapter.ConvertToRewardData(source);
            return await ShowRewardPopupAsync(rewards, title);
        }

        public async UniTask<RewardPopupResult> ShowIAPRewardPopupAsync(IBaseRewardPayload rewardPayload, string title = null)
        {
            var adapter = new IAPRewardAdapter();
            return await ShowRewardPopupAsync(rewardPayload, adapter, title);
        }

        public async UniTask<RewardPopupResult> ShowIAPRewardPopupAsync(List<IBaseRewardPayload> rewardPayloads, string title = null)
        {
            var adapter = new IAPRewardAdapter();
            return await ShowRewardPopupAsync(rewardPayloads, adapter, title);
        }
    }
}

