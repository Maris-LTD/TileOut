using Game.Gameplay.Data;
using Game.Gameplay.Entities;

namespace Game.Gameplay.Services
{
    public interface IGameplayService
    {
        void LoadLevel(LevelData levelData, int levelNumber);
        void ResetLevel();
        int GetCurrentLevel();
        int GetMovesCount();
        bool IsPlaying();
        TileGrid GetTileGrid();
    }
}