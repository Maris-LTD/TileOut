using Game.Gameplay.Entities;
using UnityEngine;

namespace Game.Gameplay.Event
{
    public readonly struct TileTappedEvent
    {
        public Tile Tile { get; }
        public Vector2Int GridPosition { get; }

        public TileTappedEvent(Tile tile)
        {
            Tile = tile;
            GridPosition = tile?.GridPosition ?? default(Vector2Int);
        }
    }

    public readonly struct TileMovedEvent
    {
        public Tile Tile { get; }
        public Vector2Int OldPosition { get; }
        public Vector2Int NewPosition { get; }

        public TileMovedEvent(Tile tile, Vector2Int oldPosition, Vector2Int newPosition)
        {
            Tile = tile;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
    
    public readonly struct TileBlockedEvent
    {
        public Tile Tile { get; }
        public Vector2Int GridPosition { get; }
        public Vector2Int TargetPosition { get; }

        public TileBlockedEvent(Tile tile, Vector2Int targetPosition)
        {
            Tile = tile;
            GridPosition = tile?.GridPosition ?? Vector2Int.zero;
            TargetPosition = targetPosition;
        }
    }

    public readonly struct LevelInitializedEvent
    {
        public int Level { get; }
        public int TileCount { get; }

        public LevelInitializedEvent(int level, int tileCount)
        {
            Level = level;
            TileCount = tileCount;
        }
    }

    public readonly struct LevelCompletedEvent
    {
        public int Level { get; }

        public LevelCompletedEvent(int level)
        {
            Level = level;
        }
    }
    
}