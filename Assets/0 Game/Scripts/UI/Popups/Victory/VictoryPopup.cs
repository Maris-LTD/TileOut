using GameModules.UI.Base;
using GameModules.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI.Victory
{
    public class VictoryPopup : BasePopup<VictoryPopupData, VictoryPopupResult>
    {
        [SerializeField] private TMP_Text victoryTitleText;
        [SerializeField] private Image trophyImage;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button nextLevelButton;
        
        private IUINavigationService _uiService;

        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            _uiService = Resolver.Resolve<IUINavigationService>();
        }

        protected override void OnInitialize(VictoryPopupData data)
        {
            if (data == null)
                return;

            if (victoryTitleText != null)
            {
                victoryTitleText.text = "VICTORY!";
            }

            if (homeButton != null)
            {
                homeButton.onClick.AddListener(OnHomeClicked);
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            }
        }

        private void OnHomeClicked()
        {
            SetResult(new VictoryPopupResult { Action = VictoryAction.Home });
            _uiService?.ClosePopupAsync();
        }

        private void OnNextLevelClicked()
        {
            SetResult(new VictoryPopupResult { Action = VictoryAction.NextLevel });
            _uiService?.ClosePopupAsync();
        }

        protected override void OnCleanup()
        {
            if (homeButton != null)
            {
                homeButton.onClick.RemoveAllListeners();
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
            }
        }
    }
}

