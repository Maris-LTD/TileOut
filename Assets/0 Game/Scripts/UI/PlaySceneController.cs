using GameModules.Core;
using GameModules.UI.Services;
using UnityEngine;
using VContainer;

namespace Game.UI
{
    [AutoInject]
    public class PlaySceneController : MonoBehaviour
    {
        private IUINavigationService _uiNavigationService;

        [Inject]
        private void OnInject(IUINavigationService uiNavigationService)
        {
            _uiNavigationService = uiNavigationService;
        }

        private void Start()
        {
            _uiNavigationService.ShowSheetAsync<OutGameSheetData, OutGameSheetResult>("OutGameSheet");    
        }
    }
}