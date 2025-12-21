using Game.Gameplay.Data;
using Game.Gameplay.Entities;
using Game.Gameplay.System;
using VContainer;

namespace Game.Gameplay.Services
{
    public class GameplayService : IGameplayService
    {
        private readonly GameplayManager _gameplayManager;

        [Inject]
        public GameplayService(GameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        public void LoadLevel(LevelData levelData, int levelNumber)
        {
            _gameplayManager?.LoadLevel(levelData, levelNumber);
        }

        public void ResetLevel()
        {
            _gameplayManager?.ResetLevel();
        }

        public int GetCurrentLevel()
        {
            return _gameplayManager?.CurrentLevel ?? 0;
        }

        public int GetMovesCount()
        {
            return _gameplayManager?.MovesCount ?? 0;
        }

        public bool IsPlaying()
        {
            return _gameplayManager?.IsPlaying ?? false;
        }

        public TileGrid GetTileGrid()
        {
            return _gameplayManager?.TileGrid;
        }
    }
}