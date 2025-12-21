using Cysharp.Threading.Tasks;
using GameModules.UI.Examples;
using GameModules.UI.Services;
using UnityEngine;
using VContainer;

namespace GameModules.UI.Examples
{
    public class UIUsageExample : MonoBehaviour
    {
        private IUINavigationService _uiService;

        [Inject]
        public void Construct(IUINavigationService uiService)
        {
            _uiService = uiService;
        }

        private async void Start()
        {
            await DemoUI();
        }

        private async UniTask DemoUI()
        {
            await UniTask.Delay(1000);

            var screenData = new ExampleScreenData { Title = "Main Menu" };
            var screenResult = await _uiService.PushScreenAsync<ExampleScreenData, ExampleScreenResult>(
                "Prefabs/Screens/ExampleScreen",
                screenData
            );

            if (screenResult != null && screenResult.Success)
            {
                Debug.Log("Screen closed successfully");
            }

            await UniTask.Delay(2000);

            var popupData = new ExamplePopupData 
            { 
                Message = "Are you sure you want to continue?",
                ConfirmText = "Yes",
                CancelText = "No"
            };
            
            var popupResult = await _uiService.ShowPopupAsync<ExamplePopupData, ExamplePopupResult>(
                "Prefabs/Popups/ExamplePopup",
                popupData
            );

            if (popupResult != null)
            {
                if (popupResult.Confirmed)
                {
                    Debug.Log("User confirmed");
                }
                else
                {
                    Debug.Log("User cancelled");
                }
            }
        }

        public async void OnOpenScreenButtonClicked()
        {
            var data = new ExampleScreenData { Title = "New Screen" };
            await _uiService.PushScreenAsync<ExampleScreenData, ExampleScreenResult>(
                "Prefabs/Screens/ExampleScreen",
                data
            );
        }

        public async void OnOpenPopupButtonClicked()
        {
            var data = new ExamplePopupData { Message = "Hello from Popup!" };
            var result = await _uiService.ShowPopupAsync<ExamplePopupData, ExamplePopupResult>(
                "Prefabs/Popups/ExamplePopup",
                data
            );
            
            Debug.Log($"Popup result: {(result?.Confirmed ?? false)}");
        }

        public async void OnCloseScreenButtonClicked()
        {
            await _uiService.PopScreenAsync();
        }

        public async void OnClosePopupButtonClicked()
        {
            await _uiService.ClosePopupAsync();
        }
    }
}

