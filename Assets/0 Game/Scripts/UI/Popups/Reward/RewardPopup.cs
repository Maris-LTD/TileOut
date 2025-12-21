using System.Collections.Generic;
using GameModules.Core;
using GameModules.UI.Base;
using GameModules.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MateInN.UI.Reward
{
    [AutoInject]
    public class RewardPopup : BasePopup<RewardPopupData, RewardPopupResult>
    {
        [SerializeField] private RectTransform gridRewardContainer;
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private Button okButton;
        [SerializeField] private TMP_Text titleText;

        private IUINavigationService _uiService;
        private readonly List<GameObject> _rewardItems = new List<GameObject>();

        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            _uiService = Resolver.Resolve<IUINavigationService>();
        }

        protected override void OnInitialize(RewardPopupData data)
        {
            if (data == null)
                return;

            ClearRewardItems();

            if (titleText != null && !string.IsNullOrEmpty(data.Title))
            {
                titleText.text = data.Title;
            }

            if (data.Rewards != null && data.Rewards.Count > 0)
            {
                PopulateRewards(data.Rewards);
            }

            if (okButton != null)
            {
                okButton.onClick.AddListener(OnOkClicked);
            }
        }

        private void PopulateRewards(List<RewardData> rewards)
        {
            if (gridRewardContainer == null || rewardItemPrefab == null)
            {
                Debug.LogError("[RewardPopup] Missing gridRewardContainer or rewardItemPrefab reference");
                return;
            }

            foreach (var reward in rewards)
            {
                var itemObj = Instantiate(rewardItemPrefab, gridRewardContainer);
                var itemView = itemObj.GetComponent<RewardItemView>();
                
                if (itemView != null)
                {
                    itemView.Setup(reward);
                }
                else
                {
                    Debug.LogWarning("[RewardPopup] RewardItemView component not found on reward item prefab");
                }

                _rewardItems.Add(itemObj);
            }
        }

        private void ClearRewardItems()
        {
            foreach (var item in _rewardItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            _rewardItems.Clear();
        }

        private void OnOkClicked()
        {
            SetResult(new RewardPopupResult { Collected = true });
            _uiService?.ClosePopupAsync();
        }

        protected override void OnCleanup()
        {
            if (okButton != null)
            {
                okButton.onClick.RemoveAllListeners();
            }

            ClearRewardItems();
        }
    }
}

