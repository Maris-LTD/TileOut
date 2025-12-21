using GameModules.Core;
using GameModules.UI.Base;
using GameModules.UI.DTOs;
using GameModules.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI
{
    public class ConfirmPopupData : IPopupData
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string AcceptText { get; set; } = "OK";
    }

    public class ConfirmPopupResult
    {
        public bool Accepted { get; set; }
    }

    [AutoInject]
    public class ConfirmPopup : BasePopup<ConfirmPopupData, ConfirmPopupResult>
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private TMP_Text acceptButtonText;

        private IUINavigationService _uiService;

        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            _uiService = Resolver.Resolve<IUINavigationService>();
        }

        protected override void OnInitialize(ConfirmPopupData data)
        {
            if (data == null) return;

            if (titleText != null)
                titleText.text = data.Title;

            if (messageText != null)
                messageText.text = data.Message;

            if (acceptButtonText != null)
                acceptButtonText.text = data.AcceptText;

            if (acceptButton != null)
                acceptButton.onClick.AddListener(OnAcceptClicked);
        }

        private void OnAcceptClicked()
        {
            SetResult(new ConfirmPopupResult { Accepted = true });
            _uiService.ClosePopupAsync();
        }

        protected override void OnCleanup()
        {
            if (acceptButton != null)
                acceptButton.onClick.RemoveAllListeners();
        }
    }
}

