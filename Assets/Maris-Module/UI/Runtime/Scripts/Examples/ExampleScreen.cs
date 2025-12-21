using GameModules.UI.Base;
using GameModules.UI.DTOs;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.UI.Examples
{
    public class ExampleScreenData : IScreenData
    {
        public string Title { get; set; }
    }

    public class ExampleScreenResult
    {
        public bool Success { get; set; }
    }

    public class ExampleScreen : BaseScreen<ExampleScreenData, ExampleScreenResult>
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Button closeButton;

        protected override void OnInitialize(ExampleScreenData data)
        {
            if (data != null && titleText != null)
            {
                titleText.text = data.Title;
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        private void OnCloseClicked()
        {
            SetResult(new ExampleScreenResult { Success = true });
        }

        protected override void OnCleanup()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }
    }
}

