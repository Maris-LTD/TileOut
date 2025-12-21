using System.Collections.Generic;
using Game.Gameplay.Data;
using UnityEngine;

namespace Game.Gameplay.Entities
{
    public class TileGrid
    {
        private readonly Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

        public int TileCount => _tiles.Count;

        public void AddTile(Tile tile)
        {
            if (tile == null) return;
            _tiles.TryAdd(tile.GridPosition, tile);
        }

        public void RemoveTile(Vector2Int gridPosition)
        {
            _tiles.Remove(gridPosition);
        }

        public void RemoveTile(Tile tile)
        {
            if (tile == null) return;
            _tiles.Remove(tile.GridPosition);
        }

        public Tile GetTileAt(Vector2Int gridPosition)
        {
            return _tiles.GetValueOrDefault(gridPosition);
        }

        public bool IsPositionOccupied(Vector2Int gridPosition)
        {
            return _tiles.ContainsKey(gridPosition);
        }

        public bool IsDirectionBlockedByOtherTile(Vector2Int position, Tile executeTile)
        {
            if (executeTile == null)
            {
                return false;
            }
            
            var direction = Vector2Int.zero;
            switch (executeTile.Direction)
            {
                case DirectionType.Up:
                    direction = Vector2Int.up;
                    break;
                case DirectionType.Down:
                    direction = Vector2Int.down;
                    break;
                case DirectionType.Left:
                    direction = Vector2Int.left;
                    break;
                case DirectionType.Right:
                    direction = Vector2Int.right;
                    break;
            }
            var pos = executeTile.GridPosition;
            var targetPosition = executeTile.GetTargetPosition();
            var distance = Vector2Int.Distance(pos, targetPosition);
            while ((pos.x != targetPosition.x || pos.y != targetPosition.y) && (distance > 0))
            {
                distance -= 1;
                pos += direction;
                if (IsPositionOccupied(pos))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            return _tiles.Values;
        }

        public void Clear()
        {
            _tiles.Clear();
        }

        public bool HasTile(Tile tile)
        {
            if (tile == null) return false;
            return _tiles.ContainsKey(tile.GridPosition);
        }
    }
}