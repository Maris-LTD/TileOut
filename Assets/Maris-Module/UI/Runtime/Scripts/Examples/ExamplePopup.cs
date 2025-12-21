using GameModules.UI.Base;
using GameModules.UI.DTOs;
using GameModules.UI.Results;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.UI.Examples
{
    public class ExamplePopupData : IPopupData
    {
        public string Message { get; set; }
        public string ConfirmText { get; set; } = "OK";
        public string CancelText { get; set; } = "Cancel";
    }

    public class ExamplePopupResult
    {
        public bool Confirmed { get; set; }
    }

    public class ExamplePopup : BasePopup<ExamplePopupData, ExamplePopupResult>
    {
        [SerializeField] private Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        protected override void OnInitialize(ExamplePopupData data)
        {
            if (data != null)
            {
                if (messageText != null)
                {
                    messageText.text = data.Message;
                }

                if (confirmButton != null)
                {
                    var confirmText = confirmButton.GetComponentInChildren<Text>();
                    if (confirmText != null)
                    {
                        confirmText.text = data.ConfirmText;
                    }
                    confirmButton.onClick.AddListener(OnConfirmClicked);
                }

                if (cancelButton != null)
                {
                    var cancelText = cancelButton.GetComponentInChildren<Text>();
                    if (cancelText != null)
                    {
                        cancelText.text = data.CancelText;
                    }
                    cancelButton.onClick.AddListener(OnCancelClicked);
                }
            }
        }

        private void OnConfirmClicked()
        {
            SetResult(new ExamplePopupResult { Confirmed = true });
        }

        private void OnCancelClicked()
        {
            SetResult(new ExamplePopupResult { Confirmed = false });
        }

        protected override void OnCleanup()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
            }
        }
    }
}

