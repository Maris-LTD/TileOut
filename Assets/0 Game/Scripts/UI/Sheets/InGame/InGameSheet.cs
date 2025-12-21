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

namespace Game.UI
{
    public class InGameSheet : BaseSheet<InGameSheetData, InGameSheetResult>
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _totalScoreText;

        private IGameplayService _gameplayService;
        private IGlobalEventBus _eventBus;
        private IUINavigationService _uiService;

        private int _total;
        private int _collected;

        protected override void OnDependenciesInjected()
        {
            base.OnDependenciesInjected();
            _gameplayService = Resolver.Resolve<IGameplayService>();
            _eventBus = Resolver.Resolve<IGlobalEventBus>();
            _uiService = Resolver.Resolve<IUINavigationService>();
            SubscribeToEvents();
            LoadAndInitializeLevel();
        }

        protected override void OnWillEnter()
        {
            base.OnWillEnter();

            if (Data != null)
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

            LevelData levelData = LoadLevelData(Data.Level);
            if (levelData == null)
            {
                Debug.LogError($"InGameSheet: Failed to load LevelData for level {Data.Level}");
                return;
            }

            Debug.Log($"{JsonUtility.ToJson(levelData)}");

            _total = levelData.nodes.Count;
            _collected = 0;

            _gameplayService?.LoadLevel(levelData, Data.Level);

            UpdateCollected(0, _total);
        }

        private LevelData LoadLevelData(int level)
        {
            string path = $"Levels/Level_{level}";
            var levelDataText = Resources.Load<TextAsset>(path);
            var levelData = JsonUtility.FromJson<LevelData>(levelDataText.text);
            if (levelData == null)
            {
                Debug.LogWarning($"LevelData not found at path: {path}. Creating test level data.");
                levelData = CreateTestLevelData();
            }

            return levelData;
        }

        private LevelData CreateTestLevelData()
        {
            LevelData testData = new LevelData
            {
                gridSize = new Vector2Int(5, 5), nodes = new List<NodeData>
                {
                    new NodeData { position = new Vector2(0, 0), direction = DirectionType.Right }
                    , new NodeData { position = new Vector2(1, 0), direction = DirectionType.Up }
                    , new NodeData { position = new Vector2(0, 1), direction = DirectionType.Down },
                }
            };
            return testData;
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
                await _uiService.ShowSheetAsync<OutGameSheetData, OutGameSheetResult>("OutGameSheet");
            }
            else if (result.Action == VictoryAction.NextLevel)
            {
                int nextLevel = Data.Level + 1;
                await LoadNextLevel(nextLevel);
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