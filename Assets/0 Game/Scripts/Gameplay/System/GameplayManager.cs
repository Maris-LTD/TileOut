using System;
using System.Collections.Generic;
using Game.Gameplay.Data;
using Game.Gameplay.Entities;
using Game.Gameplay.Event;
using Game.Gameplay.Views;
using GameModules.Systems.Events;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.System
{
    public class GameplayManager : IDisposable
    {
        private readonly TileGrid _tileGrid;
        private readonly TileMovementSystem _movementSystem;
        private readonly IGlobalEventBus _eventBus;
        private readonly TileMapSpawner _tileMapSpawner;

        private LevelData _currentLevelData;
        private int _currentLevel;
        private int _movesCount;
        private bool _isPlaying;

        public TileGrid TileGrid => _tileGrid;
        public int CurrentLevel => _currentLevel;
        public int MovesCount => _movesCount;
        public bool IsPlaying => _isPlaying;

        [Inject]
        public GameplayManager(
            TileGrid tileGrid,
            TileMovementSystem movementSystem,
            IGlobalEventBus eventBus,
            TileMapSpawner tileMapSpawner)
        {
            _tileGrid = tileGrid;
            _movementSystem = movementSystem;
            _eventBus = eventBus;
            _tileMapSpawner = tileMapSpawner;
        }

        public void Initialize()
        {
            SubscribeToEvents();
        }

        public void Cleanup()
        {
            UnsubscribeFromEvents();
            Reset();
        }

        private void SubscribeToEvents()
        {
            _eventBus?.Subscribe<TileTappedEvent>(OnTileTapped);
        }

        private void UnsubscribeFromEvents()
        {
            _eventBus?.Unsubscribe<TileTappedEvent>(OnTileTapped);
        }

        public void LoadLevel(LevelData levelData, int levelNumber)
        {
            if (levelData == null || levelData.nodes == null)
            {
                Debug.LogError("Cannot load level: LevelData is null or has no nodes");
                return;
            }

            Reset();

            _currentLevelData = levelData;
            _currentLevel = levelNumber;
            _isPlaying = true;

            CreateTilesFromLevelData(levelData);
            SpawnTileViews();

            _eventBus?.Publish(new LevelInitializedEvent(levelNumber, _tileGrid.TileCount));
        }

        private void SpawnTileViews()
        {
            _tileMapSpawner?.SpawnTileViews(_tileGrid);
        }

        private void CreateTilesFromLevelData(LevelData levelData)
        {
            foreach (var nodeData in levelData.nodes)
            {
                Vector2Int gridPosition = new Vector2Int(
                    Mathf.RoundToInt(nodeData.position.x),
                    Mathf.RoundToInt(nodeData.position.y)
                );

                Tile tile = new Tile(gridPosition, nodeData.direction);
                _tileGrid.AddTile(tile);
            }
        }

        private void OnTileTapped(TileTappedEvent evt)
        {
            if (!_isPlaying)
                return;

            if (evt.Tile == null)
                return;

            if (evt.Tile.IsMoved)
                return;

            TryMoveTile(evt.Tile);
        }

        private void TryMoveTile(Tile tile)
        {
            if (!_movementSystem.CanMove(tile, _tileGrid))
            {
                Vector2Int targetPosition = tile.GetTargetPosition();
                _eventBus?.Publish(new TileBlockedEvent(tile, targetPosition));
                _tileMapSpawner?.PlayBlockedFeedback(tile);
                return;
            }

            Vector2Int oldPosition = tile.GridPosition;
            
            if (_movementSystem.MoveTile(tile, _tileGrid))
            {
                _movesCount++;
                Vector2Int newPosition = tile.GridPosition;
                
                _tileMapSpawner?.UpdateTilePosition(tile, newPosition);
                _eventBus?.Publish(new TileMovedEvent(tile, oldPosition, newPosition));

                CheckWinCondition();
            }
        }

        private void CheckWinCondition()
        {
            if (_tileGrid.TileCount == 0)
            {
                _isPlaying = false;
                _tileMapSpawner?.PlayFadeOutForAll();
                Debug.Log("Complete Game");
                _eventBus?.Publish(new LevelCompletedEvent(_currentLevel));
            }
            else if (!_movementSystem.HasAnyMovableTile(_tileGrid))
            {
                Debug.LogWarning("No more moves available, but tiles remain. Level may be unsolvable.");
            }
        }

        public void Reset()
        {
            _tileMapSpawner?.Cleanup();
            _tileGrid.Clear();
            _currentLevelData = null;
            _currentLevel = 0;
            _movesCount = 0;
            _isPlaying = false;
        }

        public void ResetLevel()
        {
            if (_currentLevelData == null)
                return;

            LoadLevel(_currentLevelData, _currentLevel);
        }

        public List<Tile> GetMovableTiles()
        {
            return _movementSystem.GetAllMovableTiles(_tileGrid);
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}