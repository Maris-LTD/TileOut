using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.UI;
using GameModules.UI.NavBar;
using GameModules.UI.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using uPools;
using VContainer;

namespace MateInN.UI
{
    public class HomePage : BasePage
    {
        [SerializeField] private Transform _levelChainParent;
        [SerializeField] private GameObject _levelChainPrefab;
        [SerializeField] private Button _settingButton;

        [FormerlySerializedAs("_testLoadingButton")] [SerializeField]
        private Button playButton;

        private readonly List<LevelChainNode> _levelChainNodes = new();

        private IUINavigationService _uiService;

        protected override void OnDependencyInjected()
        {
            base.OnDependencyInjected();
            _uiService = _objectResolver.Resolve<IUINavigationService>();
            UpdateLevelChain(true);
        }

        protected override void Awake()
        {
            base.Awake();
            _settingButton.onClick.RemoveAllListeners();
            _settingButton.onClick.AddListener(OnClickSettingButton);

            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(OnPlayClick);
            }

            UpdateLevelChain(false);
        }

        protected override void OnPageTransitionComplete()
        {
            base.OnPageTransitionComplete();
            UpdateLevelChain(true);
        }

        private int CalculateAllNodeOnLevelChain() { return 4; }

        private void UpdateLevelChain(bool isSetUpNow)
        {
            foreach (var node in _levelChainNodes)
            {
                node.transform.SetParent(null);
                SharedGameObjectPool.Return(node.gameObject);
            }

            _levelChainNodes.Clear();

            int currentLevelIndex = 5;
            int count = CalculateAllNodeOnLevelChain();

            for (int i = 0; i < count; i++)
            {
                var obj = SharedGameObjectPool.Rent(_levelChainPrefab, _levelChainParent)
                    .GetComponent<LevelChainNode>();

                int levelIndex = currentLevelIndex + (count - i - 1);
                obj.SetUp(levelIndex);
                _levelChainNodes.Add(obj);
            }

            if (isSetUpNow)
            {
                _levelChainNodes[^1].SetNextLevel(0.5f);
            }
        }

        private async void OnClickSettingButton()
        {
            try
            {
                var data = new ConfirmPopupData
                {
                    Title = "Settings", Message = "Coming soon!", AcceptText = "Accept"
                };
                await _uiService.ShowPopupAsync<ConfirmPopupData, ConfirmPopupResult>(
                    "SettingPopup",
                    data
                );
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnPlayClick()
        {
            int currentLevel = 1;
            ShowLoadingPopup(currentLevel).Forget();
        }

        private async UniTaskVoid ShowLoadingPopup(int levelIndex)
        {
            var data = new LoadingPopupData()
            {
                LoadingText = "Loading...",
            };

            try
            {
                _uiService.ShowPopupAsync<LoadingPopupData, LoadingPopupResult>("LoadingPopup", data).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(0.4f)
                    , cancellationToken: this.GetCancellationTokenOnDestroy());
                await ShowGameplaySheet(levelIndex);
            }
            finally
            {
                await _uiService.ClosePopupAsync(playAnimation: true);
            }
        }

        private async UniTask ShowGameplaySheet(int levelIndex)
        {
            try
            {
                var sheetData = new InGameSheetData()
                {
                    Level = levelIndex
                };

                var result = await _uiService.ShowSheetAsync<InGameSheetData, InGameSheetResult>(
                    "InGameSheet",
                    sheetData
                );
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}