using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Data;
using Game.Gameplay.Event;
using Game.Gameplay.Services;
using Game.UI.Victory;
using GameModules.Systems.Events;
using GameModules.UI.Base;
using GameModules.UI.Services;
using TMPro;
using UnityEngine;
using VContainer;
using GameModules.DataManager;
using Game.Gameplay.Data.Provider;

namespace Game.UI
{
    public class InGameSheet : BaseSheet<InGameSheetData, InGameSheetResult>
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _totalScoreText;

        private IGameplayService _gameplayService;
        private IGlobalEventBus _eventBus;
        private IUINavigationService _uiService;
        private DataManager _dataManager;

        private int _total;
        private int _collected;

        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            _gameplayService = Resolver.Resolve<IGameplayService>();
            _eventBus = Resolver.Resolve<IGlobalEventBus>();
            _uiService = Resolver.Resolve<IUINavigationService>();
            _dataManager = Resolver.Resolve<DataManager>();
            SubscribeToEvents();
            LoadAndInitializeLevel();
        }

        protected override void OnWillEnter()
        {
            base.OnWillEnter();

            if (Data != null && _levelText != null)
            {
                _levelText.text = $"Level {Data.Level}";
            }
        }

        protected override void OnWillExit()
        {
            base.OnWillExit();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _eventBus?.Subscribe<LevelInitializedEvent>(OnLevelInitialized);
            _eventBus?.Subscribe<TileMovedEvent>(OnTileMoved);
            _eventBus?.Subscribe<TileBlockedEvent>(OnTileBlocked);
            _eventBus?.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void UnsubscribeFromEvents()
        {
            _eventBus?.Unsubscribe<LevelInitializedEvent>(OnLevelInitialized);
            _eventBus?.Unsubscribe<TileMovedEvent>(OnTileMoved);
            _eventBus?.Unsubscribe<TileBlockedEvent>(OnTileBlocked);
            _eventBus?.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void LoadAndInitializeLevel()
        {
            if (Data == null)
            {
                Debug.LogError("InGameSheet: Data is null");
                return;
            }

            if (_levelText != null)
            {
                _levelText.text = $"Level {Data.Level}";
            }

            LevelData levelData = LoadLevelData(Data.Level);
            if (levelData == null)
            {
                Debug.LogError($"InGameSheet: Failed to load LevelData for level {Data.Level}");
                return;
            }

            _total = levelData.nodes.Count;
            _collected = 0;

            _gameplayService?.LoadLevel(levelData, Data.Level);

            UpdateCollected(0, _total);
        }

        private LevelData LoadLevelData(int level)
        {
            string path = $"Levels/Level_{level}";
            var levelDataText = Resources.Load<TextAsset>(path);
            if (levelDataText == null)
            {
                levelDataText = Resources.Load<TextAsset>($"Levels/Level_1");
            }
            var levelData = JsonUtility.FromJson<LevelData>(levelDataText.text);
            if (levelData == null)
            {
                Debug.LogWarning($"LevelData not found at path: {path}. Creating test level data.");
            }

            return levelData;
        }
        
        private void OnLevelInitialized(LevelInitializedEvent evt) { }

        private void OnTileMoved(TileMovedEvent evt)
        {
            _collected++;
            UpdateCollected(_collected, _total);
        }

        private void OnTileBlocked(TileBlockedEvent evt) { }

        private async void OnLevelCompleted(LevelCompletedEvent evt)
        {
            try
            {
                await ShowVictoryPopup();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async UniTask ShowVictoryPopup()
        {
            if (Data == null)
                return;

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            var victoryData = new VictoryPopupData
            {
                LevelNumber = Data.Level, Score = _collected, LoadingProgress = 0f
            };

            var result = await _uiService.ShowPopupAsync<VictoryPopupData, VictoryPopupResult>(
                "VictoryPopup",
                victoryData
            );

            if (result != null)
            {
                await HandleVictoryResult(result);
            }
        }

        private async UniTask HandleVictoryResult(VictoryPopupResult result)
        {
            if (result.Action == VictoryAction.Home)
            {
                SaveProgress(Data.Level + 1);
                await GoHome();
            }
            else if (result.Action == VictoryAction.NextLevel)
            {
                int nextLevel = Data.Level + 1;
                SaveProgress(nextLevel);
                await LoadNextLevel(nextLevel);
            }
        }

        private async UniTask GoHome()
        {
            _uiService.ShowPopupAsync<LoadingPopupData, LoadingPopupResult>("LoadingPopup").Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
            try
            {
                await _uiService.CloseSheetAsync();
                await _uiService.ShowSheetAsync<OutGameSheetData, OutGameSheetResult>("OutGameSheet");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await _uiService.ClosePopupAsync();
            }
        }

        private void SaveProgress(int nextLevel)
        {
            if (_dataManager != null)
            {
                var progress = _dataManager.GetData("Game.Gameplay.Data.Provider.GameProgressProvider") as GameProgress;
                if (progress != null && nextLevel > progress.HighestLevel)
                {
                    progress.HighestLevel = nextLevel;
                    _dataManager.SaveData("Game.Gameplay.Data.Provider.GameProgressProvider");
                    Debug.Log($"[InGameSheet] Đã lưu thành công HighestLevel mới: {nextLevel}");
                }
                else if (progress == null)
                {
                    Debug.LogError("[InGameSheet] Tìm thấy DataManager nhưng GetData trả về null!");
                }
            }
            else
            {
                Debug.LogError("[InGameSheet] LỖI: Không có DataManager được Inject. Hãy kiểm tra lại DataManagerInstaller!");
            }
        }

        private async UniTask LoadNextLevel(int nextLevel)
        {
            var sheetData = new InGameSheetData
            {
                Level = nextLevel
            };

            _uiService.ShowPopupAsync<LoadingPopupData, LoadingPopupResult>("LoadingPopup").Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
            try
            {
                await _uiService.CloseSheetAsync();
                await _uiService.ShowSheetAsync<InGameSheetData, InGameSheetResult>(
                    "InGameSheet",
                    sheetData
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await _uiService.ClosePopupAsync();
            }
        }

        private void UpdateCollected(int collected, int total)
        {
            if (_totalScoreText != null)
            {
                _totalScoreText.text = $"{collected}/{total}";
            }
        }
    }
}