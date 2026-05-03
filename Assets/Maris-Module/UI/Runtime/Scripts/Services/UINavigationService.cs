using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.Core;
using GameModules.UI.Base;
using GameModules.UI.Core;
using GameModules.UI.DTOs;
using UnityEngine;
using VContainer;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Sheets;

namespace GameModules.UI.Services
{
    public class UINavigationService : IUINavigationService
    {
        private readonly UIContainerManager _containerManager;
        private readonly IObjectResolver _resolver;
        private readonly Dictionary<string, int> _sheetIds = new();

        public UINavigationService(UIContainerManager containerManager, IObjectResolver resolver)
        {
            _containerManager = containerManager;
            _resolver = resolver;
        }

        private void InjectDependenciesToScreen(ZBase.UnityScreenNavigator.Core.Screens.Screen screen)
        {
            if (screen is IDependencyInjectable injectable)
            {
                injectable.InjectDependencies(_resolver);
            }
        }

        private void InjectDependenciesToModal(Modal modal)
        {
            if (modal is IDependencyInjectable injectable)
            {
                injectable.InjectDependencies(_resolver);
            }
        }

        private void InjectDependenciesToSheet(Sheet sheet)
        {
            if (sheet is IDependencyInjectable injectable)
            {
                injectable.InjectDependencies(_resolver);
            }
        }

        public async UniTask<TResult> PushScreenAsync<TData, TResult>(
            string resourceKey,
            TData data = null,
            CancellationToken cancellationToken = default)
            where TData : class, IScreenData
            where TResult : class
        {
            if (_containerManager.ScreenContainer == null)
            {
                Debug.LogError("[UINavigationService] ScreenContainer is not initialized");
                return null;
            }

            var options = new ScreenOptions(resourceKey);
            object[] args = data != null ? new object[] { data } : null;

            await _containerManager.ScreenContainer.PushAsync(options, args);
            
            var currentScreen = _containerManager.ScreenContainer.Current.View;
            if (currentScreen != null)
            {
                InjectDependenciesToScreen(currentScreen);
                
                if (currentScreen is BaseScreen<TData, TResult> baseScreen)
                {
                    return baseScreen.GetResult();
                }
            }

            return null;
        }

        public async UniTask PopScreenAsync(CancellationToken cancellationToken = default)
        {
            if (_containerManager.ScreenContainer == null)
            {
                Debug.LogError("[UINavigationService] ScreenContainer is not initialized");
                return;
            }

            await _containerManager.ScreenContainer.PopAsync(true);
        }

        public async UniTask<TResult> ShowPopupAsync<TData, TResult>(
            string resourceKey,
            TData data = null,
            CancellationToken cancellationToken = default)
            where TData : class, IPopupData
            where TResult : class
        {
            return await ShowPopupAsync<TData, TResult>(resourceKey, data, true, cancellationToken);
        }

        public async UniTask<TResult> ShowPopupAsync<TData, TResult>(
            string resourceKey,
            TData data,
            bool playAnimation,
            CancellationToken cancellationToken = default)
            where TData : class, IPopupData
            where TResult : class
        {
            if (_containerManager.PopupContainer == null)
            {
                Debug.LogError("[UINavigationService] PopupContainer is not initialized");
                return null;
            }

            if (_containerManager.PopupContainer.Modals.Count > 0)
            {
                await _containerManager.PopupContainer.PopAsync(playAnimation);
            }

            var options = new ModalOptions(resourceKey, playAnimation);
            object[] args = data != null ? new object[] { data } : null;

            await _containerManager.PopupContainer.PushAsync(options, args);
            
            BasePopup<TData, TResult> basePopup = null;
            if (_containerManager.PopupContainer.Modals.Count > 0)
            {
                var currentModal = _containerManager.PopupContainer.Current.View;
                if (currentModal != null)
                {
                    if (currentModal is BasePopup<TData, TResult> p)
                    {
                        p.Data = data;
                    }
                    
                    InjectDependenciesToModal(currentModal);
                    
                    if (currentModal is BasePopup<TData, TResult> popup)
                    {
                        basePopup = popup;
                    }
                }
            }

            if (basePopup == null)
            {
                return null;
            }

            var initialModalCount = _containerManager.PopupContainer.Modals.Count;
            
            while (_containerManager.PopupContainer.Modals.Count >= initialModalCount)
            {
                await UniTask.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
            }

            return basePopup.GetResult();
        }

        public async UniTask ClosePopupAsync(CancellationToken cancellationToken = default)
        {
            await ClosePopupAsync(true, cancellationToken);
        }

        public async UniTask ClosePopupAsync(bool playAnimation, CancellationToken cancellationToken = default)
        {
            if (_containerManager.PopupContainer == null)
            {
                Debug.LogError("[UINavigationService] PopupContainer is not initialized");
                return;
            }

            await _containerManager.PopupContainer.PopAsync(playAnimation);
        }

        public async UniTask<TResult> ShowSheetAsync<TData, TResult>(
            string resourceKey,
            TData data = null,
            CancellationToken cancellationToken = default)
            where TData : class, ISheetData
            where TResult : class
        {
            if (_containerManager.SheetContainer == null)
            {
                Debug.LogError("[UINavigationService] SheetContainer is not initialized");
                return null;
            }

            var options = new SheetOptions(resourceKey);
            object[] args = data != null ? new object[] { data } : null;

            int sheetId;
            if (_sheetIds.TryGetValue(resourceKey, out var existingId))
            {
                sheetId = existingId;
            }
            else
            {
                sheetId = await _containerManager.SheetContainer.RegisterAsync(options, args);
                _sheetIds[resourceKey] = sheetId;
            }

            await _containerManager.SheetContainer.ShowAsync(sheetId, true, args);
            
            var activeSheet = _containerManager.SheetContainer.ActiveSheet;
            if (activeSheet != null)
            {
                if (activeSheet is BaseSheet<TData, TResult> bs)
                {
                    bs.Data = data;
                }
                
                InjectDependenciesToSheet(activeSheet);
                
                if (activeSheet is BaseSheet<TData, TResult> baseSheet)
                {
                    return baseSheet.GetResult();
                }
            }

            return null;
        }

        public async UniTask CloseSheetAsync(CancellationToken cancellationToken = default)
        {
            if (_containerManager.SheetContainer == null)
            {
                Debug.LogError("[UINavigationService] SheetContainer is not initialized");
                return;
            }

            await _containerManager.SheetContainer.HideAsync(true);
        }
    }
}
